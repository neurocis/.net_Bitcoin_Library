using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitCoin.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputeUnspentTxOuts.Main(args);
            ComputeAddressBalances.Main(args);

        }
    }
}
