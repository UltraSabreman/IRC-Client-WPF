﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;


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
						string nick = Prefix.Split("!".ToCharArray()) [0];
						target += (DateTime.Now.ToString("hh:mm:ss tt") + "\t" + nick + ": " + Trail);
						//colors: ♥04o♥08k♥09a♥11y♥12!♥
					}
				};

				InCommandDict ["RPL_NAMREPLY"] = InCommandDict [353.ToString()] = (Prefix, Params, Trail) => {
					try {
						var rDict = Util.regexMatch(Params, @"^(?<mynick>.*) (?<type>=|\*|@) (?<channel>#\w*)$", RegexOptions.Compiled);
						Channel c = channelByName(rDict ["channel"]);

						foreach (string s in Trail.Split(" ".ToCharArray()))
							c.nicks.Add(s);
					} catch { }
				};

				InCommandDict ["RPL_ENDOFNAMES"] = InCommandDict [366.ToString()] = (Prefix, Params, Trail) => {
					Channel c = channelByName("#" + Params.Split("#".ToCharArray()) [1]);

					c.nicks.Sort();
					c.updateLongestNick();
				};

				InCommandDict ["JOIN"] = (Prefix, Params, Trail) => {
					foreach (Channel c in Items)
						if (c.channelName == Params) {
							return;
							//TODO: channel exists.
						}

					Channel newChan = new Channel(this, Params);

					Items.Add(newChan);

					if (!info.Channels.Contains(Params))
						info.Channels.Add(Params);

					if (OnChannelCreation != null)
						OnChannelCreation(this, new ChannelCreatedEvent(newChan));
				};
        }
    }
}
