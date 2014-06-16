using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BitCoin.Structs
{

    /// <summary>
    /// Transaction outputs or destinations for coins.
    /// </summary>
    public class TxOut : ISerialize
    {

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// Transaction Value.
        /// </summary>
        public UInt64 value;

        /// <summary>
        /// Length of the scriptPubKey .
        /// </summary>
        public VarInt scriptPubKeyLen { get { return new VarInt(scriptPubKey.Length); } }

        /// <summary>
        /// Usually contains the public key as a Bitcoin script setting up conditions to claim this output.
        /// </summary>
        public Byte[] scriptPubKey;

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        protected TxOut()
        {
        }

        /// <summary>
        /// Initialize with a transaction with predefined values.
        /// </summary>
        /// <param name="value">Transaction Value.</param>
        /// <param name="scriptPubKey">The public key as a Bitcoin script setting up conditions to claim this output.</param>
        public TxOut(UInt64 value, Byte[] scriptPubKey)
        {
            this.value = value;
            this.scriptPubKey = scriptPubKey;
        }

        /// <summary>
        /// Initialize with a preloaded transaction.
        /// </summary>
        /// <param name="byteArray">Byte array containing the transaction contents.</param>
        public TxOut(Byte[] byteArray)
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
            this.value = _binaryReader.ReadUInt64();
            this.scriptPubKey = _binaryReader.ReadBytes(VarInt.FromStream(streamReference).intValue);
        }

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
        {
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
            _binarywriter.Write((UInt64)this.value);
            this.scriptPubKeyLen.Write(streamReference);
            _binarywriter.Write(this.scriptPubKey, 0, this.scriptPubKey.Length);
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
        public static TxOut FromStream(Stream streamReference)
        {
            TxOut _transaction = new TxOut();
            _transaction.Read(streamReference);
            return _transaction;
        }

        #endregion

    }
}
