using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Utility
{
    // <summary>
    /// This class is used for utility functions that encrypt/decrypt or hash strings.
    /// </summary>
    public class StringCryptography
    {
        public static readonly StringCryptography Instance = new StringCryptography();

        /// <summary>
        /// Get a randomly generated string of a given length.
        /// </summary>
        /// <param name="length">The desired length of the string.</param>
        /// <returns>A randomly generated string.</returns>
        public string GetRandomString(int length)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            byte[] str = new byte[length];

            provider.GetNonZeroBytes(str);

            string newString = "";

            foreach (byte b in str)
                newString += b.ToString("x2");

            return newString;
        }

        /// <summary>
        /// Get a salted and hashed version of the input value.
        /// </summary>
        /// <param name="value">The input to generate a hashed value from.</param>
        /// <param name="salt">The salt to add to the input value before hashing.</param>
        /// <returns>A salted and hashed value computed from the given input value and salt.</returns>
        public string GetSaltedHashedValueAsString(string value, string salt)
        {
            return Encoding.UTF8.GetString(GetSaltedHashedValue(value, salt));
        }

        /// <summary>
        /// Get a salted and hashed version of the input value.
        /// </summary>
        /// <param name="value">The input to generate a hashed value from.</param>
        /// <param name="salt">The salt to add to the input value before hashing.</param>
        /// <returns>A salted and hashed value computed from the given input value and salt.</returns>
        public byte[] GetSaltedHashedValue(string value, string salt)
        {
            return GetSaltedHashedValue(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt));
        }

        /// <summary>
        /// Get a salted and hashed version of the input value.
        /// </summary>
        /// <param name="value">The input to generate a hashed value from.</param>
        /// <param name="salt">The salt to add to the input value before hashing.</param>
        /// <returns>A salted and hashed value computed from the given input value and salt.</returns>
        public byte[] GetSaltedHashedValue(byte[] value, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] saltedValue = value.Concat(salt).ToArray();

            return new SHA256Managed().ComputeHash(saltedValue);
        }
    }
}
