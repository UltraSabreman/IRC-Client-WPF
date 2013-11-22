using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace IRC_Client_WPF {
    struct Connection {
        public string Name;
        public TcpClient Client;
        public Thread Listener;
    }

    class ConnectionHandler {
        public event EventHandler<MessageRaw> onMessageRaw;
        public event EventHandler<Message> onMessage;

       // private Dictionary<Thread, TcpClient> connections = new Dictionary<Thread, TcpClient>();
        private List<Connection> connections = new List<Connection>();

        public Connection Connect(string host, int port) {
            return Connect(host + ":" + port.ToString(), host, port);
        }

        public Connection Connect(string name, string host, int port) {
            Connection newConnection = new Connection();

            newConnection.Name = name;
            newConnection.Client = new TcpClient(host, port);
            newConnection.Listener = new Thread(new ThreadStart(handleThreadConnections));

            connections.Add(newConnection);

            newConnection.Listener.Start();

            
            return newConnection;
        }

        public void Disconnect(Connection c) {
            if (connections.Contains(c)) {
                c.Listener.Abort();
                connections.Remove(c);
            }
        }

        public async void SendMessege(Connection c, string msg) {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] result = encoding.GetBytes(msg);

            NetworkStream tempStream = c.Client.GetStream();
            await tempStream.WriteAsync(result, 0, result.Length);
        }


        private void handleThreadConnections() {
            Connection curConnection = connections.Find(delegate(Connection c) {
                return c.Listener == Thread.CurrentThread;
            });
           
            //SSL
            /*string test = "";
            SslStream currentStream = new SslStream(curConnection.Client.GetStream(), false, CertificateValidationCallback);
            currentStream.AuthenticateAsClient(test, null, System.Security.Authentication.SslProtocols.Ssl3, false);*/

            //Normal
            NetworkStream currentStream = curConnection.Client.GetStream();

            Int32 bytes = 1;
            while (bytes != 0) { //kills the listener when we D/C
                try {
                    Byte [] data = new Byte [1024]; //buffer for message 1MB
                    bytes = currentStream.Read(data, 0, data.Length);
                    //recives the stream. Could get multiple messeges in one go, have to split them up.
                    string [] responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes).Split(new string [] {"\r\n"}, StringSplitOptions.None);

                    foreach (string s in responseData) {
                        if (onMessageRaw != null)
                            onMessageRaw(this, new MessageRaw(s));

                        parseMessege(s);
                    }
                } catch (System.IO.IOException s) {

                }

                //no need to poll all the time.
                Thread.Sleep(10);
            }

            connections.Remove(curConnection);
        }
        /*static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }*/
        private void parseMessege(string msg) {
            if (msg == null || msg == "") return;

            //parses the messege with regex
            Regex rgx = new Regex(@"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            Match match = rgx.Match(msg);

            if (match.Success && onMessage != null)
                onMessage(this, new Message(match.Groups["prefix"].Value,  match.Groups["command"].Value, match.Groups["params"].Value, match.Groups["trail"].Value));
            
        }
    }

    public class MessageRaw : EventArgs {
        private string _messege;

        internal MessageRaw(string msg) {
            _messege = msg;
        }

        public string Msg {
            get { return _messege; }
        }
    }

    public class Message : EventArgs {
        private string _prefix;
        private string _command;
        private string _params;
        private string _trail;

        internal Message(string pre, string com, string par, string tra) {
            _prefix = pre;
            _command = com;
            _params = par;
            _trail = tra;
        }

        public string Prefix {
            get { return _prefix; }
        }
        public string Command {
            get { return _command; }
        }
        public string Params {
            get { return _params; }
        }
        public string Trail {
            get { return _trail; }
        }

    }
}
