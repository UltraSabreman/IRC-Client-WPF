using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IRC_Client_WPF {
    public partial class App : Application {
        public App() {
#if (DEBUG)
				Util.AllocConsole();
#endif


        }
    }
}
