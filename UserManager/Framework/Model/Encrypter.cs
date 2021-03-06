﻿using System;
using System.Security.Cryptography;

namespace UserManager.Framework.Model
{
    public class Encrypter
    {
        private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();


        private int getIntegerRoundedTowardLow(decimal value)
        {
            var CeilingValue = Math.Ceiling(value);
            if (CeilingValue > value) { value = CeilingValue - 1; }
            return System.Convert.ToInt32(value);
        }

        public Encrypter(string key)
        {
            //Initialize the crypto provider.
            var iKeyLength = getIntegerRoundedTowardLow(System.Convert.ToDecimal(TripleDes.KeySize / 8));
            var iBlockLength = getIntegerRoundedTowardLow(System.Convert.ToDecimal(TripleDes.BlockSize / 8));

            TripleDes.Key = TruncateHash(key, iKeyLength);
            TripleDes.IV = TruncateHash("", iBlockLength);

        }

        private byte getEmptyCharByte()
        {
            var aBytes = System.Text.Encoding.Unicode.GetBytes(" ");
            return aBytes[0];
        }

        private byte[] RedimPreserveByteArray(byte[] arr, int length)
        {
            if (arr == null) { return new byte[0]; }
            var iArrUpperBound = arr.Length - 1;
            var result = new byte[length];
            var value = getEmptyCharByte();
            var emptyCharByte = getEmptyCharByte();
            for (var i = 0; i < length; i++)
            {
                if (i > iArrUpperBound) { value = emptyCharByte; } else { value = arr[i]; }
                result.SetValue(value, i);
            }
            return result;
        }


        private byte[] TruncateHash(string key, int length)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            //Hash the key.
            var keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            var hash = sha1.ComputeHash(keyBytes);
            //Truncate or pad the hash.
            hash = RedimPreserveByteArray(hash, length);
            return hash;
        }

        public string EncryptData(string plaintext)
        {
            //Convert the plaintext string to a byte array.
            var plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);

            //Create the stream.
            var ms = new System.IO.MemoryStream();

            //Create the encoder to write to the stream.
            var encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            //Use the crypto stream to write the byte array to the stream.
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            encStream.FlushFinalBlock();

            //Convert the encrypted stream to a printable string.
            return Convert.ToBase64String(ms.ToArray());
        }

        public string DecryptData(string encryptedtext)
        {
            //Convert the encrypted text string to a byte array.
            //byte[] encryptedBytes = System.Text.Encoding.Unicode.GetBytes(encryptedtext);
            var encryptedBytes = Convert.FromBase64String(encryptedtext);

            //Create the stream.
            var ms = new System.IO.MemoryStream();

            //Create the decoder to write to the stream.
            var decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            //Use the crypto stream to write the byte array to the stream.
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();

            //Convert the plaintext stream to a string.
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }

    }
}
