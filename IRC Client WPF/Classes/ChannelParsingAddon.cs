using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;


namespace IRC_Client_WPF {
	public partial class Channel {
        private Dictionary<string, Action<string>> OutCommandDict = new Dictionary<string, Action<string>>();

        private void PopulateOutDict() {
			OutCommandDict ["NICK"] = (Params) => { 
				sendMessage("NICK " + Params);
			};

			OutCommandDict ["OPER"] = (Params) => { 
				string [] args = Params.Split(" ".ToCharArray());
				sendMessage("OPER " + args [0] + " " + args [1]);
			};

			OutCommandDict ["MODE"] = (Params) => {
				sendMessage("MODE " + serverInfo.Nick + " " + Params);
			};

			OutCommandDict ["QUIT"] = (Params) => { 
				if (!String.IsNullOrEmpty(Params))
					sendMessage("QUIT " + Params);
				else
					sendMessage("QUIT");
			};

			OutCommandDict ["JOIN"] = (Params) => { 
				sendMessage("JOIN " + Params);
			};

			OutCommandDict ["CONNECT"] = (Params) => {
				sendMessage("CONNECT " + Params);
			};

			OutCommandDict ["CLOSE"] = (Params) => {
				if (IsConnected)
					OutCommandDict ["PART"](Params);

				sendMessage("CLOSE");
			};

			OutCommandDict ["PRIVMSG"] = (Params) => {
				if (isServChan()) return;
				sendMessage("PRIVMSG " + Name + " :" + Params);
				//TODO: fix me?
			};

			OutCommandDict ["PART"] = (Params) => {
				if (String.IsNullOrEmpty(Params))
					sendMessage("PART " + Name);
				else
					sendMessage("PART " + Params);
			};

			OutCommandDict ["TOPIC"] = (Params) => {
				if (isServChan()) return;

				sendMessage("TOPIC " + Name + " :" + Params);
			};

			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };
			OutCommandDict [""] = (Params) => { };

        }

		private bool isServChan() {
			if (isServerChannel) {
				sendMessage("PRIVMSG " + Name + " :Has to be a Channel", true);
				return true;
			}
			return false;
		}

		private void sendMessage(string s, bool h = false) {
			if (OnSend != null) OnSend(this, new SendMessage(s, h));
		}

    }
}
