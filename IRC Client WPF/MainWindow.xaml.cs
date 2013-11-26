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
        List<Server> serverList = new List<Server>();


        public MainWindow() {
            InitializeComponent();
            newServer("Freenode", "irc.freenode.net", 6667);
            ServerList.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(changeChannel);
            //ServerList.Items = new ItemCollection();
        }

        public void update(Channel c) {
            //TODO: add colors and other stuff based on what's on the line?
            // (mybie make it a tuple like we had it before?);
            ChatBox.Dispatcher.BeginInvoke(new Action(delegate() {
                if (getSelectedChannel() != c) {

                } else {
                    ChatBox.Document.Blocks.Clear();

                    foreach (string s in c.buffer) {
                        Paragraph paragraph = new Paragraph(new Run(s));
                        ChatBox.Document.Blocks.Add(paragraph);
                    }
                }
          
                //TODO: make this only fire if user is scrolled to the bottom already.
                ChatBox.ScrollToEnd();
            }));
        }

        public void changeChannel(object o, RoutedPropertyChangedEventArgs<Object> e) {
            ChatBox.Document.Blocks.Clear();


            Channel c = getSelectedChannel();
            foreach (string s in c.buffer) {
                Paragraph paragraph = new Paragraph(new Run(s));
                ChatBox.Document.Blocks.Add(paragraph);
            }
        }

        
        public void createdChannel(Channel c) {
            c.OnUpdate += new Channel.update(update);

            foreach (ObjectTreeViewItem t in ServerList.Items)
                t.Dispatcher.BeginInvoke(new Action(delegate() {
                    if ((t.TheObject as Server) == c.server) {
                        //dont re-add the channel if it already exhists.
                        foreach (ObjectTreeViewItem l in t.Items)
                            if ((l.TheObject as Channel).channelName == c.channelName)
                                return;

                        ObjectTreeViewItem temp = new ObjectTreeViewItem();
                        temp.Header = c.channelName;
                        temp.TheObject = c;

                        t.Items.Add(temp);
                        
                        return;
                    }
                }));
        }


        public void newServer(string Name, string Adress, int port) {
            Server temp = new Server(Name, Adress, port, this);
            temp.OnChannelCreation += new Server.chanCreated(createdChannel);

            serverList.Add(temp);
            ObjectTreeViewItem newTV = new ObjectTreeViewItem();
                newTV.Header = Name;
                newTV.TheObject = temp;

            ServerList.Items.Add(newTV);            
        }

        public Channel getSelectedChannel() {
            ObjectTreeViewItem selected = ServerList.SelectedItem as ObjectTreeViewItem;
            if (selected == null) return null;

            if (selected.TheObject.GetType() == typeof(Server))
                return (selected.TheObject as Server).channels.First();
            else 
                return (selected.TheObject as Channel);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            foreach (Server s in serverList)
                s.disconnect();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Channel cur = getSelectedChannel();
                cur.server.parseOutgoing(cur.channelName, new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);

                InputBox.Document.Blocks.Clear();
            }
        }

    }
}
