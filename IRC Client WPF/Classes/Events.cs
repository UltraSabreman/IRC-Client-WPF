using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IRC_Client_WPF {
    public class ServerCreatedEvent : EventArgs {
		public ServerCreatedEvent() { }
    }
    public class ChannelCreatedEvent : EventArgs {
		public readonly Channel Channel;
		public ChannelCreatedEvent(Channel c) { this.Channel = c; }
    }
	public class ChannelUpdate : EventArgs {
		public readonly Channel Channel;
		public ChannelUpdate(Channel c) { this.Channel = c; }
	}

	public class LineAdded : EventArgs {
		public readonly ChatLine Line;
		public LineAdded(ChatLine l) { Line = l; }
	}
	
    public class ReciveMessage : EventArgs {
		public readonly string Msg;
		public ReciveMessage(string s) { Msg = s; }
    }
	public class SendMessage : EventArgs {
		public readonly string Msg;
		public readonly bool HandleNow;
		public SendMessage(string s, bool h = false) { Msg = s; HandleNow = h; }
	}
}
