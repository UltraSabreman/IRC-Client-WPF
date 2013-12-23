using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;


namespace IRC_Client_WPF {

    public static class Util {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


		/// <summary>
		/// Takes a variable amout of strings and consolecolor objects. 
		/// </summary>
		/// <param name="args"></param>
        public static void Print(params object [] args) {
            ConsoleColor oldF = Console.ForegroundColor;

			foreach (object o in args) {
				if (o.GetType() == typeof(ConsoleColor))
					Console.ForegroundColor = (ConsoleColor)o;
				else if (o.GetType() == typeof(string))
					Console.Write(o as string);
			}

            Console.ForegroundColor = oldF;
        }

		public static void PrintLine(string str, ConsoleColor col = ConsoleColor.Gray) {
			ConsoleColor old = Console.ForegroundColor;
			Console.ForegroundColor = col;
			Console.WriteLine(str);
			Console.ForegroundColor = old;
		}

        public static Dictionary<string, string> regexMatch(string tar, string regex, RegexOptions option) {
            var tempD = new Dictionary<string, string>();

            Regex rgx = new Regex(regex, option);
            Match match = rgx.Match(tar);

            if (match.Success) {
                foreach (string groupName in rgx.GetGroupNames())
                    tempD [groupName] = match.Groups [groupName].Value;
            } else
                throw new RegexMatchFailedException();

            return tempD;
        }

		//From http://www.askernest.com/archive/2008/01/23/how-to-programmatically-change-the-selecteditem-in-a-wpf-treeview.aspx
		public static void SetSelectedItem(ref TreeView control, object item) {
			try {
				DependencyObject dObject = control
					.ItemContainerGenerator
					.ContainerFromItem(item);

				//uncomment the following line if UI updates are unnecessary
				//((TreeViewItem)dObject).IsSelected = true;                

				MethodInfo selectMethod =
				   typeof(TreeViewItem).GetMethod("Select",
				   BindingFlags.NonPublic | BindingFlags.Instance);

				selectMethod.Invoke(dObject, new object [] { true });
			} catch { }
		}

		/// <summary>
		/// Reads numberOfLines lines backwards from the given file (but returns them
		/// in the correct order).
		/// </summary>
		/// <param name="filePath"> the path to the file</param>
		/// <param name="numberOfLines">number of lines to read</param>
		/// <returns>the lines</returns>
		public static List<string> readFileBackwards(string filePath, int numberOfLines) {
			if (numberOfLines <= 0) return null;
			List<string> retList = new List<string>();

			StreamReader r = new StreamReader(filePath);
			string all = r.ReadToEnd();
			r.BaseStream.Seek(0, SeekOrigin.End); //seek to end.

			int counter = numberOfLines; //gotta add one so we read the last line as well.
			try {
				while (counter != 0) {
					int bytes = 0;
					while (streamPeek(r.BaseStream) != '\n') {
						r.BaseStream.Position -= 1;
						bytes++;
					}

					long newPos = (r.BaseStream.Position++ - 1);
					retList.Insert(0, streamRead(r.BaseStream, bytes));
					r.BaseStream.Position = newPos;

					counter--;
				}
			} catch { } finally { r.Close(); }

			return retList;
		}

		public static char streamPeek(Stream s) {
			Byte [] c = new Byte [1];
			s.Read(c, 0, 1);
			s.Position--; //seek one back.

			return Encoding.UTF8.GetString(c, 0, 1)[0];
		}

		public static string streamRead(Stream s, int numOfBytes) {
			Byte [] buffer = new Byte [numOfBytes];
			s.Read(buffer, 0, buffer.Length);
			return Encoding.UTF8.GetString(buffer, 0, buffer.Length).Trim("\r\n\0".ToCharArray());
		}

        //From http://stackoverflow.com/questions/13951303/whats-the-easiest-way-to-clone-a-tabitem-in-wpf
        public static T TrycloneElement<T>(T orig) {
            try {
                string s = XamlWriter.Save(orig);

                StringReader stringReader = new StringReader(s);

                XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());
                XmlReaderSettings sx = new XmlReaderSettings();

                object x = XamlReader.Load(xmlReader);
                return (T)x;
            } catch {
                return (T)((object)null);
            }

        }

		// reply ids
    }
	public class Reply {
		public static string None = "0";
		// Initial
		public static string Welcome = "001";                  // :Welcome to the Internet Relay Network <nickname>
		public static string YourHost = "002";                  // :Your host is <server>; running version <ver>
		public static string Created = "003";                  // :This server was created <datetime>
		public static string MyInfo = "004";                  // <server> <ver> <usermode> <chanmode>
		public static string Map = "005";                  // :map
		public static string EndOfMap = "007";                  // :End of /MAP
		public static string MotdStart = "375";                  // :- server Message of the Day
		public static string Motd = "372";                  // :- <info>
		public static string MotdAlt = "377";                  // :- <info>                                                                        (some)
		public static string MotdAlt2 = "378";                  // :- <info>                                                                        (some)
		public static string MotdEnd = "376";                  // :End of /MOTD command.
		public static string UModeIs = "221";                  // <mode>

		// IsOn/UserHost
		public static string UserHost = "302";                  // :userhosts
		public static string IsOn = "303";                  // :nicknames

		// Away
		public static string Away = "301";                  // <nick> :away
		public static string UnAway = "305";                  // :You are no longer marked as being away
		public static string NowAway = "306";                  // :You have been marked as being away

		// WHOIS/WHOWAS
		public static string WhoisHelper = "310";                  // <nick> :looks very helpful                                                       DALNET
		public static string WhoIsUser = "311";                  // <nick> <username> <address> * :<info>
		public static string WhoIsServer = "312";                  // <nick> <server> :<info>
		public static string WhoIsOperator = "313";                  // <nick> :is an IRC Operator
		public static string WhoIsIdle = "317";                  // <nick> <seconds> <signon> :<info>
		public static string EndOfWhois = "318";                  // <request> :End of /WHOIS list.
		public static string WhoIsChannels = "319";                  // <nick> :<channels>
		public static string WhoWasUser = "314";                  // <nick> <username> <address> * :<info>
		public static string EndOfWhoWas = "369";                  // <request> :End of WHOWAS
		public static string WhoReply = "352";                  // <channel> <username> <address> <server> <nick> <flags> :<hops> <info>
		public static string EndOfWho = "315";                  // <request> :End of /WHO list.
		public static string UserIPs = "307";                  // :userips                                                                         UNDERNET
		public static string UserIP = "340";                  // <nick> :<nickname>=+<user>@<IP.address>                                          UNDERNET

		// List
		public static string ListStart = "321";                  // Channel :Users Name
		public static string List = "322";                  // <channel> <users> :<topic>
		public static string ListEnd = "323";                  // :End of /LIST
		public static string Links = "364";                  // <server> <hub> :<hops> <info>
		public static string EndOfLinks = "365";                  // <mask> :End of /LINKS list.

		// Post-Channel Join
		public static string UniqOpIs = "325";
		public static string ChannelModeIs = "324";                  // <channel> <mode>
		public static string ChannelUrl = "328";                  // <channel> :url                                                                   DALNET
		public static string ChannelCreated = "329";                  // <channel> <time>
		public static string NoTopic = "331";                  // <channel> :No topic is set.
		public static string Topic = "332";                  // <channel> :<topic>
		public static string TopicSetBy = "333";                  // <channel> <nickname> <time>
		public static string NamReply = "353";                  // = <channel> :<names>
		public static string EndOfNames = "366";                  // <channel> :End of /NAMES list.

		// Invitational
		public static string Inviting = "341";                  // <nick> <channel>
		public static string Summoning = "342";

		// Channel Lists
		public static string InviteList = "346";                  // <channel> <invite> <nick> <time>                                                 IRCNET
		public static string EndOfInviteList = "357";                  // <channel> :End of Channel Invite List                                            IRCNET
		public static string ExceptList = "348";                  // <channel> <exception> <nick> <time>                                              IRCNET
		public static string EndOfExceptList = "349";                  // <channel> :End of Channel Exception List                                         IRCNET
		public static string BanList = "367";                  // <channel> <ban> <nick> <time>
		public static string EndOfBanList = "368";                  // <channel> :End of Channel Ban List


		// server/misc
		public static string Version = "351";                  // <version>.<debug> <server> :<info>
		public static string Info = "371";                  // :<info>
		public static string EndOfInfo = "374";                  // :End of /INFO list.
		public static string YoureOper = "381";                  // :You are now an IRC Operator
		public static string Rehashing = "382";                  // <file> :Rehashing
		public static string YoureService = "383";
		public static string Time = "391";                  // <server> :<time>
		public static string UsersStart = "392";
		public static string Users = "393";
		public static string EndOfUsers = "394";
		public static string NoUsers = "395";
		public static string ServList = "234";
		public static string ServListEnd = "235";
		public static string AdminMe = "256";                  // :Administrative info about server
		public static string AdminLoc1 = "257";                  // :<info>
		public static string AdminLoc2 = "258";                  // :<info>
		public static string AdminEMail = "259";                  // :<info>
		public static string TryAgain = "263";                  // :Server load is temporarily too heavy. Please wait a while and try again.

		// tracing
		public static string TraceLink = "200";
		public static string TraceConnecting = "201";
		public static string TraceHandshake = "202";
		public static string TraceUnknown = "203";
		public static string TraceOperator = "204";
		public static string TraceUser = "205";
		public static string TraceServer = "206";
		public static string TraceService = "207";
		public static string TraceNewType = "208";
		public static string TraceClass = "209";
		public static string TraceReconnect = "210";
		public static string TraceLog = "261";
		public static string TraceEnd = "262";

		// stats
		public static string StatsLinkInfo = "211";                  // <connection> <sendq> <sentmsg> <sentbyte> <recdmsg> <recdbyte> :<open>
		public static string StatsCommands = "212";                  // <command> <uses> <bytes>
		public static string StatsCLine = "213";                  // C <address> * <server> <port> <class>
		public static string StatsNLine = "214";                  // N <address> * <server> <port> <class>
		public static string StatsILine = "215";                  // I <ipmask> * <hostmask> <port> <class>
		public static string StatsKLine = "216";                  // k <address> * <username> <details>
		public static string StatsPLine = "217";                  // P <port> <??> <??>
		public static string StatsQLine = "222";                  // <mask> :<comment>
		public static string StatsELine = "223";                  // E <hostmask> * <username> <??> <??>
		public static string StatsDLine = "224";                  // D <ipmask> * <username> <??> <??>
		public static string StatsLLine = "241";                  // L <address> * <server> <??> <??>
		public static string StatsuLine = "242";                  // :Server Up <num> days; <time>
		public static string StatsoLine = "243";                  // o <mask> <password> <user> <??> <class>
		public static string StatsHLine = "244";                  // H <address> * <server> <??> <??>
		public static string StatsGLine = "247";                  // G <address> <timestamp> :<reason>
		public static string StatsULine = "248";                  // U <host> * <??> <??> <??>
		public static string StatsZLine = "249";                  // :info
		public static string StatsYLine = "218";                  // Y <class> <ping> <freq> <maxconnect> <sendq>
		public static string EndOfStats = "219";                  // <char> :End of /STATS report
		public static string StatsUptime = "242";

		// GLINE
		public static string GLineList = "280";                  // <address> <timestamp> <reason>                                                   UNDERNET
		public static string EndOfGLineList = "281";                  // :End of G-line List                                                              UNDERNET

		// Silence
		public static string SilenceList = "271";                  // <nick> <mask>                                                                    UNDERNET/DALNET
		public static string EndOfSilenceList = "272";                  // <nick> :End of Silence List                                                      UNDERNET/DALNET

		// LUser
		public static string LUserClient = "251";                  // :There are <user> users and <invis> invisible on <serv> servers
		public static string LUserOp = "252";                  // <num> :operator(s) online
		public static string LUserUnknown = "253";                  // <num> :unknown connection(s)
		public static string LUserChannels = "254";                  // <num> :channels formed
		public static string LUserMe = "255";                  // :I have <user> clients and <serv> servers
		public static string LUserLocalUser = "265";                  // :Current local users: <curr> Max: <max>
		public static string LUserGlobalUser = "266";                  // :Current global users: <curr> Max: <max>

	} // eo enum Reply

	public class Error {
		// Errors
		public static string NoSuchNick = "401";                  // <nickname> :No such nick
		public static string NoSuchServer = "402";                  // <server> :No such server
		public static string NoSuchChannel = "403";                  // <channel> :No such channel
		public static string CannotSendToChan = "404";                  // <channel> :Cannot send to channel
		public static string TooManyChannels = "405";                  // <channel> :You have joined too many channels
		public static string WasNoSuchNick = "406";                  // <nickname> :There was no such nickname
		public static string TooManyTargets = "407";                  // <target> :Duplicate recipients. No message delivered
		public static string NoColors = "408";                  // <nickname> #<channel> :You cannot use colors on this channel. Not sent: <text>   DALNET
		public static string NoOrigin = "409";                  // :No origin specified
		public static string NoRecipient = "411";                  // :No recipient given (<command>)
		public static string NoTextToSend = "412";                  // :No text to send
		public static string NoTopLevel = "413";                  // <mask> :No toplevel domain specified
		public static string WildTopLevel = "414";                  // <mask> :Wildcard in toplevel Domain
		public static string BadMask = "415";
		public static string TooMuchInfo = "416";                  // <command> :Too many lines in the output; restrict your query                     UNDERNET
		public static string UnknownCommand = "421";                  // <command> :Unknown command
		public static string NoMotd = "422";                  // :MOTD File is missing
		public static string NoAdminInfo = "423";                  // <server> :No administrative info available
		public static string Fileor = "424";
		public static string NoNicknameGiven = "431";                  // :No nickname given
		public static string ErroneusNickname = "432";                  // <nickname> :Erroneus Nickname
		public static string NickNameInUse = "433";                  // <nickname> :Nickname is already in use.
		public static string NickCollision = "436";                  // <nickname> :Nickname collision KILL
		public static string UnAvailResource = "437";                  // <channel> :Cannot change nickname while banned on channel
		public static string NickTooFast = "438";                  // <nick> :Nick change too fast. Please wait <sec> seconds.                         (most)
		public static string TargetTooFast = "439";                  // <target> :Target change too fast. Please wait <sec> seconds.                     DALNET/UNDERNET
		public static string UserNotInChannel = "441";                  // <nickname> <channel> :They aren't on that channel
		public static string NotOnChannel = "442";                  // <channel> :You're not on that channel
		public static string UserOnChannel = "443";                  // <nickname> <channel> :is already on channel
		public static string NoLogin = "444";
		public static string SummonDisabled = "445";                  // :SUMMON has been disabled
		public static string UsersDisabled = "446";                  // :USERS has been disabled
		public static string NotRegistered = "451";                  // <command> :Register first.
		public static string NeedMoreParams = "461";                  // <command> :Not enough parameters
		public static string AlreadyRegistered = "462";                  // :You may not reregister
		public static string NoPermForHost = "463";
		public static string PasswdMistmatch = "464";
		public static string YoureBannedCreep = "465";
		public static string YouWillBeBanned = "466";
		public static string KeySet = "467";                  // <channel> :Channel key already set
		public static string ServerCanChange = "468";                  // <channel> :Only servers can change that mode                                     DALNET
		public static string ChannelIsFull = "471";                  // <channel> :Cannot join channel (+l)
		public static string UnknownMode = "472";                  // <char> :is unknown mode char to me
		public static string InviteOnlyChan = "473";                  // <channel> :Cannot join channel (+i)
		public static string BannedFromChan = "474";                  // <channel> :Cannot join channel (+b)
		public static string BadChannelKey = "475";                  // <channel> :Cannot join channel (+k)
		public static string BadChanMask = "476";
		public static string NickNotRegistered = "477";                  // <channel> :You need a registered nick to join that channel.                      DALNET
		public static string BanListFull = "478";                  // <channel> <ban> :Channel ban/ignore list is full
		public static string NoPrivileges = "481";                  // :Permission Denied- You're not an IRC operator
		public static string ChanOPrivsNeeded = "482";                  // <channel> :You're not channel operator
		public static string CantKillServer = "483";                  // :You cant kill a server!
		public static string Restricted = "484";                  // <nick> <channel> :Cannot kill; kick or deop channel service                      UNDERNET
		public static string UniqOPrivsNeeded = "485";                  // <channel> :Cannot join channel (reason)
		public static string NoOperHost = "491";                  // :No O-lines for your host
		public static string UModeUnknownFlag = "501";                  // :Unknown MODE flag
		public static string UsersDontMatch = "502";                  // :Cant change mode for other users
		public static string SilenceListFull = "511";                   // <mask> :Your silence list is full                                                UNDERNET/DALNET
	};

}
