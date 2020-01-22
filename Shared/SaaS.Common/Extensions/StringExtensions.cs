using System;
using System.Security.Cryptography;

namespace SaaS.Common.Extensions
{
    public static class StringExtensions
    {
        private const string _keyforhashing = "F&kd9chvzv4kK6Z5xu%MbLMsLrat1jxihgAjlfJdW_Vgx-nsqUPsAbhXQaCa2k";

        public static string EncryptString(this string texttoencrypt)
        {
            if (string.IsNullOrEmpty(texttoencrypt))
                throw new ArgumentNullException("texttoencrypt can not be null or empty");

            var rijndaelCipher = new RijndaelManaged();

            var plainText = System.Text.Encoding.Unicode.GetBytes(texttoencrypt);
            var salt = System.Text.Encoding.ASCII.GetBytes(_keyforhashing.Length.ToString());
            var encryptedData = "";
            //var secretKey = new Rfc2898DeriveBytes(_keyforhashing, salt);
            var secretKey = new PasswordDeriveBytes(_keyforhashing, salt);

            //Creates a symmetric encryptor object. 
            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            using (var memoryStream = new System.IO.MemoryStream())
            {
                //Defines a stream that links data streams to cryptographic transformations
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainText, 0, plainText.Length);
                    cryptoStream.FlushFinalBlock();
                    var cipherBytes = memoryStream.ToArray();
                    encryptedData = Convert.ToBase64String(cipherBytes);
                }
            }

            return encryptedData;
        }
        public static string DecryptString(this string texttodecrypt)
        {
            if (string.IsNullOrWhiteSpace(texttodecrypt))
                throw new ArgumentNullException("texttodecrypt can not be null or empty");

            var rijndaelCipher = new RijndaelManaged();

            var encryptedData = Convert.FromBase64String(texttodecrypt);
            var salt = System.Text.Encoding.ASCII.GetBytes(_keyforhashing.Length.ToString());
            var decryptedData = "";
            //Making of the key for decryption
            //var secretKey = new Rfc2898DeriveBytes(_keyforhashing, salt);
            var secretKey = new PasswordDeriveBytes(_keyforhashing, salt);

            //Creates a symmetric Rijndael decryptor object.
            ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            using (var memoryStream = new System.IO.MemoryStream(encryptedData))
            {
                //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var plainText = new byte[encryptedData.Length];
                    var decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                    decryptedData = System.Text.Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                }
            }
            return decryptedData;
        }


        public static bool IsValidGuid(this string str)
        {
            Guid guid;
            return Guid.TryParse(str, out guid);
        }

        public static bool IsValidGuid(this string str, out Guid guid)
        {
            return Guid.TryParse(str, out guid);
        }
    }
}
