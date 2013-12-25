using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//networking
using System.Net;
using System.Net.Sockets;
//using System.Net.Security; ///do SSL connections later.
//using System.Security.Cryptography.X509Certificates;
//others
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;


namespace IRC_Client_WPF {
	public class ServerInfo {
		public string Name = "";
		public string Address = "";
		public int Port = 0;
		public List<string> channelNames = new List<string>();

		public string Nick = "";
		public string RealName = "";
		public string UserName = "";
		public string UserMode = "";
	}

    public partial class Server {
        private TcpClient connection;
		private Channel serverChannel;
		private List<Channel> channels = new List<Channel>();

		public event EventHandler<ChannelCreatedEvent> OnChannelCreation;
		public event EventHandler<ChannelUpdate> OnChannelUpdate;

		public bool IsConnected { get; private set; }
		public ServerInfo Info { get; private set; }



		public Server(ServerInfo s) {		
			PopulateInDict();

			Connect(s);
		}

		public Server(string serverName, string serverAddress, int serverPort, string nickName, string userName, string realName, string userMode = "0") {
			Info = new ServerInfo();

			Info.Name = serverName;
			Info.Address = serverAddress;
			Info.Port = serverPort;

			Info.Nick = nickName;
			Info.UserName = userName;
			Info.RealName = realName;
			Info.UserMode = userMode;

			Connect();
        }

        public ServerInfo Disconnect() {
            connection.Close();
			IsConnected = false;

			return Info;
        }

		public void Connect(ServerInfo s = null) {
			if (IsConnected) return;
			if (s != null)
				Info = s;

			initServer();
		}

		public Channel joinChannel(string name) {
			//check to see if reconnecting to server channel
			if (Info.Name == name) {
				if (!serverChannel.IsConnected)
					serverChannel.IsConnected = true;
				return serverChannel;
			}

			//check to see if reconnecting to another channel
			foreach (Channel c in channels)
				if (c.Name == name) {
					if (!c.IsConnected)
						c.IsConnected = true;
					return c;
				}

			//make a new channel
			Channel newChan = new Channel(Info, name);
			newChan.MessageDispached += new EventHandler<SendMessage>(handleOutgoingMessage);
			newChan.AddedLineToChannel += new EventHandler<LineAdded>(handleIncomingMessage);

			channels.Add(newChan);

			if (OnChannelCreation != null)
				OnChannelCreation(this, new ChannelCreatedEvent(newChan));

			return newChan;
		}

        private async void sendString(string s) {
			s += "\r\n";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] result = encoding.GetBytes(s);

            try {
                await connection.GetStream().WriteAsync(result, 0, result.Length);
            } catch (ObjectDisposedException) {
				Util.PrintLine("Failed to send Message: " + s, ConsoleColor.Red);
            }
        }

		//TODO: do we keep this?
		private void handleIncomingMessage(object o, LineAdded m) {
			Channel c = o as Channel;
			if (c == null) return;

			if (OnChannelUpdate != null)
				OnChannelUpdate(this, new ChannelUpdate(c));
		}

		private void handleOutgoingMessage(object o, SendMessage m) {
			Channel c = o as Channel;
			if (c == null) return;

			if (!m.HandleNow)
				sendString(m.Msg);
			else
				parseRecivedString(m.Msg);

			if (OnChannelUpdate != null)
				OnChannelUpdate(this, new ChannelUpdate(c));
		}

        private async void listen() {
			//TODO: handle timeouts.
            int bytes = 1;
            while (bytes != 0) { //kills the listener when we D/C
				Byte [] data = new Byte [1024];

                try {
					//if (connection.GetStream().DataAvailable)
						bytes = await connection.GetStream().ReadAsync(data, 0, data.Length);
                } catch (ObjectDisposedException) {
					Util.Print(Info.Name + ": Server Stream Closer", ConsoleColor.Red);
					IsConnected = false;
                    break;
                }

				if (data != null) {
					string [] responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes).Split(new string [] { "\r\n" }, StringSplitOptions.None);

					foreach (string s in responseData)
						if (!String.IsNullOrEmpty(s) && s[0] != '\0')
							parseRecivedString(s);
				}
            }
			IsConnected = false;
        }

		private void initServer() {
			Random getNewPass = new Random();
			serverChannel = new Channel(Info, Info.Name, true);

			connection = new TcpClient(Info.Address, Info.Port);
			connection.ReceiveTimeout = 2000;

			IsConnected = true;

			sendString("PASS " + getNewPass.Next().ToString());
			sendString("NICK " + Info.Nick);
			sendString("USER " + Info.UserName + " " + Info.UserMode + " * :" + Info.RealName);

			listen();

			foreach (string channel in Info.channelNames) {
				sendString("JOIN " + channel);
			}
		}

        private void parseRecivedString(string msg) {
            if (msg == null || msg == "") return;

            Console.WriteLine(msg);
            //parses the message with regex
            try {
                var rDict = Util.regexMatch(msg, @"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

                string Prefix = rDict["prefix"];
                string Command = rDict["command"].ToUpper();
                string Params = rDict["params"].TrimEnd("\r\n".ToCharArray());
                string Trail = rDict["trail"].TrimEnd("\r\n".ToCharArray());

                try {
                    InCommandDict [Command](Prefix, Params, Trail);
                } catch (KeyNotFoundException) {
					printErrorToServer(Params, Trail);
                }

			} catch (RegexMatchFailedException) {
				Util.Print("Failed Msg Parse", ConsoleColor.DarkGreen);
			}
            
        }

		public Channel channelByName(string name) {
            if (name == Info.Name)
                return serverChannel;

			foreach (Channel c in channels)
                if (c.Name == name)
                    return c;
            return null;
        }
    }
}
