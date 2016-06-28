using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MegaZord.Library.Helpers
{
    public class MZHelperCrypto
    {
        private static byte[] _salt = Encoding.ASCII.GetBytes("MegaZord.Library 2013 :) - Etiam quis enim id orci congue commodo. Nunc ut libero sed felis");



        private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
        private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits


        public static string HashText(string text)
        {
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(text, SaltSize, PBKDF2IterCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }

            byte[] outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        public static bool VerifyHashedText(string hashedText, string text)
        {
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedText);

            // Wrong length or version header.
            if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
                return false;

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            byte[] storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(text, salt, PBKDF2IterCount))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }
            return storedSubkey.SequenceEqual(generatedSubkey);
        }
        /// <summary>
        /// Encripta o texto
        /// </summary>
        /// <param name="plainText">Texto para ser criptografado</param>
        /// <returns>texto criptografado</returns>
        public static string EncryptString(string plainText)
        {
            return EncryptString(plainText, MZHelperConfiguration.MZPublicCryptoKey);
        }
        /// <summary>
        /// Texto para ser descriptografado
        /// </summary>
        /// <param name="plainText">Texto criptogragado</param>
        /// <returns>texto plano</returns>
        public static string DecryptString(string plainText)
        {
            return DecryptString(plainText, MZHelperConfiguration.MZPublicCryptoKey);
        }

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match. 
        /// The SharedSecret for the Password Reset that is used is in the next line
        ///  string sharedSecret = "OneUpSharedSecret9";
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        private static string EncryptString(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return


            // generate the key from the shared secret and the salt
            using (var key = new Rfc2898DeriveBytes(sharedSecret, _salt))
            {

                // Create a RijndaelManaged object
                using (var aesAlg = new RijndaelManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // prepend the IV
                        msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                        }

                        outStr = HttpServerUtility.UrlTokenEncode(msEncrypt.ToArray());
                        //outStr = Convert.ToBase64String(msEncrypt.ToArray());
                        // you may need to add a reference. right click reference in solution explorer => "add Reference" => .NET tab => select "System.Web"
                    }
                    aesAlg.Clear();
                }
            }


            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        private static string DecryptString(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

           

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            byte[] inputByteArray;

            try
            {
                // generate the key from the shared secret and the salt
                using (var key = new Rfc2898DeriveBytes(sharedSecret, _salt))
                {

                    // Create the streams used for decryption.                
                    //byte[] bytes = Convert.FromBase64String(cipherText);
                    inputByteArray = HttpServerUtility.UrlTokenDecode(cipherText);

                    using (MemoryStream msDecrypt = new MemoryStream(inputByteArray))
                    {
                        // Create a RijndaelManaged object
                        // with the specified key and IV.
                        using (var aesAlg = new RijndaelManaged())
                        {
                            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                            // Get the initialization vector from the encrypted stream
                            aesAlg.IV = ReadByteArray(msDecrypt);
                            // Create a decrytor to perform the stream transform.
                            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {

                                    // Read the decrypted bytes from the decrypting stream
                                    // and place them in a string.
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return "ERROR";
                //throw ex;

            }
             

            return plaintext;
        }

        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

    }

}
