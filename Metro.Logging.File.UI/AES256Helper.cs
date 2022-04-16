using System;
using System.Security.Cryptography;
using System.Text;

namespace Metro.Logging.File.UI
{
    public static class AES256Helper
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Encrypt(string encryptStr, byte[] key)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(encryptStr);
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            //aes.IV = Encoding.UTF8.GetBytes(iv);
            var transform = aes.CreateEncryptor();
            return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string decryptStr, byte[] key)
        {
            var encryptBytes = Convert.FromBase64String(decryptStr);
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            //aes.IV = Encoding.UTF8.GetBytes(iv);
            var transform = aes.CreateDecryptor();
            return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }            
    }
}