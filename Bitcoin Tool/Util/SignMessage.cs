using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoin.Crypto;
using BitCoin.DataConverters;
using BitCoin.Structs;
using System.Security.Cryptography;

namespace BitCoin.Util
{
    public class SignMessage
    {
        private SHA256Managed sha256 = new SHA256Managed();

        public string signMessage(PublicKey pubKey, String message)
        {
            
            return "";
        }
    }
}
