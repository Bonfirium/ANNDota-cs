using System;
using AnnEngine;

namespace AnnDemo {
    internal class Program {
        private static bool FunctionForLearning(bool x, bool y, bool z) => ((x & y) | (x & z) | y) & !(x & y & z);
        private const uint VARIABLES = 8;

        public static void Main(string[ ] args) {
            bool[ ] idealResults = new bool[VARIABLES];
            float[ ] idealResultsFloat = new float[VARIABLES];
            for (uint i = 0; i < VARIABLES; i++) {
                idealResults[i] = FunctionForLearning(
                    Utils.GetBit(i, 0),
                    Utils.GetBit(i, 1),
                    Utils.GetBit(i, 2));
                idealResultsFloat[i] = idealResults[i].ToFloat( );
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

            Utils.SetRandomSeed(2);

            Ann ann = new Ann(3, new[ ] { 4u, 4u }, 1, 2.2f, 0.9f);
            float maxError;
            uint testNumber = 0;
            do {
                Console.Write(testNumber++ + ".\t");
                maxError = 0f;
                for (uint i = 0; i < VARIABLES; i++) {
                    AnnResult result = ann.Learn(new[ ] {
                        Utils.GetBit(i, 0).ToFloat( ),
                        Utils.GetBit(i, 1).ToFloat( ),
                        Utils.GetBit(i, 2).ToFloat( )
                    }, new[ ] { idealResultsFloat[i] });
                    Console.Write("" + (result.Result[0]).ToString(".0000") + "\t");
                    maxError = Math.Max(maxError, result.Error);
                }
                Console.Write("\tERROR = " + (maxError * 100f).ToString("00.0000") + "%");
                Console.WriteLine( );
            } while (maxError > 0.01f);

#if DEBUG
            Console.Write("Press any key to close program");
            Console.ReadKey( );
#endif
        }
    }
}