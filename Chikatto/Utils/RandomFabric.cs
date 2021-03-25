using System;
using System.Security.Cryptography.X509Certificates;

namespace Chikatto.Utils
{
    public static class RandomFabric
    {
        public static Random Random = new();
        
        public static string GenerateBanchoToken()
        {
            return "chikatto-1337"; //TODO
        }
    }
}