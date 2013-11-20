using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;


namespace IRC_Client_WPF {
    public static class Util {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


        public static void print(string str, ConsoleColor col = ConsoleColor.Gray) {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = col;
            Console.WriteLine(str);
            Console.ForegroundColor = old;
        }

        public static long ToInt(string addr) {
            // careful of sign extension: convert to uint first;
            // unsigned NetworkToHostOrder ought to be provided.
            return (long)(uint)IPAddress.NetworkToHostOrder((int)IPAddress.Parse(addr).AddressFamily);
        }

        public static string ToAddr(long address) {
            return IPAddress.Parse(address.ToString()).ToString();
            // This also works:
            // return new IPAddress((uint) IPAddress.HostToNetworkOrder(
            //    (int) address)).ToString();
        }
    }
}
