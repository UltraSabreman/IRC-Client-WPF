using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
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
	class DataReader {
		public List<ServerInfo> Servers = new List<ServerInfo>();
		public IRCOptions Options = new IRCOptions();

		private string filePath = "data.json";

		public DataReader(string path = "") {
			if (path == "" || path == null) return;
			if (!File.Exists(path))
				throw (new System.IO.FileNotFoundException());
			else
				filePath = path;
		}

		public void deserialize() {
			if (File.Exists(filePath)) {
				StreamReader fs = File.OpenText(filePath);
				bool success = false;
				while (!success) {
					try {
						JsonConvert.PopulateObject(fs.ReadToEnd(), this); //#yolo
						success = true;
					} catch {
						if (MessageBox.Show("Can't read data file. Hit ok to retry, \n or cancel to abort and re-generate file.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Cancel) {
							fs.Close();
							StreamWriter rewrite = new StreamWriter(filePath);
							rewrite.Write("{}");
							rewrite.Close();
							return;
						}
					}
				}

				fs.Close();
			}
		}

		public void serialize() {
			if (!File.Exists(filePath)) File.CreateText(filePath);

			StreamWriter fs = null;
			while (fs == null) {
				try {
					fs = new StreamWriter(filePath);
					fs.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
				} catch (System.IO.IOException) {
					if (MessageBox.Show("File is in use", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Cancel) {
						fs.Close();
						return;
					}
				}
			}

			fs.Close();
		}
	}
}

