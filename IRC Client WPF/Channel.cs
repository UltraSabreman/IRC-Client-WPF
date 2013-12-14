using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;

//TOOD: proper public/provate shiz.
namespace IRC_Client_WPF {
    public partial class Channel : TreeViewItem {
        public List<string> nicks = new List<string>();
        public List<string> buffer = new List<string>();
        public List<string> newBuffer = new List<string>();
        public Server server;
        
        public string channelName;

        ////////////////////////////////
        public event EventHandler<ChannelUpdate> OnUpdate;

        public Channel(Server s, string name) {
            server = s;
            channelName = name;
            Header = name;
            PopulateOutDict();
        }

        public void parseOutgoing(string s) {
            if (s == "" || s == null) return;

            try {
                var rDict = Util.regexMatch(s, @"^(/(?<command>\S+) )?(?<text>.+)$", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

                string Command = rDict["command"].ToUpper();
                string Text = rDict["text"].TrimEnd("\n\r".ToCharArray());

                if (String.IsNullOrEmpty(Command) && s.StartsWith("/")) {
                    server.serverChannel.addLine("Invalid Command");
                    return;
                }

                try {
                    OutCommandDict[Command](Text);
                } catch (ArgumentOutOfRangeException e) {
                    server.serverChannel.addLine("Invalid Command");
                }
                
            } catch { }
        }

        public int longestNick() {
            int l = 0;
            foreach (string s in nicks) {
                if (s.Length > l)
                    l = s.Length;
            }
            return l;
        }

        public static Channel operator +(Channel c, string s) {
            c.addLine(s);
            return c;
        }

        public void addLine(string s) {
            newBuffer.Add(s);
            Header = channelName + " (" + newBuffer.Count.ToString() + ")";
            if (OnUpdate != null)
                OnUpdate(this, new ChannelUpdate(this));
        }

        public void sync() {
            foreach (string s in newBuffer)
                buffer.Add(s);

            newBuffer.Clear();
            Header = channelName;
        }

       
    }
}
