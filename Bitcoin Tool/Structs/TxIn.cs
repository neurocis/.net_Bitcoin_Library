using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BitCoin.Structs
{

    /// <summary>
    /// Transaction input or source for coins 
    /// </summary>
    /// <remarks>
    /// See also https://en.bitcoin.it/wiki/Protocol_specification#tx for more details
    /// </remarks>
	public class TxIn : ISerialize
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// The hash of the previous output transaction.
		public Hash prevOut;

        /// <summary>
        /// The index of hte previous output transaction.
        /// </summary>
		public UInt32 prevOutIndex;

        /// <summary>
        /// The length of the signature script.
        /// </summary>
		public VarInt scriptSigLen { get { return new VarInt(scriptSig.Length); } }

        /// <summary>
        /// Computational Script for confirming transaction authorization.
        /// </summary>
		public Byte[] scriptSig;

        /// <summary>
        /// Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
        /// </summary>
		public UInt32 sequenceNo;

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        protected TxIn()
		{
		}

        /// <summary>
        /// Initialize with a preloaded transaction.
        /// </summary>
        /// <param name="prevOut">The hash of the previous output transaction.</param>
        /// <param name="prevOutIndex">The index of hte previous output transaction.</param>
        /// <param name="scriptSig">Computational Script for confirming transaction authorization.</param>
        /// <param name="sequenceNo">Transaction version as defined by the sender.</param>
		public TxIn(Byte[] prevOut, UInt32 prevOutIndex, Byte[] scriptSig, UInt32 sequenceNo = 0xFFFFFFFF)
		{
			this.prevOut = prevOut;
			this.prevOutIndex = prevOutIndex;
			this.scriptSig = scriptSig;
			this.sequenceNo = sequenceNo;
		}

        /// <summary>
        /// Initialize with a preloaded transaction.
        /// </summary>
        /// <param name="byteArray">Byte array containing the transaction contents.</param>
        public TxIn(Byte[] byteArray)
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
        public void Read(Stream streamReference)
		{
            BinaryReader _binaryReader = new BinaryReader(streamReference);
            this.prevOut = _binaryReader.ReadBytes(32);
            this.prevOutIndex = _binaryReader.ReadUInt32();
            this.scriptSig = _binaryReader.ReadBytes(VarInt.FromStream(streamReference).intValue);
            this.sequenceNo = _binaryReader.ReadUInt32();
		}

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
		{
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
            _binarywriter.Write(this.prevOut, 0, 32);
            _binarywriter.Write((UInt32)this.prevOutIndex);
            this.scriptSigLen.Write(streamReference);
            _binarywriter.Write(this.scriptSig, 0, scriptSig.Length);
            _binarywriter.Write((UInt32)this.sequenceNo);
		}

        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw transaction data.</returns>
        public Byte[] ToBytes()
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
        /// <returns>A new copy of this structure containing the transaction data.</returns>
        public static TxIn FromStream(Stream streamReference)
		{
            TxIn _transaction = new TxIn();
			_transaction.Read(streamReference);
			return _transaction;
        }

        #endregion

    }
}
