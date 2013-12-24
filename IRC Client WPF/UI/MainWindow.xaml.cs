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
			bool shouldScroll = UIChatBox.VerticalOffset + UIChatBox.ViewportHeight >= UIChatBox.ExtentHeight;

            if (getSelectedChannel() == e.channel) {
				UINickList.Items.Clear();
                foreach (string s in e.channel.nicks)
                    UINickList.Items.Add(s);

				UIChatBox.Document = e.channel.buffer;
                e.channel.sync();
            }

			if (shouldScroll) UIChatBox.ScrollToEnd();
			UIChanTopic.Text = e.channel.topic;
        }

        public void changeChannel(object o, RoutedPropertyChangedEventArgs<Object> e) {
			UINickList.Items.Clear();

			UIChannel c = getSelectedChannel();
			if (c == null) return;
			foreach (string s in c.nicks)
				UINickList.Items.Add(s);

			UIChatBox.Document = c.buffer;
			c.sync();

			UIChatBox.ScrollToEnd();
			UIChanTopic.Text = c.topic;

			//if (!c.isAdmin)
			//	UIChanTopic.IsReadOnly = true;

        }

        public void channelCreated(object o, ChannelCreatedEvent e) {
            e.channel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);
        }

        public UIChannel getSelectedChannel() {
            object selected = UIServerList.SelectedItem;
            if (selected == null) return null;

            if (selected.GetType() == typeof(UIServer))
                return (selected as UIServer).serverChannel;
            else 
                return (selected as UIChannel);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			reader.Servers.Clear();
			foreach (UIServer s in UIServerList.Items) {
				reader.Servers.Add(s.disconnect());
			}

			//TODO: dump + serialise options.
			reader.serialize();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                UIChannel cur = getSelectedChannel();

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
				UIServer temp = new UIServer(si, this);
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
				UIServer temp = new UIServer(Name, Adress, port, this);
				temp.OnChannelCreation += new EventHandler<ChannelCreatedEvent>(channelCreated);
				temp.serverChannel.OnUpdate += new EventHandler<ChannelUpdate>(channelUpdated);

				UIServerList.Items.Add(temp);
				temp.IsSelected = true;

			} catch (System.Net.Sockets.SocketException) {
				MessageBox.Show("Error: Invalid server adress.", "Invalid Server Adress", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void UIChanTopic_KeyUp(object sender, KeyEventArgs e) {
			UIChannel c = getSelectedChannel();
			if (c == null) return;

			if (e.Key == Key.Enter) {
				c.parseOutgoing("/topic " + UIChanTopic.Text);
			} 
		}
    }
}
