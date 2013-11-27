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
// fill the nicklist and UIChatBox with stuff from the current server-channel combo.
// (i am brilliant!.... at avoiding stupid mistakes).
namespace IRC_Client_WPF {
    public partial class MainWindow : Window {
        List<Server> serverList = new List<Server>();


        public MainWindow() {
            InitializeComponent();
            newServer("Freenode", "irc.freenode.net", 6667);
            UIServerList.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(changeChannel);
            //UIServerList.Items = new ItemCollection();
        }

        public void update(Channel c) {
            //TODO: add colors and other stuff based on what's on the line?
            // (mybie make it a tuple like we had it before?);
            UIChatBox.Dispatcher.BeginInvoke(new Action(delegate() {
                if (getSelectedChannel() != c) {

                } else {
                    UIChatBox.Document.Blocks.Clear();

                    foreach (string s in c.buffer) {
                        Paragraph paragraph = new Paragraph(new Run(s));
                        UIChatBox.Document.Blocks.Add(paragraph);
                    }
                }
          
                //TODO: make this only fire if user is scrolled to the bottom already.
                UIChatBox.ScrollToEnd();
            }));
        }

        public void changeChannel(object o, RoutedPropertyChangedEventArgs<Object> e) {
            UIChatBox.Document.Blocks.Clear();


            Channel c = getSelectedChannel();
            foreach (string s in c.buffer) {
                Paragraph paragraph = new Paragraph(new Run(s));
                UIChatBox.Document.Blocks.Add(paragraph);
            }
        }

        
        public void createdChannel(Channel c) {
            c.OnUpdate += new Channel.update(update);

            /*c.Dispatcher.BeginInvoke(new Action(delegate() {
            //foreach (Server s in UIServerList.Items)
             //       if (s == c.server) {
                        //dont re-add the channel if it already exhists.
                        //if (s.Items.Contains(c))
                        //   return;
                
                        Server s = 
                        c.server.Items.Add(c);
              //      }
                }));*/
            Console.Beep();
        }


        public void newServer(string Name, string Adress, int port) {
            Server temp = new Server(Name, Adress, port, this);
            temp.OnChannelCreation += new Server.chanCreated(createdChannel);

            serverList.Add(temp);
            UIServerList.Items.Add(temp);            
        }

        public Channel getSelectedChannel() {
            object selected = UIServerList.SelectedItem;
            if (selected == null) return null;

            if (selected.GetType() == typeof(Server))
                return (selected as Server).serverChannel;
            else 
                return (selected as Channel);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            foreach (Server s in serverList)
                s.disconnect();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Channel cur = getSelectedChannel();
                cur.server.parseOutgoing(cur.channelName, new TextRange(UIInputBox.Document.ContentStart, UIInputBox.Document.ContentEnd).Text);

                UIInputBox.Document.Blocks.Clear();
            }
        }

    }
}
