using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitCoin.Structs.Other
{

    /// <summary>
    /// The outpoint structure.
    /// </summary>
    /// <remarks>
    /// See also https://en.bitcoin.it/wiki/Protocol_specification#tx for more details.
    /// </remarks>
	public struct OutPoint : ISerialize
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        /// <summary>
        /// The hash of the referenced transaction.
        /// </summary>
		public Hash hash;

        /// <summary>
        /// The index of the specific output in the transaction.
        /// </summary>
		public UInt32 index;

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Initialize with a transaction with predefined values.
        /// </summary>
        /// <param name="hash">The hash of the referenced transaction.</param>
        /// <param name="index">The index of the specific output in the transaction.</param>
        public OutPoint(Hash hash, UInt32 index)
		{
			this.hash = hash;
			this.index = index;
		}

        /// <summary>
        /// Initialize with a preloaded transaction.
        /// </summary>
        /// <param name="byteArray">Byte array containing the transaction contents.</param>
        public OutPoint(Byte[] byteArray)
		{
			this.hash = null;
			this.index = 0;
            using (MemoryStream _memorystream = new MemoryStream(byteArray))
				this.Read(_memorystream);
		}

        #endregion

        /* ********************************************************************************
         * Functions
         * ****************************************************************************** */
        #region "Functions"

        /// <summary>
        /// Extend the built-in operators to provide explicit comarison of two OutPoint typed values
        /// </summary>
        /// <param name="value">Target hash to compare to.</param>
        /// <returns>True if value is equal to this hash value, false if not.</returns>
        public override bool Equals(Object value)
		{
			if (value != null && value is OutPoint)
				return (index == ((OutPoint)value).index) && hash.Equals(((OutPoint)value).hash);
			return false;
		}

        /// <summary>
        /// Generate hashcode.
        /// </summary>
        /// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return (int)(hash.GetHashCode() ^ index);
		}

        /// <summary>
        /// Load the contents of a stream into this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Read(Stream streamReference)
		{
            BinaryReader _binaryReader = new BinaryReader(streamReference);
			this.hash = _binaryReader.ReadBytes(32);
			this.index = _binaryReader.ReadUInt32();
		}

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
		{
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
			_binarywriter.Write(this.hash, 0, 32);
			_binarywriter.Write((UInt32)this.index);
		}


        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw OutPoint data.</returns>
        public Byte[] ToBytes()
		{
            using (MemoryStream _memorystream = new MemoryStream())
			{
				this.Write(_memorystream);
				return _memorystream.ToArray();
			}
		}

        /// Read the contents of a (predefined) stream into a new copy of this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        /// <returns>A new copy of this structure containing the transaction data.</returns>
        public static OutPoint FromStream(Stream streamReference)
		{
            OutPoint _outpoint = new OutPoint();
            _outpoint.Read(streamReference);
            return _outpoint;
        }

        #endregion

    }
}
