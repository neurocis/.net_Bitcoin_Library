using System;
using System.IO;

namespace BitCoin.Structs
{

    /// <summary>
    /// Variable length integer. A integer that can be encoded depending on the represented value to save space.
    /// </summary>
    /// <remarks>
    /// See https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_integer for more detailed Information.
    /// </remarks>
    public class VarInt : ISerialize
    {

        /* ********************************************************************************
        * Property declarations
        * ****************************************************************************** */
        #region "Property declarations"

        /// <summary>
        /// VarInt value.
        /// </summary>
        public UInt64 value;

        /// <summary>
        /// Standard 32bit Integer representation of the VarInt value.
        /// </summary>
        public Int32 intValue { get { return (Int32)value; } }

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        public VarInt()
        {
        }

        /// <summary>
        /// Initializing with a Signed 64bit Integer
        /// </summary>
        /// <param name="i">Signed 64bit Integer value</param>
        public VarInt(Int64 i)
        {
            this.value = (UInt64)i;
        }

        /// <summary>
        /// Initializing with an Unsigned 64bit Integer
        /// </summary>
        /// <param name="i">Unsigned 64bit Integer value</param>
        public VarInt(UInt64 i)
        {
            this.value = i;
        }

        /// <summary>
        /// Initialize with a preloaded byte array.
        /// </summary>
        /// <param name="byteArray">Byte array containing the contents.</param>
        public VarInt(Byte[] byteArray)
        {
            using (MemoryStream _memorystream = new MemoryStream(byteArray))
            {
                this.Read(_memorystream);
            }
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
            byte _type = _binaryReader.ReadByte();
            if (_type < 0xFD)
                this.value = _type;
            else if (_type == 0xFD)
                this.value = _binaryReader.ReadUInt16();
            else if (_type == 0xFE)
                this.value = _binaryReader.ReadUInt32();
            else
                this.value = _binaryReader.ReadUInt64();
        }

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
        {
            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
            if (this.value < 0xFD)
            {
                _binarywriter.Write((Byte)this.value);
            }
            else if (this.value < UInt16.MaxValue)
            {
                _binarywriter.Write((Byte)0xFD);
                _binarywriter.Write((UInt16)this.value);
            }
            else if (this.value < UInt32.MaxValue)
            {
                _binarywriter.Write((Byte)0xFE);
                _binarywriter.Write((UInt32)this.value);
            }
            else
            {
                _binarywriter.Write((Byte)0xFF);
                _binarywriter.Write((UInt64)this.value);
            }
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
        public static VarInt FromStream(Stream streamReference)
        {
            VarInt _varint = new VarInt(0);
            _varint.Read(streamReference);
            return _varint;
        }

        /// <summary>
        /// Extend the built-in operators or to provide conversion from a Unsigned 64bit Integer into the VarInt type.
        /// </summary>
        /// <param name="i">A Unsigned 64bit Integer value</param>
        /// <returns>Value of the type VarInt</returns>
        public static implicit operator VarInt(UInt64 i)
        {
            return new VarInt(i);
        }

        /// <summary>
        /// Extend the built-in operators or to provide conversion from a VarInt into the Unsigned 64bit Integer type.
        /// </summary>
        /// <param name="vi">Value of the type VarInt</param>
        /// <returns>A Unsigned 64bit Integer value</returns>
        public static implicit operator UInt64(VarInt vi)
        {
            return vi.value;
        }

        /// <summary>
        /// Extend the built-in operators or to provide conversion from a VarInt into the Signed 64bit Integer type.
        /// </summary>
        /// <param name="vi">Value of the type VarInt</param>
        /// <returns>A Signed 64bit Integer value</returns>
        public static implicit operator Int64(VarInt vi)
        {
            return (Int64)vi.value;
        }

        #endregion

    }
}
