using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IRC_Client_WPF {
    public partial class Server : TreeViewItem {
        private Dictionary<string, Action<string, string, string>> InCommandDict = new Dictionary<string, Action<string, string, string>>();

        private void PopulateInDict() {
            InCommandDict ["PING"] = (Prefix, Params, Trail) => {
                sendString("PING :" + Trail + "\r\n");
            };

            InCommandDict ["PRIVMSG"] = (Prefix, Params, Trail) => {
                Channel target = channelByName(Params);
                if (target != null) {
                    string nick = Prefix.Split(new char [] { '!' }) [0];
                    target += (DateTime.Now.ToString("T") + " " + nick + ": " + Trail);
                }
            };

            InCommandDict ["JOIN"] = (Prefix, Params, Trail) => {
                foreach (Channel c in Items)
                    if (c.channelName == Params) {
                        return;
                        //TODO: channel exists.
                    }

                Channel newChan = new Channel(this, Params);

                Items.Add(newChan);

                if (OnChannelCreation != null)
                    OnChannelCreation(this, new ChannelCreatedEvent(newChan));
            };

        }
    }
}
