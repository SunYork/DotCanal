using System;
using System.Security.Cryptography;
using System.Text;

namespace DotCanal.Driver.Utils
{
    public static class MySQLPasswordEncrypter
    {
        public static byte[] Scramble411(string password, byte[] seed)
        {
            byte[] pass = Encoding.UTF8.GetBytes(password);
            if (pass.Length == 0) return new byte[1];
            SHA1 sha = SHA1.Create();

            byte[] firstHash = sha.ComputeHash(pass);
            byte[] secondHash = sha.ComputeHash(firstHash);

            byte[] input = new byte[seed.Length + secondHash.Length];
            Array.Copy(seed, 0, input, 0, seed.Length);
            Array.Copy(secondHash, 0, input, seed.Length, secondHash.Length);
            byte[] thirdHash = sha.ComputeHash(input);

            byte[] finalHash = new byte[thirdHash.Length + 1];
            finalHash[0] = 0x14;
            Array.Copy(thirdHash, 0, finalHash, 1, thirdHash.Length);

            for (int i = 1; i < finalHash.Length; i++)
                finalHash[i] = (byte)(finalHash[i] ^ firstHash[i - 1]);
            return finalHash;
        }
    }
}
