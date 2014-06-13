using System.IO;

namespace BitCoin.Structs.Net
{
	public class Ping : EmptyPayload, IPayload
	{
		public static Ping FromStream(Stream s)
		{
			return new Ping();
		}
	}
}