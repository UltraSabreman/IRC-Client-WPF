using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace IRC_Client_WPF {
    public partial class Channel : TreeViewItem {
        private Dictionary<string, Action<string>> OutCommandDict = new Dictionary<string, Action<string>>();

        private void PopulateOutDict() {
            OutCommandDict [""] = (Text) => {
                if (server.local) {
                    server.sendString("PRIVMSG null :Has to be a Channel\r\n"); //TODO: find a more approriate messge type.
                    return;
                }

                server.sendString("PRIVMSG " + channelName + " :" + Text + "\r\n");
                addLine("Sabreman : " + Text); //TODO: fix me
            };
            OutCommandDict ["JOIN"] = (Text) => {
                if (server.local) {
                    server.sendString("PRIVMSG null :Has to be a Server\r\n"); //TODO: find a more approriate messge type.
                    return;
                }
                    
                server.sendString("JOIN " + Text + "\r\n");
            };
            OutCommandDict ["CONNECT"] = (Text) => {
                string [] split = Text.Split(new char [] { ':' }); ;

                if (split.Length == 1)
                    server.ui.createServer(Text, Text, 6667);
                else
                    server.ui.createServer(Text, split [0], int.Parse(split [1]));
            };
            OutCommandDict ["PART"] = (Text) => {
                if (server.local) {
                    server.sendString("PRIVMSG null :Has to be a Channel\r\n"); //TODO: find a more approriate messge type.
                    return;
                }

                server.sendString("PART " + Text + "\r\n");
            };
        }
    }
}
