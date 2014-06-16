using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BitCoin.Structs
{

    /// <summary>
    /// Byte array representing a hash value.
    /// </summary>
	public class Hash
	{

        /* ********************************************************************************
         * Property declarations
         * ****************************************************************************** */
        #region "Property declarations"

        /// <summary>
        /// Byte array representing a hash value.
        /// </summary>
		public Byte[] hash;

        /// <summary>
        /// Redirect this class to be a property.
        /// </summary>
        /// <param name="i">Length of this array.</param>
        /// <returns>Contents of this array.</returns>
		public Byte this[int i]
		{
			get { return this.hash[i]; }
			set { this.hash[i] = value; }
		}

        #endregion

        /* ********************************************************************************
         * Class constructors
         * ****************************************************************************** */
        #region "Class constructors"

        /// <summary>
        /// Initialize with a transaction with predefined values.
        /// </summary>
        /// <param name="byteArray"></param>
        public Hash(Byte[] byteArray)
		{
			this.hash = byteArray;
		}

        #endregion

        /* ********************************************************************************
         * Functions
         * ****************************************************************************** */
        #region "Functions"

        /// <summary>
        /// Extend the built-in operators to provide explicit conversion from a Hash type into a Byte array.
        /// </summary>
        /// <param name="value">Source hash</param>
        /// <returns>Hash value as a byte array</returns>
        public static implicit operator Byte[](Hash value)
		{
			return value.hash;
		}

        /// <summary>
        /// Extend the built-in operators to provide explicit conversion from a a Byte array into Hash type.
        /// </summary>
        /// <param name="value">Source byte array</param>
        /// <returns> byte arrayas a Hash typed value</returns>
		public static implicit operator Hash(Byte[] value)
		{
			return new Hash(value);
		}

        /// <summary>
        /// Extend the built-in operators to provide explicit comarison of two Hash typed values
        /// </summary>
        /// <param name="value">Target hash to compare to.</param>
        /// <returns>True if value is equal to this hash value, false if not.</returns>
		public override bool Equals(Object value)
		{
			if (value != null && value is Hash)
				return this.hash.SequenceEqual(((Hash)value).hash);
			return false;
		}

        /// <summary>
        /// Generates a hashcode
        /// </summary>
        /// <returns>
        /// The hashcode
        /// </returns>
		public override int GetHashCode()
		{
			if (this.hash.Length >= 4)
                return (this.hash[0] << 24) | (this.hash[1] << 16) | (this.hash[2] << 8) | (this.hash[3] << 0);
            return this.hash.GetHashCode();
		}

        #endregion

	}
}
