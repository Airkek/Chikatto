using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Chikatto.Enums;
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

        public static string ChartEntry(string name, string before, string after) => $"{name}Before:{before}|{name}After:{after}";

        public static async Task<Score> ScoreDataToScore(string[] scoreData)
        {
            //0           1        2                    3        4        5       6         7         8         9     10    11      12   13   14        15       16   17
            //beatmapHash:username:chickenmcnuggetshash:count300:count100:count50:countGeki:countKatu:countMiss:score:combo:perfect:rank:mods:completed:playmode:date:version

            var mods = (Mods) int.Parse(scoreData[13]);
            var score = new Score
            {
                BeatmapChecksum = scoreData[0],
                Count300 = int.Parse(scoreData[3]),
                Count100 = int.Parse(scoreData[4]),
                Count50 = int.Parse(scoreData[5]),
                CountGeki = int.Parse(scoreData[6]),
                CountKatu = int.Parse(scoreData[7]),
                CountMiss = int.Parse(scoreData[8]),
                GameScore = int.Parse(scoreData[9]),
                MaxCombo = int.Parse(scoreData[10]),
                Perfect = scoreData[11] == "True",
                Mods = mods,
                Completed = scoreData[14] == "True",
                PlayMode = (GameMode) int.Parse(scoreData[15]),
                IsRelax = (mods & Mods.Relax) != 0,
                Time = DateTime.UtcNow,
            };

            var acc = (score.Count300 * 300f + score.Count100 * 100f + score.Count50 * 50f) 
                / ((score.Count300 + score.Count100 + score.Count50 + score.CountMiss) * 300f) 
                * 100f;

            score.Accuracy = acc;

            return score;
        }
    }
}