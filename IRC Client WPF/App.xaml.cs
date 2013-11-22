using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

/* YOu jsut have to redesing this.
 * 
 * 1) build the fullish ui (at least so you know where the servers will go and stuff)
 * 2) Desing a system that will do stuff around that ui
 * 3) code
 */

namespace IRC_Client_WPF {
    public partial class App : Application {
        Connection freenode;
        Connection rita;
        ConnectionHandler NetworkEngine;
        CommandHandler CommandEngine;
        MainWindow wind;
        public App() {
            CommandEngine = new CommandHandler();
            CommandEngine.OnResponce += new CommandHandler.thing(responce);
            NetworkEngine = new ConnectionHandler();
            wind = (MainWindow)MainWindow;


            freenode = NetworkEngine.Connect("irc.freenode.net", 6667);
            NetworkEngine.onMessage += new EventHandler<Message>(reciveString);
            NetworkEngine.SendMessege(freenode, "PASS 123456\r\n");
            NetworkEngine.SendMessege(freenode, "NICK sabreman2\r\n");
            NetworkEngine.SendMessege(freenode, "USER sabreman2 0 * :Andrey B\r\n");

           // rita = 

        }

        public void registerWindow(MainWindow w) {
            wind = w;
            wind.SubmitText += new MainWindow.stringDel(sendString);
        }

        private void reciveString(object s, Message msg) {
            if (wind != null) {
                chatLine newLine = CommandEngine.Decode(msg);

                if (newLine != null) {
                    wind.Dispatcher.BeginInvoke(new Action(delegate() {
                        wind.Write(newLine);
                    }));
                }
            }
        }

        public void sendString(string s) {
            string newLine = CommandEngine.Encode(s);
            NetworkEngine.SendMessege(freenode, newLine);
        }

        public void responce(string s) {
            NetworkEngine.SendMessege(freenode, s);
        }
        public void exit() {
            NetworkEngine.Disconnect(freenode);

        }

    }
}
