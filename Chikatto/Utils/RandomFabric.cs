using System;
using System.Threading;

namespace Chikatto.Utils
{
    public static class RandomFabric
    {
        private static readonly Random Random = new();
        private static readonly object Locker = new();

        public static double NextDouble()
        {
            double res;
            
            Monitor.TryEnter(Locker);
            try
            {
                res = Random.NextDouble();
            }
            finally
            {
                Monitor.Exit(Locker);
            }

            return res;
        }

        public static int Next()
        {
            int res;
            
            Monitor.TryEnter(Locker);
            try
            {
                res = Random.Next();
            }
            finally
            {
                Monitor.Exit(Locker);
            }

            return res;
        }

        public static int Next(int max)
        {
            int res;
            
            Monitor.TryEnter(Locker);
            try
            {
                res = Random.Next(max);
            }
            finally
            {
                Monitor.Exit(Locker);
            }

            return res;
        }
        
        public static int Next(int min, int max)
        {
            int res;
            
            Monitor.TryEnter(Locker);
            try
            {
                res = Random.Next(min, max);
            }
            finally
            {
                Monitor.Exit(Locker);
            }

            return res;
        }
    }
}