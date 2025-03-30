using System.Security.Cryptography;
using System.Text;

namespace Utils_DotNet.PasswordHelper
{
    /// <summary>
    /// 使用 SHA256 加盐 加密密码；
    /// 用户须有 password slat 属性
    /// </summary>
    public class PasswordHelper
    {
        /// <summary>
        /// 将 密码 转化为 加密密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string HashPassword(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var passwordWithSaltBytes = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, passwordWithSaltBytes, passwordBytes.Length, salt.Length);

            var hash = SHA256.HashData(passwordBytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// 验证密码，通过返回true,否则返回false
        /// </summary>
        /// <param name="password">用户输入密码</param>
        /// <param name="passwordHash">数据库密码</param>
        /// <param name="salt">盐</param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string passwordHash, byte[] salt) { 
            return HashPassword(password, salt) == passwordHash;
        }

        /// <summary>
        /// 生成随机的16字节 byte[]盐 
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(16);
        }

    }
}
