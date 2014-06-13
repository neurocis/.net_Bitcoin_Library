using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace BitCoin.Structs
{

    /// <summary>
    /// Bitcoin Blockchain datablock
    /// </summary>
    /// <remarks>
    /// See https://en.bitcoin.it/wiki/Protocol_specification for more detailed information
    /// </remarks>
    public class Block : ISerialize
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// Block version information, based upon the software version creating this block.
        /// </summary>
		public UInt32 version;

        /// <summary>
        /// The hash value of the previous block this particular block references.
        /// </summary>
		public Hash prev_block;

        /// <summary>
        /// The reference to a Merkle tree collection which is a hash of all transactions related to this block.
        /// </summary>
		public Hash merkle_root;
        
		/// <summary>
		/// A timestamp recording when this block was created (Will overflow in 2106).
		/// </summary>
        public UInt32 timestamp;

        /// <summary>
        /// The calculated difficulty target being used for this block.
        /// </summary>
		public UInt32 bits;

        /// <summary>
        /// The nonce used to generate this block… to allow variations of the header and compute different hashes.
        /// </summary>
		public UInt32 nonce;

        /// <summary>
        /// Number of transaction entries, this value is always 0.
        /// </summary>
		public VarInt tx_count { get { return new VarInt(transactions.Length); } }

        /// <summary>
        /// Array containing all transactions within this block
        /// </summary>
		public Transaction[] transactions = new Transaction[0];

        /// <summary>
        /// Double Hashed value of the contents of this block
        /// </summary>
		public Hash Hash
		{
			get
			{
				if (_hash == null)
				{
					SHA256 sha256 = new SHA256Managed();
					using (MemoryStream ms = new MemoryStream())
					{
						this.WriteHeader(ms);
						_hash = sha256.ComputeHash(sha256.ComputeHash(ms.ToArray())).ToArray();
					}
				}
				return _hash;
			}
		}

        #endregion "Public Properties"

        #region "Private Properties"

        /// <summary>
        /// Double Hashed value of the contents of this block
        /// </summary>
		private Hash _hash = null;

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        protected Block()
		{
		}

        /// <summary>
        /// Initialize with a preloaded blockfile.
        /// </summary>
        /// <param name="byteArray">Byte array containing the block contents.</param>
        public Block(Byte[] byteArray)
		{
            using (MemoryStream _memorystream = new MemoryStream(byteArray))
                this.Read(_memorystream);
		}

        #endregion

        /* ********************************************************************************
         * Functions
         * ****************************************************************************** */
        #region "Functions"

        /// <summary>
        /// Load the contents of a stream into this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public virtual void Read(Stream streamReference)
		{
            BinaryReader _binaryReader = new BinaryReader(streamReference);

            version = _binaryReader.ReadUInt32();
            prev_block = _binaryReader.ReadBytes(32);
            merkle_root = _binaryReader.ReadBytes(32);
            timestamp = _binaryReader.ReadUInt32();
            bits = _binaryReader.ReadUInt32();
            nonce = _binaryReader.ReadUInt32();
			transactions = new Transaction[VarInt.FromStream(streamReference)];
			for (int i = 0; i < transactions.Length; i++)
				transactions[i] = Transaction.FromStream(streamReference);
		}

        /// <summary>
        /// (Re)write the header contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        private void WriteHeader(Stream streamReference)
		{
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);

			_binarywriter.Write((UInt32)version);
			_binarywriter.Write(prev_block, 0, 32);
			_binarywriter.Write(merkle_root, 0, 32);
			_binarywriter.Write((UInt32)timestamp);
			_binarywriter.Write((UInt32)bits);
			_binarywriter.Write((UInt32)nonce);
		}

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public virtual void Write(Stream streamReference)
		{
            this.WriteHeader(streamReference);
            tx_count.Write(streamReference);

			for (int i = 0; i < transactions.Length; i++)
                transactions[i].Write(streamReference);
		}

        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw blockfile data.</returns>
        public virtual Byte[] ToBytes()
        {
            using (MemoryStream _memorystream = new MemoryStream())
            {
                this.Write(_memorystream);
                return _memorystream.ToArray();
            }
        }

        /// <summary>
        /// Read the contents of a (predefined) stream into a new copy of this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        /// <returns>A new copy of this structure containing the blockdata.</returns>
        public static Block FromStream(Stream streamReference)
		{
			Block _block = new Block();
            _block.Read(streamReference);
			return _block;
        }

        #endregion

    }
}
