﻿using System;
using System.IO;
using BitCoin.Structs;
using BitCoin.Structs.Other;
using BitCoin.Util;

// Will only work if blockchain contains no orphan blocks.

namespace BitCoin.Examples
{
	class ComputeUnspentTxOuts
	{
		public static void Main(string[] args)
		{
			string outFile = @"C:\utxo.dat";

            UnspentTxOutList utxo = new UnspentTxOutList();

			Hash lastBlockHash = null;

			if (File.Exists(outFile))
			{
				using (FileStream fs = new FileStream(outFile, FileMode.Open))
				{
					BinaryReader br = new BinaryReader(fs);
					lastBlockHash = br.ReadBytes(32);
					utxo = UnspentTxOutList.FromStream(fs);
				}
			}

            BlockFileReader blockDb = new BlockFileReader("blkindex.dat", "C:\\Temp\\PIG");
            //BlockFileReader blockDb = new BlockFileReader("blkindex.dat", "C:\\Users\\Neurocis\\AppData\\Roaming\\PiggyCoin");
            

			Block_Disk b;
			Block lastBlock = null;
			while ((b = blockDb.getNext()) != null)
			{
				lastBlock = b;
				if (lastBlockHash != null && !b.prev_block.Equals(lastBlockHash))
					continue;
				lastBlockHash = null;

				foreach (Transaction tx in b.transactions)
				{
					foreach (TxIn txin in tx.inputs)
					{
						if (txin.prevOutIndex == 0xFFFFFFFF) //coinbase
							continue;
						if (!utxo.TryRemove(new OutPoint(txin.prevOut, txin.prevOutIndex)))
						{
							Console.WriteLine("Error: Invalid TxIn! Blockchain likely contains orphans.");
							return;
						}
					}
					for (uint i = 0; i < tx.outputs.Length; i++)
						utxo.Add(new OutPoint(tx.Hash, i), tx.outputs[i]);
				}
			}

			using (FileStream fs = new FileStream(outFile, FileMode.Create))
			{
				BinaryWriter bw = new BinaryWriter(fs);
				bw.Write(lastBlock.Hash, 0, 32);
				utxo.Write(fs);
			}
		}
	}
}
