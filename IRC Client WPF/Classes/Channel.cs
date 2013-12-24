using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

//TOOD: proper public/provate shiz.
namespace IRC_Client_WPF {
	public class ChatLine {
		public enum MessageType {Text, Error, Connect, Disconnect};
		public MessageType Type;
		public DateTime Time;
		public string Sender = "";
		public string Text = "";

		public ChatLine(DateTime inTime, string inSender, string inText) {
			Time = inTime;
			Sender = inSender;
			Text = inText;
			Type = MessageType.Text;
		}
		public ChatLine(string inSender, string inText, MessageType inType) {
			Time = DateTime.Now;
			Sender = inSender;
			Text = inText;
			Type = inType;
		}
		public ChatLine(string inSender, string inText) {
			Time = DateTime.Now;
			Sender = inSender;
			Text = inText;
			Type = MessageType.Text;
		}

		public ChatLine(string inText) {
			Time = DateTime.Now;
			Sender = "--";
			Text = inText;
			Type = MessageType.Text;
		}

		public override string ToString() {
			return Time.ToString() + " " + Sender + " : " + Text;
		}
	}

    public partial class Channel {
		private List<string> nicks = new List<string>();
		private List<string> backlog = new List<string>();
		private List<ChatLine> buffer = new List<ChatLine>();
		private List<ChatLine> deltaBuffer = new List<ChatLine>(); //holds all new lines since last channel update.

		private string topic = "";
		
		private bool isServerChannel = false;
		public string Name { get; private set; }
		public bool IsConnected { get; set; }

		private ServerInfo serverInfo = null;

		public event EventHandler<ReciveMessage> OnRecive;
		public event EventHandler<SendMessage> OnSend;

		public Channel(ServerInfo inInfo, string inName, bool local = false) {
			serverInfo = inInfo;
			Name = inName;
            PopulateOutDict();
			loadBacklog();
			IsConnected = true;

			if (local)
				isServerChannel = true;
        }

		public void ParseOutgoing(string s) {
			if (s == "" || s == null) return;

			if (s.StartsWith("/")) {
				try {
					var rDict = Util.regexMatch(s, @"^(/(?<command>\S+)?)( (?<params>.+))?", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

					string Command = rDict ["command"].ToUpper();
					string Params = rDict ["params"].ToUpper();

					try {
						OutCommandDict [Command](Params);
					} catch (KeyNotFoundException) { 
						//TODO: do me
					}
				} catch (RegexMatchFailedException) {
					Util.Print(ConsoleColor.DarkGreen, "Failed OutMsg Parse: ", ConsoleColor.White, s, "\n");
				}

			} else
				OutCommandDict ["PRIVMSG"](s);
		}

		public void PrintToChannel(string nick, string text, ChatLine.MessageType inType) {
			addLine(new ChatLine(nick, text, inType));
		}
        public void PrintToChannel(string nick, string text) {
			addLine(new ChatLine(nick, text));
        }
		public void PrintToChannel(string text) {
			addLine(new ChatLine(text));
		}

		public List<ChatLine> GetNewMessages() {
			List<ChatLine> temp = new List<ChatLine>(deltaBuffer);
			deltaBuffer.Clear();
			return temp;
		}

		public void UserConnect(string name) {
			PrintToChannel("==>", name + " connected.", ChatLine.MessageType.Connect);
			nicks.Add(name);
			nicks.Sort();
		}
		public bool TryUserQuit(string name, string partMessage) {
			if (nicks.Contains(name)) {
				PrintToChannel("<==", name + " disconnected. \"" + partMessage + "\"", ChatLine.MessageType.Disconnect);
				nicks.Remove(name);
				nicks.Sort();
				return true;
			}
			return false;
		}

		public bool TryRenameNick(string oldNick, string newNick) {
			if (nicks.Contains(oldNick)) {
				PrintToChannel(oldNick, "Is now known as:" + newNick);

				nicks.Remove(oldNick);
				nicks.Add(newNick);
				nicks.Sort();

				return true;
			}
			return false;
		}







		private void addLine(ChatLine l) {
			buffer.Add(l);
			deltaBuffer.Add(l);

			if (OnRecive != null)
				OnRecive(this, new ReciveMessage(l.ToString()));

			writeLineToLog(l.ToString());
		}

		private void loadBacklog() {
			string logPath = "logs/" + Name + ".txt";
			if (File.Exists(logPath)) {
				try {
					//TODO: load from options.
					int numOfLines = 10;

					List<string> file = Util.readFileBackwards(logPath, numOfLines);
					foreach (string s in file)
						backlog.Add(s);

				} catch (AccessViolationException) {
					Util.PrintLine("ERROR: Log reading failed for " + Name, ConsoleColor.Red);
				}
			}
		}

		private void writeLineToLog(string whatToWrite) {
			string logPath = "logs/" + Name + ".txt";
			if (!Directory.Exists("logs"))
				Directory.CreateDirectory("logs");
			if (!File.Exists(logPath))
				File.CreateText(logPath);

			try {
				using (StreamWriter fs = new StreamWriter(logPath, true, new ASCIIEncoding()))
					fs.WriteLine(whatToWrite);
			} catch (AccessViolationException) {
				Util.PrintLine("ERROR: Log writing failed for " + Name, ConsoleColor.Red);
			}
		}

    }
}
