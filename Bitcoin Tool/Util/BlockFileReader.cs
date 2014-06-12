using System;
using System.IO;
using Bitcoin_Tool.Structs;

namespace Bitcoin_Tool.Util
{

    /// <summary>
    /// BitCoin BlockChain datafile(s) reader functions
    /// </summary>
    public class BlockFileReader
    {

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Public Properties"

        /// <summary>
        /// Consecutive number of the active BlockChain datafile.
        /// </summary>
        /// <remarks>
        /// Can be read from everywhere, but only be set from within this class.
        /// </remarks>
        public int blockFileNum { get; private set; }

        #endregion "Public Properties"

        #region "Private Properties"

        /// <summary>
        /// Path to the location of the BitCoin BlockChain datafile(s).
        /// </summary>
        private String _blockFilePath = String.Empty;

        private String _blockFileName
        {
            get
            {
                return "blk" + blockFileNum.ToString("D5") + ".dat";
            }
        }

        /// <summary>
        /// FileStream for the active BlockChain datafile.
        /// </summary>
        private FileStream _fileStream;

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Initializes the filestream with the default BitCoin-QT / BitCoind datapath and blockchain datafile.
        /// </summary>
        public BlockFileReader()
        {
            this.InitBlockFileReader(String.Empty, String.Empty);
        }

        /// <summary>
        /// Initializes the filestream with the default BitCoin-QT / BitCoind datapath and a custom filename.
        /// </summary>
        /// <param name="file">Name of the blockchain datafile.</param>
        public BlockFileReader(String file)
        {
            this.InitBlockFileReader(file, String.Empty);
        }

        /// <summary>
        /// Initializes the filestream with a custom BitCoin-QT/BitCoind datapath and/or blockchain datafile
        /// </summary>
        /// <param name="file">Name of the blockchain datafile</param>
        /// <param name="path">Location of the blockchain datafile(s)</param>
        /// <remarks>
        /// Pass an empty String or null value to initialize the file or path value with the defaults.
        /// </remarks>
        public BlockFileReader(String file, String path)
        {
            this.InitBlockFileReader(file, path);
        }

        #endregion

        /* ********************************************************************************
         * Functions
         * ****************************************************************************** */
        #region "Functions"

        /// <summary>
        /// Initializes the filestream with a custom BitCoin-QT/BitCoind datapath and/or blockchain datafile.
        /// </summary>
        /// <param name="file">Name of the blockchain datafile.</param>
        /// <param name="path">Location of the blockchain datafile(s).</param>
        /// <remarks>
        /// Pass an empty String or null value to initialize the file or path value with the defaults.
        /// </remarks>
        private void InitBlockFileReader(String file, String path)
        {

            blockFileNum = 0;
            _blockFilePath = path;

            if (String.IsNullOrWhiteSpace(path))
                _blockFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bitcoin\blocks");
            if (!System.IO.Directory.Exists(path))
                throw new Exception("BitCoin BlockChain datafolder ('" + path + "') not found!");

            if (String.IsNullOrWhiteSpace(file))
                file = _blockFileName;

            String _fullPath = System.IO.Path.Combine(path, file);
            if (!System.IO.File.Exists(_fullPath))
                throw new Exception("BitCoin BlockChain datafile ('" + file + "') not found in '" + path + "'!");

            _fileStream = new FileStream(_fullPath, FileMode.Open);

        }

         /// <summary>
        /// Reads a block disk into the <seealso cref="Block_Disk">Block_Disk</seealso> structure from the active filestream and opens the next blockfile.
        /// </summary>
        /// <returns>A <seealso cref="Block_Disk"/>Block_Disk</seealso> structure containing the contents of the blockfile.</returns>
       public Block_Disk getNext()
        {
            return getNext(String.Empty, String.Empty);
        }

        /// <summary>
        /// Reads a block disk into the <seealso cref="Block_Disk">Block_Disk</seealso> structure from the active filestream and opens the next blockfile.
        /// </summary>
        /// <param name="file">Name of the blockchain datafile.</param>
        /// <returns>A <seealso cref="Block_Disk"/>Block_Disk</seealso> structure containing the contents of the blockfile.</returns>
        public Block_Disk getNext(String file)
        {
            return getNext(file, String.Empty);
        }

        /// <summary>
        /// Reads a block disk into the <seealso cref="Block_Disk">Block_Disk</seealso> structure from the active filestream and opens the next blockfile.
        /// </summary>
        /// <param name="file">Name of the blockchain datafile.</param>
        /// <param name="path">Location of the blockchain datafile(s).</param>
        /// <returns>A <seealso cref="Block_Disk"/>Block_Disk</seealso> structure containing the contents of the blockfile.</returns>
        /// <remarks>
        /// Pass an empty String or null value to initialize the file or path value with the defaults.
        /// </remarks>
        public Block_Disk getNext(String file, String path)
        {
            if (String.IsNullOrWhiteSpace(file))
                file = _blockFileName;
            if (String.IsNullOrWhiteSpace(path))
                path = _blockFilePath;

            while (_fileStream.Position < _fileStream.Length)
            {
                Block_Disk _block;
                _block = Block_Disk.FromStream(_fileStream);

                if (_block.blockSize == 0)
                {
                    blockFileNum++;
                    try
                    {
                        _fileStream.Close();
                        String _fullPath = System.IO.Path.Combine(path, file);
                        _fileStream = new FileStream(_fullPath, FileMode.Open);
                    }
                    catch (FileNotFoundException)
                    {
                        return null;
                    }
                    continue;
                }

                return _block;
            }

            return null;
        }

        #endregion
    }
}
