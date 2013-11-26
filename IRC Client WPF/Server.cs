using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//networking
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
//others
using System.Threading;
using System.Text.RegularExpressions;


namespace IRC_Client_WPF {
    public class Server {
        private MainWindow ui;
        private TcpClient connection;
        private NetworkStream nwStream;
        private Thread listner;

        private string nick;
        private string sessionPass;
        private string realname;
        private string mode;

        private bool connected;
        ////////////////////////////////////
        public bool IsConnected { get {return connected;} }
        public List<Channel> channels = new List<Channel>();

        public delegate void chanCreated(Channel c);
        public event chanCreated OnChannelCreation;

        public string name;
        public string adress;
        public int port;

        public Server(string inName, string inAdress, int inPort, MainWindow win) {
            name = inName; adress = inAdress; port = inPort;
            ui = win;

            Random test = new Random();
            sessionPass = test.Next().ToString();
            realname = "Testing Spaces here";
            nick = "sabreman2";
            mode = "0";

            channels.Add(new Channel(this, ""));

            connection = new TcpClient(inAdress, inPort);
            nwStream = connection.GetStream();

            listner = new Thread(new ThreadStart(listen));
            listner.Start();

            Util.AllocConsole();
        }

        public void disconnect() {
            listner.Abort();
            
            nwStream.Close();
            nwStream.Dispose();

            connection.Close();

            //TODO: more stuff here, ie: write channles and buffers to file.
        }

        public async void sendString(string s) {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] result = encoding.GetBytes(s);

            await nwStream.WriteAsync(result, 0, result.Length);
        }

        private void listen() {
            Int32 bytes = 1;
            while (bytes != 0) { //kills the listener when we D/C
                Byte [] data = new Byte [1024]; //buffer for message 1MB
                bytes = nwStream.Read(data, 0, data.Length);

                //recives the stream. Could get multiple messeges in one go, have to split them up.
                string [] responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes).Split(new string [] { "\r\n" }, StringSplitOptions.None);

                foreach (string s in responseData) 
                    parseIncoming(s);

            }
            connected = false;
        }

        private void parseIncoming(string msg) {
            if (msg == null || msg == "") return;

            //TODO: replace with actual usfull data (Retrive from settings class?)
            if (!connected) {
                connected = true;
                sendString("PASS " + sessionPass + "\r\n");
                sendString("NICK " + nick + "\r\n");
                sendString("USER " + nick + " " + mode + " * :" + realname + "\r\n");
                return;
            }

            Console.WriteLine(msg);
            //parses the messege with regex
            Regex rgx = new Regex(@"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            Match match = rgx.Match(msg);

            if (match.Success) {
                string Prefix = match.Groups ["prefix"].Value;
                string Command = match.Groups ["command"].Value.ToUpper();
                string Params = match.Groups ["params"].Value.TrimEnd(new char [] { '\n', '\r' });
                string Trail = match.Groups ["trail"].Value.TrimEnd(new char [] { '\n', '\r' });

                if (Command == "PING") {
                    sendString("PING :" + Trail + "\r\n");
                } else if (Command == "PRIVMSG") {
                    Channel target = channelByName(Params);
                    if (target != null) {
                        string nick = Prefix.Split(new char [] { '!' }) [0];
                        target += (DateTime.Now.TimeOfDay.ToString() + " " + nick + "| " + Trail);
                    }

                } else if (Command == "JOIN") {
                    Channel newChan = new Channel(this, Params);
                    channels.Add(newChan);
                    if (OnChannelCreation != null)
                        OnChannelCreation(newChan);
                } 
            }
            
        }

        public void parseOutgoing(string chanName, string s) {
            if (s == "" || s == null) return;

            Regex rgx = new Regex(@"^(/(?<command>\S+) )?(?<text>.+)$", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            Match match = rgx.Match(s);

            if (match.Success) {
                string Command = match.Groups ["command"].Value.ToUpper();
                string Text = match.Groups ["text"].Value.TrimEnd(new char [] { '\n', '\r' });

                if (Command == "") {
                    Channel tarChan = channelByName(chanName);
                    if (tarChan != null) {
                        sendString("PRIVMSG " + chanName + " :" + Text + "\r\n");
                        tarChan += "Sabreman : " + Text; //TODO: fix me
                    } else {
                        channels.First().addLine("Not a channel.");
                        return;
                    }
                } else if (Command == "JOIN") {
                    sendString("JOIN " + Text + "\r\n");
                } else if (Command == "CONNECT") {
                    string [] split = Text.Split(new char [] { ':' });;

                    if (split.Length == 1)
                        ui.newServer(Text, Text, 6667);
                    else
                        ui.newServer(Text, split [0], int.Parse(split [1]));
                }else if (Command == "PART") {
                    sendString("PART " + Text + "\r\n");
                } else {
                    channels.First().addLine("Bad Command");
                }
            }
        }

        public Channel channelByName(string name) {
            foreach (Channel c in channels)
                if (c.channelName == name)
                    return c;
            return null;
        }

    }
}
