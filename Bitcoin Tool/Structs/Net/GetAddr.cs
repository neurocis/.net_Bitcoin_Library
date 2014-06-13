using System.IO;

namespace BitCoin.Structs.Net
{
	public class GetAddr : EmptyPayload, IPayload
	{
		public static GetAddr FromStream (Stream s) {
			return new GetAddr();
		}
	}
}
