using System;
using Fanex.RemotingRef;

namespace Special.UnitTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = new RemotingRefService().GetResourceDataByRefIdAndResourceId("WMBasketball", "1-2", "en", string.Empty);
            Console.WriteLine(a);
            Console.ReadLine();
        }
    }
}