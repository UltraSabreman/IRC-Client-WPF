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
using System.IO;

//TOOD: proper public/provate shiz.
namespace IRC_Client_WPF {
    public partial class Channel : TreeViewItem {
        public List<string> nicks = new List<string>();
		public List<string> changedBuffer = new List<string>();
		public FlowDocument buffer = new FlowDocument();

		public int newMesseges = 0;
        public Server server;
		public int LongestNick = 0;
        public string channelName;
		public string topic = "";

		public bool isAdmin = false;
		public bool isConnected = false;

		private bool flop = false; //determiens background for chat box.

        public event EventHandler<ChannelUpdate> OnUpdate;

        public Channel(Server s, string name) {
            server = s;
            channelName = name;
            Header = name;
            PopulateOutDict();
			loadBacklog();
			isConnected = true;

			//this trips when we init the first server (since it starts before the UI).
			//Expands the tree and selects this newly made channel.
			try { server.ExpandSubtree(); } catch (NullReferenceException e) { }
			IsSelected = true;
        }

		public void loadBacklog() {
			string logPath = "logs/" + channelName + ".txt";
			if (File.Exists(logPath)) {
				try {
					//TODO: figure out how to read backwards.
					StreamReader r = new StreamReader(logPath);
					//r.BaseStream.Position = r.BaseStream.Length;
					int numOfLines = 10;
					
					Paragraph temp = new Paragraph();

					string test = r.ReadToEnd();
					r.Close();
					string [] lol = test.Split("\n".ToCharArray());

					if (numOfLines > lol.Length)
						foreach (string s in lol)
							temp.Inlines.Add(new Run(s));
					else
						for (int i = lol.Length - numOfLines; i < lol.Length; i++)
							temp.Inlines.Add(new Run(lol[i]));


					buffer.Blocks.Add(temp);
					
				} catch (AccessViolationException e) {
					Util.print("ERROR: Log reading failed for " + channelName, ConsoleColor.Red);					
				}
			}
		}

		public void dumpLine(string whatToWrite) {
			string logPath = "logs/" + channelName + ".txt";
			if (!Directory.Exists("logs"))
				Directory.CreateDirectory("logs");
			if (!File.Exists(logPath)) 
				File.CreateText(logPath);
			
			try {
				StreamWriter fs = new StreamWriter(logPath, true, new ASCIIEncoding());
				fs.WriteLine(whatToWrite);
				fs.Close();
			} catch (AccessViolationException e) {
				Util.print("ERROR: Log writing failed for " + channelName, ConsoleColor.Red);
			}
		}

		public void parseOutgoing(string s) {
			if (s == "" || s == null) return;

			if (s.StartsWith("/")) {
				try {
					var rDict = Util.regexMatch(s, @"^(/(?<command>\S+))(?<text> .+)?$", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

					string Command = rDict ["command"].ToUpper();
					string Text = rDict ["text"].TrimEnd("\n\r".ToCharArray());

					try {
						if (String.IsNullOrEmpty(Command))
							OutCommandDict [Text](null);
						else
							OutCommandDict [Command](Text);
					} catch (KeyNotFoundException e) {

						server.serverChannel.addLine("Invalid Command");
					}
				} catch (RegexMatchFailedException) {
					Util.print("Failed Msg Parse", ConsoleColor.DarkGreen);
				}

			} else
				OutCommandDict ["PRIVMSG"](s);
		}

        public void updateLongestNick() {
            int l = 0;
            foreach (string s in nicks) {
                if (s.Length > l)
                    l = s.Length;
            }
			LongestNick = l;
        }

        public static Channel operator +(Channel c, string s) {
            c.addLine(s);
            return c;
        }

        public void addLine(string s) {
			//TODO: highlight our name if mentioned.

			buffer.Blocks.Add(formatLine(s));
			changedBuffer.Add(s);
            Header = channelName + " (" + (++newMesseges).ToString() + ")";
			if (channelName == server.info.Name)
				server.Header = Header;

            if (OnUpdate != null)
                OnUpdate(this, new ChannelUpdate(this));

			dumpLine(s);
        }

		private Paragraph formatLine(string line) {
			Paragraph temp = new Paragraph();
			try {
				Brush color = flop ? Brushes.LightGray : Brushes.White;
				flop = !flop;

				var rDict = Util.regexMatch(line, @"^(?<time>\d{2,2}:\d{2,2}:\d{2,2} (?:AM|PM)\s+)(?<nick>\w*): (?<text>.*)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

				string times = String.Format("{0,-" + (12 + LongestNick).ToString() + " }", rDict ["time"]);
				TextBlock time = new TextBlock(new Run(times));
				time.Foreground = Brushes.DarkGray;
				time.Background = flop ? Brushes.LightGray : Brushes.White;
				temp.Inlines.Add(time);

				TextBlock nick = new TextBlock(new Run(rDict ["nick"]));
				nick.Foreground = Brushes.Blue;
				nick.Background = flop ? Brushes.LightGray : Brushes.White;
				temp.Inlines.Add(nick);


				TextBlock sep = new TextBlock(new Run(": "));
				sep.Background = flop ? Brushes.LightGray : Brushes.White;
				Grid.SetColumn(sep, 2);
				temp.Inlines.Add(sep);

				TextBlock text = new TextBlock(new Run(rDict ["text"]));
				text.Background = flop ? Brushes.LightGray : Brushes.White;
				text.Foreground = Brushes.Green;
				text.TextWrapping = TextWrapping.Wrap;
				temp.Inlines.Add(text);

			} catch {
				temp.Inlines.Add(new Run(line));
			}
			return temp;
		}

        public void sync() {
			newMesseges = 0;
            Header = channelName;
			changedBuffer.Clear();
			if (channelName == server.info.Name)
				server.Header = Header;
        }

       
    }
}
