using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitCoin.Structs.Other
{
	public class UnspentTxOutList : ISerialize, IEnumerable<KeyValuePair<OutPoint, TxOut>>
	{
		Dictionary<OutPoint, TxOut> utxo = new Dictionary<OutPoint, TxOut>();
		List<KeyValuePair<OutPoint, TxOut>> duptxout = new List<KeyValuePair<OutPoint, TxOut>>();

		public UnspentTxOutList()
		{
		}

		public UnspentTxOutList(Byte[] b)
		{
			using (MemoryStream ms = new MemoryStream(b))
				Read(ms);
		}

		public void Add(OutPoint h, TxOut txo)
		{
			try
			{
				utxo.Add(h, txo);
			}
			catch (ArgumentException)
			{
				// Duplicate!
				duptxout.Add(new KeyValuePair<OutPoint, TxOut>(h, txo));
			}
		}

		public bool TryRemove(OutPoint h)
		{
			if (utxo.Remove(h))
				return true;
			// Not found.. in dup?
			int i = duptxout.FindIndex(x => x.Key.Equals(h));
			if (i >= 0)
			{
				duptxout.RemoveAt(i);
				return true;
			}
			// Still not found...
			return false;
		}

		public void Read(Stream s)
		{
			utxo.Clear();
			duptxout.Clear();
			BinaryReader br = new BinaryReader(s);
			UInt64 count = br.ReadUInt64();
			for (UInt64 i = 0; i < count; i++)
			{
				OutPoint h = OutPoint.FromStream(s);
				TxOut t = TxOut.FromStream(s);
				utxo.Add(h, t);
			}
			UInt64 dupcount = br.ReadUInt64();
			for (UInt64 i = 0; i < dupcount; i++)
			{
				OutPoint h = OutPoint.FromStream(s);
				TxOut t = TxOut.FromStream(s);
				duptxout.Add(new KeyValuePair<OutPoint, TxOut>(h, t));
			}
		}

		public void Write(Stream s)
		{
			// Attempt cleanup
			for (int i = 0; i < duptxout.Count; i++)
			{
				try
				{
					utxo.Add(duptxout[i].Key, duptxout[i].Value);
					duptxout.RemoveAt(i);
					i = -1;
				}
				catch (ArgumentException) { }
			}

			BinaryWriter bw = new BinaryWriter(s);
			bw.Write((UInt64)utxo.Count);
			foreach (KeyValuePair<OutPoint, TxOut> h in utxo)
			{
				h.Key.Write(s);
				h.Value.Write(s);
			}
			bw.Write((UInt64)duptxout.Count);
			foreach (KeyValuePair<OutPoint, TxOut> h in duptxout)
			{
				h.Key.Write(s);
				h.Value.Write(s);
			}
		}

		public Byte[] ToBytes()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				Write(ms);
				return ms.ToArray();
			}
		}

		public static UnspentTxOutList FromStream(Stream s)
		{
			UnspentTxOutList x = new UnspentTxOutList();
			x.Read(s);
			return x;
		}

		public IEnumerator<KeyValuePair<OutPoint, TxOut>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<OutPoint, TxOut>> e;
			e = utxo.GetEnumerator();
			while (e.MoveNext())
				yield return e.Current;
			e = duptxout.GetEnumerator();
			while (e.MoveNext())
				yield return e.Current;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerator<KeyValuePair<OutPoint, TxOut>> e;
			e = utxo.GetEnumerator();
			while (e.MoveNext())
				yield return e.Current;
			e = duptxout.GetEnumerator();
			while (e.MoveNext())
				yield return e.Current;	
		}
	}
}
