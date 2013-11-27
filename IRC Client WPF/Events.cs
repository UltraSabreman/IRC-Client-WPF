using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IRC_Client_WPF {
    public class ServerCreatedEvent : EventArgs {
        public readonly IRC_Client_WPF.Server server;
        public ServerCreatedEvent(IRC_Client_WPF.Server s) { server = s; }
    }
    public class ChannelCreatedEvent : EventArgs {
        public readonly IRC_Client_WPF.Channel channel;
        public ChannelCreatedEvent(IRC_Client_WPF.Channel c) { channel = c; }
    }

    public class ChannelUpdate : EventArgs {
        public readonly IRC_Client_WPF.Channel channel;
        public ChannelUpdate(IRC_Client_WPF.Channel c) { channel = c; }
    }
}
