using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitCoin.Structs
{
	public interface ISerialize
	{
		void Read(Stream s);
		void Write(Stream s);
		Byte[] ToBytes();
	}
}
