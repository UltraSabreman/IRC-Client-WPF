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
using System.Text.RegularExpressions;

namespace IRC_Client_WPF {
    public partial class MainWindow : Window {
        //Server MainBuffer;
		DataReader reader = new DataReader();

        public MainWindow() {
            InitializeComponent();

			reader.deserialize();
			foreach (ServerInfo si in reader.Servers) {
				createServer(si);
			}

            UIServerList.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(changeChannel);
        }

        public void channelUpdated(object o, ChannelUpdate e) {
			//TODO: make me efficent

			bool shouldScroll = UIChatBox.VerticalOffset + UIChatBox.ViewportHeight >= UIChatBox.ExtentHeight;

            if (getSelectedChannel() == e.channel) {
				UINickList.Items.Clear();
                foreach (string s in e.channel.nicks)
                    UINickList.Items.Add(s);

				UIChatBox.Document = e.channel.buffer;
                e.channel.sync();
            }

			if (shouldScroll) UIChatBox.ScrollToEnd();
        }

        public void changeChannel(object o, RoutedPropertyChangedEventArgs<Object> e) {
			//UIChatBox.Document.Blocks.Clear();
			UINickList.Items.Clear();

			Channel c = getSelectedChannel();
			if (c == null) return;
			foreach (string s in c.nicks)
				UINickList.Items.Add(s);

			UIChatBox.Document = c.buffer;
			c.sync();

			UIChatBox.ScrollToEnd();
        }

        public void channelCreated(object o, ChannelCreatedEvent e) {
			//UIServerList.SelectedValuePath = e.channel.DisplayMemberPath;
			//Util.SetSelectedItem(ref UIServerList, e.channel);
            e.channel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);
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
			reader.Servers.Clear();
			foreach (Server s in UIServerList.Items) {
				reader.Servers.Add(s.disconnect());
			}

			//TODO: dump + serialise options.
			reader.serialize();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Channel cur = getSelectedChannel();

                if (cur != null)
                    cur.parseOutgoing(new TextRange(UIInputBox.Document.ContentStart, UIInputBox.Document.ContentEnd).Text);
				//TODO: redirect intput to channel that textbox is attached to?

                UIInputBox.Document.Blocks.Clear();
            }
        }

		private void UIAddServer_Click(object sender, RoutedEventArgs e) {
			createServer(UIServerName.Text);
			UIServerName.Text = "";
		}

		private void UIServerName_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				createServer(UIServerName.Text);
				UIServerName.Text = "";
			}
		}

		private void createServer(ServerInfo si) {
			try {
				Server temp = new Server(si, this);
				temp.OnChannelCreation += new EventHandler<ChannelCreatedEvent>(channelCreated);
				temp.serverChannel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);

				UIServerList.Items.Add(temp);
				temp.IsSelected = true;
			} catch (System.Net.Sockets.SocketException) {
				MessageBox.Show("Error: Invalid server adress.", "Invalid Server Adress", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void createServer(string name) {
			if (!String.IsNullOrEmpty(name)) {
				string servername = name;

				int port = 6667;
				string address = "";
				string [] stuff = servername.Split(":".ToCharArray());

				if (stuff.Length == 2)
					port = int.Parse(stuff [1]);
				address = stuff [0];

				createServer(address, address, port);
			}
		}
		public void createServer(string Name, string Adress, int port) {
			try {
				Server temp = new Server(Name, Adress, port, this);
				temp.OnChannelCreation += new EventHandler<ChannelCreatedEvent>(channelCreated);
				temp.serverChannel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);

				UIServerList.Items.Add(temp);
				temp.IsSelected = true;

			} catch (System.Net.Sockets.SocketException) {
				MessageBox.Show("Error: Invalid server adress.", "Invalid Server Adress", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
    }
}
