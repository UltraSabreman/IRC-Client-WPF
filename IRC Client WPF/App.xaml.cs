using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IRC_Client_WPF {
    public partial class App : Application {
        Connection freenode;
        ConnectionHandler NetworkEngine;
        CommandHandler CommandEngine;
        MainWindow wind;
        public App() {
            CommandEngine = new CommandHandler();
            NetworkEngine = new ConnectionHandler();
            wind = (MainWindow)MainWindow;

            freenode = NetworkEngine.Connect("irc.freenode.net", 6667);
            NetworkEngine.onMessage += new EventHandler<Message>(grabData);
            NetworkEngine.SendMessege(freenode, "PASS 123456\r\n");
            NetworkEngine.SendMessege(freenode, "NICK sabreman2\r\n");
            NetworkEngine.SendMessege(freenode, "USER sabreman2 0 * :Andrey Byelogurov\r\n");
        }

        public void registerWindow(MainWindow w) {
            wind = w;
        }

        //TODO: define my own colors to reduce copling.
        private void grabData(object s, Message msg) {
            if (wind != null) {
                wind.Dispatcher.BeginInvoke(new Action(delegate() {
                    wind.Write(msg.Prefix + msg.Command + msg.Params + msg.Trail + "", Color.FromRgb((byte)0, (byte)0, (byte)0));
                }));
            }
        }

        public void lol(string s) {
            NetworkEngine.SendMessege(freenode, s);
        }

        public void Exit() {
            NetworkEngine.Disconnect(freenode);

        }

    }
}
