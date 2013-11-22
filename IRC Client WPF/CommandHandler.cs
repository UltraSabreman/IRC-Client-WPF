using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Text.RegularExpressions;


namespace IRC_Client_WPF {
    public class chatLine {
        public string Server = "";
        public string Channel = "";
        public string User = "";
        public List<Tuple<string, SolidColorBrush>> Line = new List<Tuple<string, SolidColorBrush>>();
    }
    class CommandHandler {
        public delegate void thing(string s);
        public event thing OnResponce;

        public CommandHandler() {

        }
        /// <summary>
        /// Decodes incoming messeges and returns a line struct. 
        /// </summary>
        /// <param name="msg">The recived messege</param>
        /// <returns>chatLine or null if responce is required.</returns>
        public chatLine Decode(Message msg) {
            chatLine line = new chatLine();

            if (msg.Command.ToUpper() == "PING") {
                if (OnResponce != null)
                    OnResponce(msg.Params);
                return null;
            }

            line.Line.Add(new Tuple<string, SolidColorBrush>(DateTime.Now.TimeOfDay.ToString() + "\t", Brushes.Black));
            line.Line.Add(new Tuple<string, SolidColorBrush>(msg.Prefix, Brushes.Purple));
            line.Line.Add(new Tuple<string, SolidColorBrush>(" | ", Brushes.Black));
            line.Line.Add(new Tuple<string, SolidColorBrush>(msg.Params + msg.Trail, Brushes.Red));

         
            return line;
        }

        public string Encode(string s) {
            string msg = "";

            Regex rgx = new Regex(@"^(/(?<command>\S+) )?(?<text>.+)$", RegexOptions.Multiline | RegexOptions.Compiled);
            Match match = rgx.Match(s);
            
            if (match.Success) {
                string command = match.Groups["command"].Value;
                string text = match.Groups["text"].Value;

                if (command == "")
                    msg = text;


            }

            return msg;
        }

    }
}
