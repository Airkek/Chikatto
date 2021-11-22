using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Chikatto.Utils
{
    public static class Submission
    {
        public static async Task<string[]> Decrypt(string scoreB64, string ivB64, string osuver)
        {
            var key = $"osu!-scoreburgr---------{osuver}";

            var iv = Convert.FromBase64String(ivB64);
            var scoreEncr = Convert.FromBase64String(scoreB64);

            var rjd = new RijndaelEngine(256);

            var keyParam = new KeyParameter(Encoding.Latin1.GetBytes(key));
            var keyParamIV = new ParametersWithIV(keyParam, iv, 0, 32);

            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(rjd), new Pkcs7Padding());

            cipher.Init(false, keyParamIV);

            var decryptedBytes = new byte[cipher.GetOutputSize(scoreEncr.Length)];

            var length = cipher.ProcessBytes(scoreEncr, decryptedBytes, 0);

            cipher.DoFinal(decryptedBytes, length);

            return Encoding.UTF8.GetString(decryptedBytes).Split(':');
        }

        public static string ChartEntry<T>(string name, T before, T after) => $"{name}Before:{before}|{name}After:{after}";
    }
}