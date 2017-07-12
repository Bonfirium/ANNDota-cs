using System;

namespace AnnEngine {
    public static class Utils {
        private static Random _random = new Random( );

        public static float Random( ) => (float) _random.NextDouble( );
        public static float Random(float maxValue) => Random( ) * maxValue;
        public static float Random(float minValue, float maxValue) => Random(maxValue - minValue) + minValue;

        public static void SetRandomSeed(int seed) {
            _random = new Random(seed);
        }

        public static void Randomize( ) {
            _random = new Random( );
        }

        public static bool GetBit(uint number, int bitNumber) => (number >> bitNumber) % 2 == 1;
        public static uint ToUInt(this bool value) => value ? 1u : 0u;
        public static float ToFloat(this bool value) => value ? 1f : 0f;
    }
}