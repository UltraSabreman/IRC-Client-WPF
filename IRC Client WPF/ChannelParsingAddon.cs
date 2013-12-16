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
			OutCommandDict ["PRIVMSG"] = (Text) => {
				if (isServChan()) return;
                server.sendString("PRIVMSG " + channelName + " :" + Text + "\r\n");
				server.parseIncoming(":" + server.info.Nick + "!local PRIVMSG " + channelName + " :" + Text + "\r\n");
            };

            OutCommandDict ["JOIN"] = (Text) => {
                server.sendString("JOIN " + Text + "\r\n");
            };

            OutCommandDict ["CONNECT"] = (Text) => {
                string [] split = Text.Split(":".ToCharArray());

                if (split.Length == 1)
                    server.ui.createServer(Text, Text, 6667);
                else
                    server.ui.createServer(Text, split [0], int.Parse(split [1]));
            };

            OutCommandDict ["PART"] = (Text) => {
				if (isServChan()) return;

				if (String.IsNullOrEmpty(Text))
					server.sendString("PART " + channelName + "\r\n");
				else
					server.sendString("PART " + Text + "\r\n");

				isConnected = false;
            };

			OutCommandDict ["TOPIC"] = (Text) => {
				if (isServChan()) return;

				server.sendString("TOPIC " + channelName + " :" + Text + "\r\n");
			};

			OutCommandDict ["CLOSE"] = (Text) => {
				if (isServChan()) return;

				if (isConnected)
					OutCommandDict ["PART"](Text);

				server.closeChannel(this);
			};
        }

		/// <summary>
		/// Checks to see if the current channel is the server's buffer (ie: where all server messeges get dumped)
		/// </summary>
		/// <returns>True if this channel is the server buffer..</returns>
		private bool isServChan() {
			if (channelName == server.info.Name) {
				server.parseIncoming("PRIVMSG " + channelName + " :Has to be a Channel\r\n"); //TODO: find a more approriate messge type.
				return true;
			}
			return false;
		}
    }
}
