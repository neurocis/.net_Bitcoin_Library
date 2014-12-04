﻿using System;
using System.Security.Cryptography;
using BitCoin.Scripts;
using BitCoin.DataConverters;
using BitCoin.Structs;
using System.Linq;

namespace BitCoin.Crypto
{
	public class Address
	{
        public const Byte PUBKEYHASH = 15; //PIG
        public const Byte SCRIPTHASH = 5;  //PIG
        //public const Byte PUBKEYHASH = 118; //PIGGY
        //public const Byte SCRIPTHASH = 28;  //PIGGY
        public const Byte PUBKEY = 111;
        public const Byte SCRIPT = 196;
        //public const Byte PUBKEYHASH = 0x00;  //BTC
        //public const Byte SCRIPTHASH = 0x05;  //BTC
        //public const Byte PUBKEY = 0xFE;
		//public const Byte SCRIPT = 0xFF;

		private String address = null;
		private Hash pubKeyHash = null;
		private Hash scriptHash = null;
		private Byte? type = null;

		public Hash PubKeyHash
		{
			get
			{
				if (pubKeyHash == null && calcHash() != PUBKEYHASH)
					throw new InvalidOperationException("Address is not a public key hash.");
				return pubKeyHash;
			}
		}

		public Hash ScriptHash
		{
			get
			{
				if (pubKeyHash == null && calcHash() != SCRIPTHASH)
					throw new InvalidOperationException("Address is not a script hash.");
				return scriptHash;
			}
		}

		public Hash EitherHash
		{
			get
			{
				if (pubKeyHash == null && scriptHash == null)
					calcHash();
				if (pubKeyHash != null)
					return pubKeyHash;
				if (scriptHash != null)
					return scriptHash;
				return null;
			}
		}

		public Byte Type
		{
			get
			{
				if (type == null)
					calcHash();
				return type.Value;
			}
		}

		public Address(Byte[] data, Byte version = PUBKEY)
		{
			SHA256 sha256 = new SHA256Managed();
			RIPEMD160 ripemd160 = new RIPEMD160Managed();
			switch (version)
			{
				case PUBKEY:
					pubKeyHash = ripemd160.ComputeHash(sha256.ComputeHash(data));
					version = PUBKEYHASH;
					break;
				case SCRIPT:
					scriptHash = ripemd160.ComputeHash(sha256.ComputeHash(data));
					version = SCRIPTHASH;
					break;
				case PUBKEYHASH:
					pubKeyHash = data;
					break;
				case SCRIPTHASH:
					scriptHash = data;
					break;
			}
			this.type = version;
		}

		public Address(String address)
		{
			this.address = address;
		}

		private Byte calcHash()
		{
			Byte version;
			Byte[] hash = Base58CheckString.ToByteArray(this.ToString(), out version);
			switch (version)
			{
				case PUBKEYHASH:
					pubKeyHash = hash;
					break;
				case SCRIPTHASH:
					scriptHash = hash;
					break;
			}
			type = version;
			return version;
		}

		private void calcBase58()
		{
			if (pubKeyHash != null)
				this.address = Base58CheckString.FromByteArray(pubKeyHash, PUBKEYHASH);
			else if (scriptHash != null)
				this.address = Base58CheckString.FromByteArray(scriptHash, SCRIPTHASH);
			else
				throw new InvalidOperationException("Address is not a public key or script hash!");
		}

		public static Address FromScript(Byte[] b)
		{
			Script s = new Script(b);
			if (s.IsPayToPubKeyHash())
				return new Address(s.elements[s.elements.Count - 3].data, PUBKEYHASH);
			if (s.IsPayToScriptHash())
				return new Address(s.elements[s.elements.Count - 2].data, SCRIPTHASH);
			if (s.IsPayToPublicKey())
				return new Address(s.elements[s.elements.Count - 2].data, PUBKEY);
			return null;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Address))
				return false;
			if (this.EitherHash == null || ((Address)obj).EitherHash == null)
				return false;
			return this.EitherHash.hash.SequenceEqual(((Address)obj).EitherHash.hash);
		}

		public override int GetHashCode()
		{
			return this.EitherHash.GetHashCode();
		}

		public override String ToString()
		{
			if (address == null)
				calcBase58();
			return address;
		}
	}
}
