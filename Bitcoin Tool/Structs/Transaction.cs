using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace BitCoin.Structs
{

    /// <summary>
    ///  Structure of a bitcoin transaction.
    /// </summary>
    /// <remarks>
    /// See https://en.bitcoin.it/wiki/Protocol_specification#tx for more details
    /// </remarks>
	public class Transaction : ISerialize
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// Transaction data format version.
        /// </summary>
		public UInt32 version;

        /// <summary>
        /// Number of Transaction inputs 
        /// </summary>
		public VarInt numInputs { get { return new VarInt(inputs.Length); } }

        /// <summary>
        /// A list of 1 or more transaction inputs or sources for coins.
        /// </summary>
		public TxIn[] inputs;

        /// <summary>
        /// Number of Transaction outputs 
        /// </summary>
		public VarInt numOutputs { get { return new VarInt(outputs.Length); } }

        /// <summary>
        /// A list of 1 or more transaction outputs or destinations for coins.
        /// </summary>
		public TxOut[] outputs;

        /// <summary>
        /// The block number or timestamp at which this transaction is locked.
        /// </summary>
		public UInt32 lock_time;

        /// <summary>
        /// Double hashed value of this structure
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
						this.Write(ms);
						_hash = sha256.ComputeHash(sha256.ComputeHash(ms.ToArray())).ToArray();
					}
				}
				return _hash;
			}
		}

        #endregion "Public Properties"

        #region "Private Properties"

        /// <summary>
        /// Double hashed value of this structure
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
        protected Transaction()
		{
		}

        /// <summary>
        /// Initialize with a transaction with predefined values.
        /// </summary>
        /// <param name="version">Transaction data format version </param>
        /// <param name="inputs">A list of 1 or more transaction inputs or sources for coins</param>
        /// <param name="outputs">A list of 1 or more transaction outputs or destinations for coins</param>
        /// <param name="lock_time">The block number or timestamp at which this transaction is locked</param>
		public Transaction(UInt32 version, TxIn[] inputs, TxOut[] outputs, UInt32 lock_time)
		{
			this.version = version;
			this.inputs = inputs;
			this.outputs = outputs;
			this.lock_time = lock_time;
		}

        /// <summary>
        /// Initialize with a preloaded transaction.
        /// </summary>
        /// <param name="byteArray">Byte array containing the transaction contents.</param>
        public Transaction(Byte[] byteArray)
		{
            using (MemoryStream _memorystream = new MemoryStream(byteArray))
                Read(_memorystream);
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
            this.version = _binaryReader.ReadUInt32();
            this.inputs = new TxIn[VarInt.FromStream(streamReference)];
            for (int i = 0; i < this.inputs.Length; i++)
                this.inputs[i] = TxIn.FromStream(streamReference);
            this.outputs = new TxOut[VarInt.FromStream(streamReference)];
            for (int i = 0; i < this.outputs.Length; i++)
                this.outputs[i] = TxOut.FromStream(streamReference);
            this.lock_time = _binaryReader.ReadUInt32();
		}

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
		{
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
            _binarywriter.Write((UInt32)this.version);
            this.numInputs.Write(streamReference);
            for (int i = 0; i < this.inputs.Length; i++)
                this.inputs[i].Write(streamReference);
            this.numOutputs.Write(streamReference);
            for (int i = 0; i < this.outputs.Length; i++)
                this.outputs[i].Write(streamReference);
            _binarywriter.Write((UInt32)this.lock_time);
		}

        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw transaction data.</returns>
        public Byte[] ToBytes()
		{
            using (MemoryStream _memorystream = new MemoryStream())
			{
                Write(_memorystream);
                return _memorystream.ToArray();
			}
		}

        /// <summary>
        /// Read the contents of a (predefined) stream into a new copy of this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        /// <returns>A new copy of this structure containing the transaction data.</returns>
        public static Transaction FromStream(Stream s)
		{
            Transaction _transaction = new Transaction();
            _transaction.Read(s);
            return _transaction;
		}

        #endregion

    }
}
