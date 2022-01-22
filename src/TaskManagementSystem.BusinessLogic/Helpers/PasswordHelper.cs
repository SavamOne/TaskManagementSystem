using System.Security.Cryptography;
using System.Text;

namespace TaskManagementSystem.BusinessLogic.Helpers;

public static class PasswordHelper
{
    private static readonly Encoding PasswordEncoding = Encoding.ASCII;

    public static byte[] GetHash(string password)
    {
        SHA512 hashAlgorithm = new SHA512CryptoServiceProvider();
        byte[] passwordBytes = PasswordEncoding.GetBytes(password);
        return hashAlgorithm.ComputeHash(passwordBytes);
    }
}