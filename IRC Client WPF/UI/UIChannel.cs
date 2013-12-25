using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.IO;

//TOOD: proper public/provate shiz.
namespace IRC_Client_WPF {
    public partial class UIChannel : TreeViewItem {
		public FlowDocument UIBuffer = new FlowDocument();

        public UIChannel(Channel c) {
            Header = c.Name;
			c.AddedLineToChannel += new EventHandler<LineAdded>(updateChannelUI);
        }

		private void updateChannelUI(object o, LineAdded m) {

		}

        public void updateLongestNick() {
            int l = 0;
            foreach (string s in nicks) {
                if (s.Length > l)
                    l = s.Length;
            }
			LongestNick = l;
        }

        public void addLine(string nick, string s) {
			buffer.Blocks.Add(formatLine(nick, s));
			changedBuffer.Add(s);
            Header = channelName + " (" + (++newMessages).ToString() + ")";
			if (channelName == server.info.Name)
				server.Header = Header;

            if (OnUpdate != null)
                OnUpdate(this, new ChannelUpdate(this));

			dumpLine(s);
        }

		private Paragraph formatLine(string nick, string line) {
			Paragraph temp = new Paragraph();
			try {
				Brush color = flop ? Brushes.LightGray : Brushes.White;
				flop = !flop;

				//var rDict = Util.regexMatch(line, @"^(?<time>\d{2,2}:\d{2,2}:\d{2,2} (?:AM|PM)\s+)(?<nick>\w*): (?<text>.*)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

				string times = String.Format("{0,-" + (12 + LongestNick).ToString() + " }", DateTime.Now.ToString("hh:mm:ss tt"));
				TextBlock time = new TextBlock(new Run(times));
				time.Foreground = Brushes.DarkGray;
				time.Background = color;
				temp.Inlines.Add(time);

				//TODO: make this dissapear after swithicn channels.
				bool hasMyName = line.Contains(server.info.Nick);
				TextBlock nickBlock = new TextBlock(new Run(nick));
				nickBlock.Foreground = Brushes.Blue;
				//nickBlock.Background = (hasMyName ? Brushes.MediumPurple : color);
				temp.Inlines.Add(nickBlock);


				TextBlock sep = new TextBlock(new Run(": "));
				//sep.Background = color;
				Grid.SetColumn(sep, 2);
				temp.Inlines.Add(sep);

				TextBlock text = new TextBlock(new Run(line));
				//text.Background = color;
				text.Foreground = Brushes.Green;
				text.TextWrapping = TextWrapping.Wrap;
				temp.Inlines.Add(text);

			} catch {
				temp.Inlines.Add(new Run(line));
			}
			return temp;
		}

        public void sync() {
			newMessages = 0;
            Header = channelName;
			changedBuffer.Clear();
			if (channelName == server.info.Name)
				server.Header = Header;
        }

       
    }
}