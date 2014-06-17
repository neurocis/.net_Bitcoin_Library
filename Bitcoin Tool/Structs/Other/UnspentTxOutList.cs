using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitCoin.Structs.Other
{

    /// <summary>
    /// An enumerated list of Unspent Transaction outputs or destinations for coins.
    /// </summary>
	public class UnspentTxOutList : ISerialize, IEnumerable<KeyValuePair<OutPoint, TxOut>>
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        #region "Private Properties"

        /// <summary>
        /// List of Unspent Transaction outputs or destinations for coins.
        /// </summary>
		private Dictionary<OutPoint, TxOut> _unspentTxOutList = new Dictionary<OutPoint, TxOut>();

        /// <summary>
        /// List of duplicate entries
        /// </summary>
        private List<KeyValuePair<OutPoint, TxOut>> _duplicateTxOutList = new List<KeyValuePair<OutPoint, TxOut>>();

        #endregion

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Clean initialization.
        /// </summary>
        public UnspentTxOutList()
		{
		}

        /// <summary>
        /// Initialize with a preloaded range.
        /// </summary>
        /// <param name="byteArray">Byte array containing the range contents.</param>
        public UnspentTxOutList(Byte[] byteArray)
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
        /// Adds a Transaction Output to this list.
        /// </summary>
        /// <param name="txReference">Transaction reference</param>
        /// <param name="txOut">Transaction output contents</param>
        /// <remarks>
        /// If this entry allready exists in this list, it will be added to the list of suplicates.
        /// </remarks>
        public void Add(OutPoint txReference, TxOut txOut)
		{
			try
			{
                this._unspentTxOutList.Add(txReference, txOut);
			}
			catch (ArgumentException)
			{
				// Duplicate!
                this._duplicateTxOutList.Add(new KeyValuePair<OutPoint, TxOut>(txReference, txOut));
			}
		}

        /// <summary>
        /// Attempt to remove a transaction Output from this list.
        /// </summary>
        /// <param name="txReference">Transaction reference</param>
        /// <returns>True if succesfully removed from this or the duplicates list, false if unsuccesful.</returns>
        public bool TryRemove(OutPoint txReference)
		{
            if (this._unspentTxOutList.Remove(txReference))
				return true;
			// Not found.. in dup?
            int i = this._duplicateTxOutList.FindIndex(x => x.Key.Equals(txReference));
			if (i >= 0)
			{
                this._duplicateTxOutList.RemoveAt(i);
				return true;
			}
			// Still not found...
			return false;
		}

        /// <summary>
        /// Load the contents of a stream into this structure.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Read(Stream streamReference)
		{
            this._unspentTxOutList.Clear();
            this._duplicateTxOutList.Clear();
            BinaryReader _binaryReader = new BinaryReader(streamReference);

			UInt64 _count = _binaryReader.ReadUInt64();
			for (UInt64 i = 0; i < _count; i++)
			{
                OutPoint _txReference = OutPoint.FromStream(streamReference);
				TxOut _txOut = TxOut.FromStream(streamReference);
                this._unspentTxOutList.Add(_txReference, _txOut);
			}
			
            UInt64 _dupcount = _binaryReader.ReadUInt64();
			for (UInt64 i = 0; i < _dupcount; i++)
			{
                OutPoint _txReference = OutPoint.FromStream(streamReference);
                TxOut _txOut = TxOut.FromStream(streamReference);
                this._duplicateTxOutList.Add(new KeyValuePair<OutPoint, TxOut>(_txReference, _txOut));
			}
		}

        /// <summary>
        /// (Re)write the contents of this structure into a stream.
        /// </summary>
        /// <param name="streamReference">Reference to the (predefined) stream.</param>
        public void Write(Stream streamReference)
		{
			// Attempt cleanup
            for (int i = 0; i < this._duplicateTxOutList.Count; i++)
			{
				try
				{
                    this._unspentTxOutList.Add(this._duplicateTxOutList[i].Key, this._duplicateTxOutList[i].Value);
                    this._duplicateTxOutList.RemoveAt(i);
					i = -1;
				}
				catch (ArgumentException) { }
			}

            BinaryWriter _binarywriter = new BinaryWriter(streamReference);
            _binarywriter.Write((UInt64)this._unspentTxOutList.Count);
            foreach (KeyValuePair<OutPoint, TxOut> _entry in this._unspentTxOutList)
			{
				_entry.Key.Write(streamReference);
				_entry.Value.Write(streamReference);
			}
            _binarywriter.Write((UInt64)this._duplicateTxOutList.Count);
            foreach (KeyValuePair<OutPoint, TxOut> _entry in this._duplicateTxOutList)
			{
				_entry.Key.Write(streamReference);
				_entry.Value.Write(streamReference);
			}
		}

        /// <summary>
        /// Export the contents of this structure into a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw transaction output data.</returns>
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
        /// <returns>A new copy of this structure containing the transaction output data.</returns>
        public static UnspentTxOutList FromStream(Stream streamReference)
		{
            UnspentTxOutList _unspenttxoutlist = new UnspentTxOutList();
			_unspenttxoutlist.Read(streamReference);
			return _unspenttxoutlist;
		}

        /// <summary>
        /// Gets the enumerated value
        /// </summary>
		public IEnumerator<KeyValuePair<OutPoint, TxOut>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<OutPoint, TxOut>> _enumerator;
            _enumerator = this._unspentTxOutList.GetEnumerator();
			while (_enumerator.MoveNext())
				yield return _enumerator.Current;
            _enumerator = this._duplicateTxOutList.GetEnumerator();
			while (_enumerator.MoveNext())
				yield return _enumerator.Current;
		}

        /// <summary>
        /// Gets the enumerated value
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
		{
            IEnumerator<KeyValuePair<OutPoint, TxOut>> _enumerator;
            _enumerator = this._unspentTxOutList.GetEnumerator();
			while (_enumerator.MoveNext())
				yield return _enumerator.Current;
            _enumerator = this._duplicateTxOutList.GetEnumerator();
			while (_enumerator.MoveNext())
				yield return _enumerator.Current;	
		}

        #endregion

    }
}
