﻿using System;
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


namespace IRC_Client_WPF {
    public static class Util {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


        public static void print(string str, ConsoleColor col = ConsoleColor.Gray) {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = col;
            Console.WriteLine(str);
            Console.ForegroundColor = old;
        }

        /*public static Dictionary<string, string> regexMatch(string tar, string regex, RegexOptions option) {
            Regex rgx = new Regex(regex, option);
            Match match = rgx.Match(tar);

            if (match.Success) {
                var tempD = new Dictionary<string, string>();
                
                foreach (GroupCollection g in match.Groups)
                    tempD[g.V]

            }

        }*/

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
    }
}
