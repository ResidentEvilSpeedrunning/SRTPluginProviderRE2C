using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE2C
{
    public static class GameHashes
    {
        private static readonly byte[] re2C_Rebirth_1p10 = new byte[32] { 33, 178, 166, 71, 203, 237, 218, 193, 236, 56, 155, 232, 71, 242, 42, 67, 74, 2, 100, 161, 128, 32, 88, 157, 233, 28, 178, 130, 77, 203, 27, 245 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(re2C_Rebirth_1p10))
                return GameVersion.re2C_Rebirth_1p10;
            else
                return GameVersion.Unknown;
        }
    }
}
