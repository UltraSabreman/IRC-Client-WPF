using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
//TOOD: proper public/provate shiz.
namespace IRC_Client_WPF {
    public class Channel {
        public List<string> nicks = new List<string>();
        public List<string> buffer = new List<string>(); //make this a tuple to perserve formatting OR write a function in the UI to parse the formatting out.
        public Server server;
        
        public string channelName;

        ////////////////////////////////
        public delegate void update(Channel c);
        public event update OnUpdate;



        public Channel(Server s, string name) {
            server = s;
            channelName = name;
        }

        //will this work, who knows?
        public static Channel operator +(Channel c, string s) {
            c.addLine(s);
            return c;
        }

        public void addLine(string s) {
            buffer.Add(s);
            if (OnUpdate != null)
                OnUpdate(this);
        }

       
    }
}
