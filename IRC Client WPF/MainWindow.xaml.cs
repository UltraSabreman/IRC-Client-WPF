using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;


namespace IRC_Client_WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {
        ConnectionHandler handler;
        Connection freenode;
        Connection rita;
        Connection mirror;
       

        public MainWindow() {
            InitializeComponent();
            //this.Hide();

            Util.AllocConsole();

            handler = new ConnectionHandler();
            freenode = handler.Connect("irc.freenode.net", 6667);
            //rita = handler.Connect("irc.cat.pdx.edu", 6667);
            //mirror = handler.Connect("localhost", 9090);
            handler.onMessage += new EventHandler<Message>(grabData);
            //handler.SendMessege(freenode, "USER " + "sab" + " 0 * :" + "someone" + "\r\n" + "NICK " + "sab" + "\r\n");
            handler.SendMessege(freenode, "PASS 123456\r\n");
            handler.SendMessege(freenode, "NICK sabreman2\r\n");
            handler.SendMessege(freenode, "USER sabreman2 0 * :Andrey Byelogurov\r\n");
       }



        void grabData(object s, Message msg) {

            //gotta sort by tab
            if (msg.Command == "NOTICE") {
                Util.print(msg.Prefix + "\t: " + msg.Params, ConsoleColor.Yellow);
                printString(msg.Prefix + "\t: " + msg.Params);
            } else if (msg.Command == "MOTD") {
                Util.print(msg.Prefix + "\t: " + msg.Params, ConsoleColor.Cyan);
                printString(msg.Prefix + "\t: " + msg.Params);
            }

            


        }

        private void printString(string s) {
            ChatWindow.Dispatcher.BeginInvoke(new Action(delegate() {
                ChatWindow.Document.Blocks.Add(new Paragraph(new Run(s)));
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Util.FreeConsole();
        }

    }
}
