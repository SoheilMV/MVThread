using System;
using System.Security.Cryptography;

namespace MVThread
{
    internal class SecureRandom
    {
        private Random _random;

        public SecureRandom()
        {
            int seed = BytesToInt(SHA512(NextBytes(32)));
            _random = new Random(seed);
        }

        public int Next()
        {
            return _random.Next();
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public byte[] NextBytes(int length)
        {
            byte[] buffer = new byte[length];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        public void NextBytes(byte[] buffer)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
        }

        private int BytesToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        private byte[] SHA512(byte[] bytes)
        {
            using (SHA512 sha512 = SHA512Managed.Create())
            {
                return sha512.ComputeHash(bytes);
            }
        }
    }
}
