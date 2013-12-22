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
    public partial class Channel : TreeViewItem {
        public List<string> nicks = new List<string>();
		public List<string> changedBuffer = new List<string>();
		public FlowDocument buffer = new FlowDocument();

		public int newMesseges = 0;
        public Server server;
		public bool isServerChannel = false;
		public int LongestNick = 0;
        public string channelName;
		public string topic = "";

		public bool isAdmin = false;
		public bool isConnected = false;

		private bool flop = false; //determiens background for chat box.

        public event EventHandler<ChannelUpdate> OnUpdate;

        public Channel(Server s, string name, bool local = false) {
            server = s;
            channelName = name;
            Header = name;
            PopulateOutDict();
			loadBacklog();
			isConnected = true;
			if (local)
				isServerChannel = true;

			//this trips when we init the first server (since it starts before the UI).
			//Expands the tree and selects this newly made channel.
			try { server.ExpandSubtree(); } catch (NullReferenceException) { }
			IsSelected = true;
        }

		public void loadBacklog() {
			string logPath = "logs/" + channelName + ".txt";
			if (File.Exists(logPath)) {
				try {
					//TODO: load form options.
					int numOfLines = 10;
					
					Paragraph tempParagraph = new Paragraph();

					/*string test = r.ReadToEnd();
					r.Close();
					string [] lol = test.Split("\n".ToCharArray());

					if (numOfLines > lol.Length)
						foreach (string s in lol) {
							TextBlock tempBlock = new TextBlock { Text = s + "\n", Foreground = Brushes.LightGray };
							tempParagraph.Inlines.Add(tempBlock);
						}
					else
						for (int i = lol.Length - numOfLines; i < lol.Length; i++) {
							TextBlock tempBlock = new TextBlock { Text = lol [i], Foreground = Brushes.LightGray };
							tempParagraph.Inlines.Add(tempBlock);
						}
					 */
					List<string> file = Util.readFileBackwards(logPath, numOfLines);
					foreach (string s in file) {
						TextBlock tempBlock = new TextBlock { Text = s, Foreground = Brushes.LightGray };
						tempParagraph.Inlines.Add(tempBlock);
					}

					buffer.Blocks.Add(tempParagraph);


					Line line = new Line { X1 = 0, Y1 = 0, X2 = 1000, Y2 = 0, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 2 };
					InlineUIContainer tempInline = new InlineUIContainer(line);
					buffer.Blocks.Add(new Paragraph(tempInline));
					
				} catch (AccessViolationException) {
					Util.PrintLine("ERROR: Log reading failed for " + channelName, ConsoleColor.Red);					
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
			} catch (AccessViolationException) {
				Util.PrintLine("ERROR: Log writing failed for " + channelName, ConsoleColor.Red);
			}
		}

		public void parseOutgoing(string s) {
			if (s == "" || s == null) return;

			if (s.StartsWith("/")) {
				try {
					var rDict = Util.regexMatch(s, @"^(/(?<command>\S+)?)( (?<params>.+))?", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

					string Command = rDict ["command"].ToUpper();
					string Params = rDict ["params"].ToUpper();

					try {
						/*if (String.IsNullOrEmpty(Command))
							OutCommandDict [Params](null);
						else
							OutCommandDict [Command](Text);*/
						OutCommandDict [Command](Params);
					} catch (KeyNotFoundException) {
						//server.serverChannel.addLine("Invalid Command");
						//Silenty continue.
						//TODO: let the user know it failed?
					}
				} catch (RegexMatchFailedException) {
					Util.Print(ConsoleColor.DarkGreen, "Failed OutMsg Parse: ", ConsoleColor.White, s, "\n");
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

        public void addLine(string nick, string s) {
			buffer.Blocks.Add(formatLine(nick, s));
			changedBuffer.Add(s);
            Header = channelName + " (" + (++newMesseges).ToString() + ")";
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
			newMesseges = 0;
            Header = channelName;
			changedBuffer.Clear();
			if (channelName == server.info.Name)
				server.Header = Header;
        }

       
    }
}
