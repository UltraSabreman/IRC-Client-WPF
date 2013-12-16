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
using System.Windows.Controls;
using System.IO;


namespace IRC_Client_WPF {
	public class ServerInfo {
		public string Name = "";
		public string Address = "";
		public int Port = 0;
		public List<string> Channels = new List<string>();

		public string Nick = "";
		public string RealName = "";
		public string UserName = "";
		public string UserMode = "";
	}

    public partial class Server : TreeViewItem {
        public MainWindow ui;
        private TcpClient connection;
        private NetworkStream nwStream;
		public ServerInfo info;

        ////////////////////////////////////
		public bool IsConnected { get; private set; }
        public event EventHandler<ChannelCreatedEvent> OnChannelCreation;
        public Channel serverChannel;

		public Server(ServerInfo s, MainWindow win) {		
			info = s; ui = win;

			initServer();
			foreach (string channel in info.Channels) {
				sendString("JOIN " + channel + "\r\n");
			}
		}

        public Server(string inName, string inAdress, int inPort, MainWindow win) {
            ui = win;
			info = new ServerInfo();

			info.Name = inName;
			info.Address = inAdress;
			info.Port = inPort;

			//TODO: get user info with dialog box.
			info.Nick = "sabreman_test";
			info.UserName = info.Nick;
			info.RealName = "My Real Name";
			info.UserMode = "0";

			initServer();
        }

		private void initServer() {
			PopulateInDict();

			//TODO: get user data form somewhere
			Random getNewPass = new Random();
			serverChannel = new Channel(this, info.Name);
			Header = serverChannel.Header;

			connection = new TcpClient(info.Address, info.Port);
			connection.ReceiveTimeout = 2000;
			nwStream = connection.GetStream();

			IsConnected = true;

			sendString("PASS " + getNewPass.Next().ToString() + "\r\n");
			sendString("NICK " + info.Nick + "\r\n");
			sendString("USER " + info.UserName + " " + info.UserMode + " * :" + info.RealName + "\r\n");

			listen();
		}

        public ServerInfo disconnect() {
            nwStream.Close();
            nwStream.Dispose();

            connection.Close();

			return info;
            //TODO: more stuff here, ie: write channles and buffers to file.
        }

		public void closeChannel(Channel c) {
			if (Items.Contains(c))
				Items.Remove(c);
		}

        public async void sendString(string s) {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] result = encoding.GetBytes(s);

            try {
                await nwStream.WriteAsync(result, 0, result.Length);
            } catch (ObjectDisposedException e) {
                return;
            }


        }

		//TODO: handle timeouts.
        private async void listen() {
            int bytes = 1;
            while (bytes != 0) { //kills the listener when we D/C
				Byte [] data = new Byte [1024];

                try {
					//if (nwStream.DataAvailable)
						bytes = await nwStream.ReadAsync(data, 0, data.Length);
                } catch (ObjectDisposedException e) {
					Util.print(Name + ": Server Stream Closer", ConsoleColor.Red);
                    break;
                }

				if (data != null) {
					string [] responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes).Split(new string [] { "\r\n" }, StringSplitOptions.None);

					foreach (string s in responseData)
						if (!String.IsNullOrEmpty(s) && s[0] != '\0')
							parseIncoming(s);
				}
            }
			IsConnected = false;
        }

		
        public void parseIncoming(string msg) {
            if (msg == null || msg == "") return;

            Console.WriteLine(msg);
            //parses the messege with regex
            try {
                var rDict = Util.regexMatch(msg, @"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

                string Prefix = rDict["prefix"];
                string Command = rDict["command"].ToUpper();
                string Params = rDict["params"].TrimEnd("\r\n".ToCharArray());
                string Trail = rDict["trail"].TrimEnd("\r\n".ToCharArray());

                try {
                    InCommandDict [Command](Prefix, Params, Trail);
                } catch (KeyNotFoundException e) {
                    serverChannel.addLine(msg);
                }

			} catch (RegexMatchFailedException e) {
				Util.print("Failed Msg Parse", ConsoleColor.DarkGreen);
			}
            
        }

        public Channel channelByName(string name) {
            if (name == "null")
                return serverChannel;

            foreach (Channel c in Items)
                if (c.channelName == name)
                    return c;
            return null;
        }

    }
}
