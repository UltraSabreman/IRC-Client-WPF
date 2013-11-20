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
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ((App)Application.Current).registerWindow(this);
            ChatBox.Document.Blocks.Clear();
        }
        public void Write(string messege, Color c) {
            ChatBox.Dispatcher.BeginInvoke(new Action(delegate() {
                Paragraph paragraph = new Paragraph(new Run(messege));

                ChatBox.Document.Blocks.Add(paragraph);
                ChatBox.ScrollToEnd();
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            ((App)Application.Current).Exit();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) {
            ((App)Application.Current).lol(new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);
            InputBox.Document.Blocks.Clear();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ((App)Application.Current).lol(new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);
                InputBox.Document.Blocks.Clear();

            }
            
        }

    }
}
