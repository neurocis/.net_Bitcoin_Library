using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BitCoin.Structs
{

    /// <summary>
    /// BlockFile contents.
    /// </summary>
    public class Block_Disk : Block, ISerialize
    {

        /* ********************************************************************************
        * Property declarations
        * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// Magic bytes (identifying which network you are on).
        /// </summary>
        public UInt32 magic;

        /// <summary>
        /// Total number of bytes in this block (including header).
        /// </summary>
        public UInt32 blockSize;

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructor(s)
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        protected Block_Disk()
        {
        }

        /// <summary>
        /// Initialize with a preloaded blockfile.
        /// </summary>
        /// <param name="byteArray">Byte array containing the blockfile contents.</param>
        public Block_Disk(Byte[] byteArray)
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
        public override void Read(Stream streamReference)
        {
            BinaryReader _binaryReader = new BinaryReader(streamReference);
            magic = _binaryReader.ReadUInt32();      // Get the magic bytes
            blockSize = _binaryReader.ReadUInt32();  // Get total number of bytes in this block
            base.Read(streamReference);              // Read the remaining bytes (see Block structure)
        }

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="blockFileStream">Reference to the (predefined) stream.</param>
        public override void Write(Stream blockFileStream)
        {
            BinaryWriter _binarywriter = new BinaryWriter(blockFileStream);
            _binarywriter.Write((UInt32)magic);      // Set the magic bytes
            _binarywriter.Write((UInt32)blockSize);  // Set total number of bytes in this block
            base.Write(blockFileStream);             // Write the remaining bytes (see Block structure)
        }

        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw blockfile data.</returns>
        public override Byte[] ToBytes()
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
        public new static Block_Disk FromStream(Stream streamReference)
        {
            Block_Disk _diskBlock = new Block_Disk();
            _diskBlock.Read(streamReference);
            return _diskBlock;
        }

        #endregion
    }
}
