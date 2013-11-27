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
        Server MainBuffer;
        public MainWindow() {
            InitializeComponent();
            MainBuffer = new Server("Chat Client", "", 0, this);
            MainBuffer.OnChannelCreation += new EventHandler<ChannelCreatedEvent>(channelCreated);
            MainBuffer.serverChannel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);
            UIServerList.Items.Add(MainBuffer);

            createServer("Freenode", "irc.freenode.net", 6667);
            UIServerList.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(changeChannel);

        }

        public void channelUpdated(object o, ChannelUpdate e) {
            //TODO: add colors and other stuff based on what's on the line?
            // (mybie make it a tuple like we had it before?);
            if (getSelectedChannel() == e.channel) {

                foreach (string s in e.channel.newBuffer) {
                    Paragraph paragraph = new Paragraph(new Run(s));
                    UIChatBox.Document.Blocks.Add(paragraph);
                }
                e.channel.sync();
            }
          
            //TODO: make this only fire if user is scrolled to the bottom already.
            UIChatBox.ScrollToEnd();
        }

        public void changeChannel(object o, RoutedPropertyChangedEventArgs<Object> e) {
            UIChatBox.Document.Blocks.Clear();


            Channel c = getSelectedChannel();
            c.sync();
            foreach (string s in c.buffer) {
                Paragraph paragraph = new Paragraph(new Run(s));
                UIChatBox.Document.Blocks.Add(paragraph);
            }

            UIChatBox.ScrollToEnd();
        }

        
        public void channelCreated(object o, ChannelCreatedEvent e) {
            e.channel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);
        }


        public void createServer(string Name, string Adress, int port) {
            Server temp = new Server(Name, Adress, port, this);
            temp.OnChannelCreation += new EventHandler<ChannelCreatedEvent>(channelCreated);
            temp.serverChannel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);

            UIServerList.Items.Add(temp);            
        }

        private Channel getSelectedChannel() {
            object selected = UIServerList.SelectedItem;
            if (selected == null) return null;

            if (selected.GetType() == typeof(Server))
                return (selected as Server).serverChannel;
            else 
                return (selected as Channel);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            foreach (Server s in UIServerList.Items)
                s.disconnect();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Channel cur = getSelectedChannel();

                if (cur != null)
                    cur.parseOutgoing(new TextRange(UIInputBox.Document.ContentStart, UIInputBox.Document.ContentEnd).Text);
                else
                    MainBuffer.serverChannel.parseOutgoing(new TextRange(UIInputBox.Document.ContentStart, UIInputBox.Document.ContentEnd).Text);

                UIInputBox.Document.Blocks.Clear();
            }
        }

    }
}
