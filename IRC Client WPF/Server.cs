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
using System.Windows.Controls;
using System.IO;


namespace IRC_Client_WPF {
    public partial class Server : TreeViewItem {
        public MainWindow ui;
        private TcpClient connection;
        private NetworkStream nwStream;

        private string nick;
        private string sessionPass;
        private string realname;
        private string mode;

        private bool connected;

        ////////////////////////////////////
        public bool IsConnected { get {return connected;} }

        public event EventHandler<ChannelCreatedEvent> OnChannelCreation;

        public Channel serverChannel;

        public string serverName;
        public string address;
        public int port;

        public bool local;

        public Server(string inName, string inAdress, int inPort, MainWindow win) {
            serverName = inName; address = inAdress; port = inPort;
            Header = serverName;
            ui = win;

            PopulateInDict();

            //TODO: get user data form somewhere
            Random test = new Random();
            sessionPass = test.Next().ToString();
            realname = "Testing Spaces here";
            nick = "sabreman2";
            mode = "0";

            serverChannel = new Channel(this, "null");

            if (!String.IsNullOrEmpty(address)) {
                connection = new TcpClient(inAdress, inPort);
                connection.ReceiveTimeout = 1;
                nwStream = connection.GetStream();

                connected = true;
                local = false;

                sendString("PASS " + sessionPass + "\r\n");
                sendString("NICK " + nick + "\r\n");
                sendString("USER " + nick + " " + mode + " * :" + realname + "\r\n");

                listen();
            } else 
                local = true;
            
            
            Util.AllocConsole();
        }

        public void disconnect() {
            if (!local) {
                nwStream.Close();
                nwStream.Dispose();

                connection.Close();
            }

            //TODO: more stuff here, ie: write channles and buffers to file.
        }

        public async void sendString(string s) {
            //if this is the main buffer, we dont actualy want to send anything...
            if (local) {
                parseIncoming(s);
                return;
            }

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] result = encoding.GetBytes(s);

            try {
                await nwStream.WriteAsync(result, 0, result.Length);
            } catch (ObjectDisposedException e) {
                return;
            }

        }

        private async void listen() {
            Int32 bytes = 1;
            while (bytes != 0) { //kills the listener when we D/C
                Byte [] data = new Byte [5000];

                try {
                    //TODO: make this read variable length stuff. Also make it faster.
                    bytes = await nwStream.ReadAsync(data, 0, data.Length);
                } catch (ObjectDisposedException e) {
                    break;
                }

                //recives the stream. Could get multiple messeges in one go, have to split them up.
                string [] responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes).Split(new string [] { "\r\n" }, StringSplitOptions.None);

                foreach (string s in responseData) 
                    parseIncoming(s);

            }
            connected = false;
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
                } catch (Exception e) {
                    serverChannel.addLine(msg);
                }
                
            } catch { }
            
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
