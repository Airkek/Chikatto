using System;
using System.Security.Cryptography.X509Certificates;

namespace Chikatto.Utils
{
    public static class RandomFabric
    {
        private static Random Random = new();

        public static double NextDouble() => Random.NextDouble();
        public static int Next() => Random.Next();
    }
}