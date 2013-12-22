using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;


namespace IRC_Client_WPF {
    public partial class Channel : TreeViewItem {
        private Dictionary<string, Action<string>> OutCommandDict = new Dictionary<string, Action<string>>();

        private void PopulateOutDict() {
			OutCommandDict ["NICK"] = (Params) => { 
				if (!validParams("Nick", Params, 1)) { return; }
				
				server.sendString("NICK " + Params);
			};

			OutCommandDict ["OPER"] = (Params) => { 
				if (!validParams("OPER", Params, 2)) { return; }

				string [] args = Params.Split(" ".ToCharArray());
				server.sendString("OPER " + args[0] + " " + args[1]);
			};

			OutCommandDict ["MODE"] = (Params) => {
				server.sendString("MODE " + server.info.Nick + " " + Params);
			};

			OutCommandDict ["QUIT"] = (Params) => { 
				if (!String.IsNullOrEmpty(Params))
					server.sendString("QUIT " + Params);
				else
					server.sendString("QUIT");

				//TODO: make sure this works.
				server.ui.Close();
			};

			OutCommandDict ["JOIN"] = (Params) => { 
				server.sendString("JOIN " + Params);
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

			OutCommandDict ["CLOSE"] = (Params) => {
				if (isServChan()) return;

				if (isConnected)
					OutCommandDict ["PART"](Params);

				server.closeChannel(this);
			};

			OutCommandDict ["CONNECT"] = (Params) => {
				string [] split = Params.Split(":".ToCharArray());
				string name = Params;
				try {
					name = Params.Split(" ".ToCharArray(), 1) [1];
				} catch { }


				if (split.Length == 1)
					server.ui.createServer(name, Params, 6667);
				else
					server.ui.createServer(name, split [0], int.Parse(split [1]));
			};

			OutCommandDict ["PRIVMSG"] = (Params) => {
				if (isServChan()) return;
				server.sendString("PRIVMSG " + channelName + " :" + Params);
				server.sendString(":" + server.info.Nick + "!local PRIVMSG " + channelName + " :" + Params, true);
			};

			/*

			OutCommandDict ["PART"] = (Params) => {
				Util.regexMatch(Params, @"^")
				if (String.IsNullOrEmpty(Params))
					server.sendString("PART " + channelName + "\r\n");
				else
					server.sendString("PART " + Params + "\r\n");

				isConnected = false;
			};

			

 

			OutCommandDict ["TOPIC"] = (Params) => {
				if (isServChan()) return;

				server.sendString("TOPIC " + channelName + " :" + Params + "\r\n");
			};

*/
        }

		/// <summary>
		/// Checks to see if the current channel is the server's buffer (ie: where all server messeges get dumped)
		/// </summary>
		/// <returns>True if this channel is the server buffer..</returns>
		private bool isServChan() {
			if (isServerChannel) {
				server.sendString("PRIVMSG " + channelName + " :Has to be a Channel\r\n"); //TODO: find a more approriate messge type.
				return true;
			}
			return false;
		}

		private bool validParams(string name, string args, int numOfParams) {
			if (!String.IsNullOrEmpty(args) && args.Split(" ".ToCharArray()).Length == numOfParams)
				return true;

			server.printToServer("Error: to few argumnets for \"" + name + "\" command.");
			return false;
		}

		private void badFormatError(string name) {
			server.printToServer("Error: \"" + name + "\" command is improperly formatted.");
		}
    }
}
