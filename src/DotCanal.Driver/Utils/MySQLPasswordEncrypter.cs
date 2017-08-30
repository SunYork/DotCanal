using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DotCanal.Driver.Utils
{
    public static class MySQLPasswordEncrypter
    {
        public static byte[] Scramble411(byte[] pass, byte[] seed)
        {
            var sha1 = SHA1.Create();
            var pass1 = sha1.ComputeHash(pass);
            var pass2 = sha1.ComputeHash(pass1);

            sha1.
        }
    }
}
