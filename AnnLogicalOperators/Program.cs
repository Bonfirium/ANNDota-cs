using System;
using AnnEngine;

namespace AnnLogicalOperators {
    internal class Program {
        private static bool FunctionForLearning(bool x, bool y, bool z) => ((x & y) | (x & z) | y) & !(x & y & z);
        private const uint VARIABLES = 8;

        public static void Main(string[ ] args) {
            bool[ ] idealResults = new bool[VARIABLES];
            for (uint i = 0; i < VARIABLES; i++) {
                idealResults[i] = FunctionForLearning(
                    Utils.GetBit(i, 0),
                    Utils.GetBit(i, 1),
                    Utils.GetBit(i, 2));
            }

            for (uint i = 0; i < VARIABLES; i++) {
                Console.Write("\t " +
                              Utils.GetBit(i, 0).ToUInt( ) +
                              Utils.GetBit(i, 1).ToUInt( ) +
                              Utils.GetBit(i, 2).ToUInt( ));
            }
            Console.WriteLine( );

            for (uint i = 0; i < VARIABLES; i++) {
                Console.Write("\t  " + idealResults[i].ToUInt( ));
            }
            Console.WriteLine("\n");

            Ann ann = new Ann(3, new[ ] { 4u, 4u }, 1, 0.4f, 0.2f);
            for (uint i = 0; i < VARIABLES; i++) {
                float result = ann.Run(new[ ] {
                    Utils.GetBit(i, 0).ToFloat( ),
                    Utils.GetBit(i, 1).ToFloat( ),
                    Utils.GetBit(i, 2).ToFloat( )
                })[0];
                Console.Write("\t" + (result * 10f).ToString("0.000"));
            }
            Console.WriteLine( );

#if DEBUG
            Console.Write("Press any key to close program");
            Console.ReadKey( );
#endif
        }
    }
}