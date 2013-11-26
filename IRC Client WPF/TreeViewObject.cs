using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace IRC_Client_WPF {
    public class ObjectTreeViewItem : TreeViewItem {
        #region Data Member
        
        Object theObject;

        #endregion

        #region Properties

        public Object TheObject {
            get { return theObject; }
            set {
                theObject = value;
            }
        }
        #endregion
    }
}
