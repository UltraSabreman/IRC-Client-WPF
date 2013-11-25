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

//TODO: DO NOT recareate any gui elemnts for each sderver/channel. Simply
// fill the nicklist and chatbox with stuff from the current server-channel combo.
// (i am brilliant!.... at avoiding stupid mistakes).
namespace IRC_Client_WPF {
    public partial class MainWindow : Window {
        Server freenode;

        public MainWindow() {
            freenode = new Server("Freenode", "irc.freenode.net", 6667);
            freenode.OnChannelCreation += new Server.chanCreated(createdChannel);
        }

        public void update(Channel c) {
            ChatBox.Dispatcher.BeginInvoke(new Action(delegate() {
                //TODO: check to see if we're int eh right channel to display this.
                // other wise just update the things on the left w/ appropriate info.
                //TODO: add colors and other stuff based on what's on the line?
                // (mybie make it a tuple like we had it before?);

                ChatBox.Document.Blocks.Clear();

                foreach (string s in c.buffer) {
                    Paragraph paragraph = new Paragraph(new Run(s));
                    ChatBox.Document.Blocks.Add(paragraph);                
                }

                //TODO: make this only fire if user is scrolled to the bottom already.
                ChatBox.ScrollToEnd();
            }));
        }

        public void createdChannel(Channel c) {
            c.OnUpdate += new Channel.update(update);

        }






        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //TODO: fix coupling
            freenode.disconnect();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                //TODO: get curent channel.

                freenode.parseOutgoing("#test",new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);

                InputBox.Document.Blocks.Clear();
            }
        }

    }
}
