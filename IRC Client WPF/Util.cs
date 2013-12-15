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


		
        public static void print(string str, ConsoleColor col = ConsoleColor.Gray) {
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
                throw new Exception("Match Failed");

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
