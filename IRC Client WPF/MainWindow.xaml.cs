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
        public delegate void stringDel(string s);
        public stringDel SubmitText;

        public MainWindow() {
            InitializeComponent();
            //TODO: fix coupling
            ((App)Application.Current).registerWindow(this);
            ChatBox.Document.Blocks.Clear();

            TabItem test = Util.TrycloneElement<TabItem>(this.Freenode);
            test.Name = "rita";
            test.Header = "rita";
            Tabs.Items.Add(test);

        }

        public void Write(chatLine line) {
            ChatBox.Dispatcher.BeginInvoke(new Action(delegate() {
                Paragraph paragraph = new Paragraph();

                //TODO: Figure out what tab, server, and channel this messege goes to.
                foreach (Tuple<string, SolidColorBrush> t in line.Line) {
                    Run temp = new Run(t.Item1);
                    if (t.Item2 == null)
                        temp.Foreground = Brushes.Black;
                    else
                        temp.Foreground = t.Item2;

                    paragraph.Inlines.Add(temp);
                }

                ChatBox.Document.Blocks.Add(paragraph);
                //TODO: make this only fire if user is scrolled to the bottom already.
                ChatBox.ScrollToEnd();
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //TODO: fix coupling
            ((App)Application.Current).exit();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) {
            if (SubmitText != null)
                SubmitText(new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);

            InputBox.Document.Blocks.Clear();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (SubmitText != null)
                    SubmitText(new TextRange(InputBox.Document.ContentStart, InputBox.Document.ContentEnd).Text);

                InputBox.Document.Blocks.Clear();
            }
        }

    }
}
