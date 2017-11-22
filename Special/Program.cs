using RemotingRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Special.UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = RefData.GetResourceDataByRefIdAndResourceId("WMBasketball", "1-2", "en", string.Empty);
            Console.WriteLine(a);
            Console.ReadLine();
        }
    }
}
