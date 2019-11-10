using System;

namespace LionLibrary.Utils
{
    public class RandomThreadSafe
    {
        private static readonly Random _global = new Random();
        
        [ThreadStatic] 
        private static Random _local;

        public int Next()
        {
            EnsureThreadStaticRandomCreated();
            return _local.Next();
        }

        public int Next(int maxValue)
        {
            EnsureThreadStaticRandomCreated();
            return _local.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            EnsureThreadStaticRandomCreated();
            return _local.Next(minValue, maxValue);
        }

        private void EnsureThreadStaticRandomCreated()
        {
            if (_local == null)
            {
                lock (_global)
                {
                    if (_local == null)
                    {
                        int seed = _global.Next();
                        _local = new Random(seed);
                    }
                }
            }
        }
    }
}
