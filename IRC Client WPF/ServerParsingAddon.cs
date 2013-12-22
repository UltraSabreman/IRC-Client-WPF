using System;
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
			InCommandDict ["NICK"] = (Prefix, Params, Trail) => {
				//Params: new nickname
				//Trail: null
				string ourNick = getNickFromPrefix(Prefix);

				foreach (Channel c in Items) {
					if (c.nicks.Contains(ourNick)) {
						c.addLine(ourNick, "Is now known as:" + Params);

						c.nicks.Remove(ourNick);
						c.nicks.Add(Params);
						c.nicks.Sort();
						c.updateLongestNick();
					}
				}
			};

			InCommandDict ["QUIT"] = (Prefix, Params, Trail) => { 
				//Params: null
				//Trail: custom messege;

				string quitter = getNickFromPrefix(Prefix);
				foreach (Channel c in Items) {
					if (c.nicks.Contains(quitter)) {
						c.addLine("<==", quitter + " disconnected. \"" + Trail + "\"");
						c.nicks.Remove(quitter);
						c.nicks.Sort();
						c.updateLongestNick();
					}
				}
			};

			InCommandDict ["JOIN"] = (Prefix, Params, Trail) => {
				//Params: channel name
				//Trail: NULL

				string nick = getNickFromPrefix(Prefix);

				if (nick == info.Nick) {
					foreach (Channel c in Items)
						if (c.channelName == Params) {
							if (!c.isConnected)
								c.isConnected = true;

							ExpandSubtree();
							c.IsSelected = true;
							return;
						}

					Channel newChan = new Channel(this, Params);

					Items.Add(newChan);

					if (!info.Channels.Contains(Params))
						info.Channels.Add(Params);

					if (OnChannelCreation != null)
						OnChannelCreation(this, new ChannelCreatedEvent(newChan));
				}
			};

			InCommandDict ["PRIVMSG"] = (Prefix, Params, Trail) => {
				Channel target = channelByName(Params);
				if (target != null) {
					string nick = Prefix.Split("!".ToCharArray()) [0];
					target.addLine(nick, Trail);
					//colors: ♥04o♥08k♥09a♥11y♥12!♥
				}
			};
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { }; 
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { }; 
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };


			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			InCommandDict [""] = (Prefix, Params, Trail) => { };
			/*InCommandDict [Error.NoSuchChannel] = (Prefix, Params, Trail) => { printToServer(Params + " :No such channel"); };
			InCommandDict [Error.ChannelIsFull] = (Prefix, Params, Trail) => { printToServer(Params + " :Cannot join channel (+l)"); };
			InCommandDict [Error.InviteOnlyChan] = (Prefix, Params, Trail) => { printToServer(Params + " :Cannot join channel (+i)"); };
			InCommandDict [Reply.UModeIs] = (Prefix, Params, Trail) => { printToServer("Mode is: " + Params); };
			InCommandDict [Error.UsersDontMatch] = (Prefix, Params, Trail) => { printToServer(":Cannot change mode for other users"); };
			InCommandDict [Error.UModeUnknownFlag] = (Prefix, Params, Trail) => { printToServer(":Unknown MODE flag"); };
			InCommandDict [Error.PasswdMistmatch] = (Prefix, Params, Trail) => { printToServer(":Password incorrect"); };
			InCommandDict [Reply.YoureOper] = (Prefix, Params, Trail) => { printToServer(":You are now an IRC operator"); };
			InCommandDict [Error.NoOperHost] = (Prefix, Params, Trail) => { printToServer(":No O-lines for your host"); };
			InCommandDict [Error.NeedMoreParams] = (Prefix, Params, Trail) => { printToServer(Params + ":Not enough parameters"); };
			InCommandDict [Error.Restricted] = (Prefix, Params, Trail) => { printToServer(":Your connection is restricted!"); };
			InCommandDict [Error.NickCollision] = (Prefix, Params, Trail) => { 
				var d = Util.regexMatch(Prefix, @"^(?<name>[^!@\0\n\r :]+)(!(?<user>[^!@\0\n\r :]+))?(@(?<host>[^!@\0\n\r :]+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				printToServer(Params + " :Nickname collision KILL from "+d["user"]+"@"+d["host"]);
			};
			InCommandDict [Error.ErroneusNickname] = (Prefix, Params, Trail) => { printToServer(Params + " :Erroneous nickname"); };
			InCommandDict [Error.UnAvailResource] = (Prefix, Params, Trail) => { printToServer(Params + " :Nick/channel is temporarily unavailable"); };
			InCommandDict [Error.NickNameInUse] = (Prefix, Params, Trail) => { printToServer(Params + "Nickname is already in use"); };
			InCommandDict [Error.NoNicknameGiven] = (Prefix, Params, Trail) => { printToServer(":No nickname given"); };*/




			/*InCommandDict ["PING"] = (Prefix, Params, Trail) => {
				sendString("PING :" + Trail + "\r\n");
			};



			InCommandDict [Reply.NamReply] = (Prefix, Params, Trail) => {
				try {
					var rDict = Util.regexMatch(Params, @"^(?<mynick>.*) (?<type>=|\*|@) (?<channel>#\w*)$", RegexOptions.Compiled);
					Channel c = channelByName(rDict ["channel"]);

					foreach (string s in Trail.Split(" ".ToCharArray()))
						c.nicks.Add(s);
				} catch (RegexMatchFailedException) {
					Util.PrintLine("Failed NickParse", ConsoleColor.DarkGreen);
				}
					
			};

			InCommandDict [Reply.EndOfNames] = (Prefix, Params, Trail) => {
				Channel c = channelByName("#" + Params.Split("#".ToCharArray()) [1]);

				c.nicks.Sort();
				c.updateLongestNick();
			};


			InCommandDict [Error.ChanOPrivsNeeded]= (Prefix, Params, Trail) => {
				Channel target = channelByName(Params);
				if (target != null) {
					target += (DateTime.Now.ToString("hh:mm:ss tt") + "\t SERVER : " + Trail);
				}

			};

			InCommandDict [Reply.Topic] = (Prefix, Params, Trail) => {
				Channel target = channelByName(Params.Split(" ".ToCharArray()) [1]);
				if (target != null) {
					target.topic = Trail;
					//TODO: update UI;
				}
			};
			InCommandDict ["TOPIC"] = (Prefix, Params, Trail) => {
				Channel target = channelByName(Params);
				if (target != null) {
					string nick = Prefix.Split("!".ToCharArray()) [0];
					target += (DateTime.Now.ToString("hh:mm:ss tt") + "\t" + nick + " : changed topic to: " + Trail);
					target.topic = Trail;
				}
			};

			InCommandDict ["JOIN"] = (Prefix, Params, Trail) => {
				foreach (Channel c in Items)
					if (c.channelName == Params) {
						if (!c.isConnected)
							c.isConnected = true;

						ExpandSubtree();
						c.IsSelected = true;
						return;
					}

				Channel newChan = new Channel(this, Params);

				Items.Add(newChan);

				if (!info.Channels.Contains(Params))
					info.Channels.Add(Params);

				if (OnChannelCreation != null)
					OnChannelCreation(this, new ChannelCreatedEvent(newChan));
			};*/
        }

		private void printErrorToServer(string Params, string Trail) {
			string [] test = Params.Split(" ".ToCharArray(), 1);
			string newParams;
			if (test.Length != 1) {
				newParams = Params.Split(" ".ToCharArray(), 1) [1];
				printToServer(newParams + ": " + Trail);

			} else { 
				//newParams = Params;
				printToServer(Trail);
			}
		}

		private string getNickFromPrefix(string prefix) {
			try {
				var d = Util.regexMatch(prefix, @"^(?<name>[^!@\0\n\r :]+)(!(?<user>[^!@\0\n\r :]+))?(@(?<host>[^!@\0\n\r :]+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

				return d ["name"];
			} catch { }
			return null;
		}
    }
}
