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

namespace IRC_Client_WPF {
	/// <summary>
	/// Interaction logic for AutoScrollTextBox.xaml
	/// </summary>
	/*public partial class AutoScrollTextBox : UserControl {
		public AutoScrollTextBox() {
			InitializeComponent();
		}
	}*/
	
	//Figure this out pls

	//From http://stackoverflow.com/questions/13456441/wpf-richtextboxs-conditionally-scroll
	public partial class AutoScrollTextBox : RichTextBox {
		public static bool GetIsAutoScroll(DependencyObject obj) {
			return (bool)obj.GetValue(IsAutoScrollProperty);
		}

		public static void SetIsAutoScroll(DependencyObject obj, bool value) {
			obj.SetValue(IsAutoScrollProperty, value);
		}

		public static readonly DependencyProperty IsAutoScrollProperty =
			DependencyProperty.RegisterAttached("IsAutoScroll", typeof(bool), typeof(TestBox), new PropertyMetadata(false, new PropertyChangedCallback((s, e) => {
				RichTextBox richTextBox = s as RichTextBox;
				if (richTextBox != null) {
					if ((bool)e.NewValue)
						richTextBox.TextChanged += richTextBox_TextChanged;
					else if ((bool)e.OldValue)
						richTextBox.TextChanged -= richTextBox_TextChanged;

				}
			})));

		static void richTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			RichTextBox richTextBox = sender as RichTextBox;
			if ((richTextBox.VerticalOffset + richTextBox.ViewportHeight) == richTextBox.ExtentHeight || richTextBox.ExtentHeight < richTextBox.ViewportHeight)
				richTextBox.ScrollToEnd();
		}
	}
}
