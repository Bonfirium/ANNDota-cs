using System;
using AnnEngine;

namespace AnnDemo {
    internal class Program {
        private static bool FunctionForLearning(bool x, bool y, bool z) => ((x & y) | (x & z) | y) & !(x & y & z);
        private const uint VARIABLES = 8;
        private const uint INTERVAL_OF_NULL_RESULT = 1000u;
        private const float NULL_RESULT_SIZE = 0.0001f;

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

            Utils.Randomize( );
            //Utils.SetRandomSeed(100);

            Ann ann = new Ann(3, new[ ] { 6u, 3u }, 1, 0.8f, 0.4f);
            float maxError;
            float maxErrorAtPreviousStep = 1f;
            uint previousStepIndex = 0u;
            bool previousStepIsSetted = false;
            uint testNumber = 0u;
            do {
                testNumber++;
                string output = testNumber.ToString( ) + ".\t";
                maxError = 0f;
                for (uint i = 0; i < VARIABLES; i++) {
                    AnnResult result = ann.Learn(new[ ] {
                        Utils.GetBit(i, 0).ToFloat( ),
                        Utils.GetBit(i, 1).ToFloat( ),
                        Utils.GetBit(i, 2).ToFloat( )
                    }, new[ ] { idealResultsFloat[i] });
                    output += (result.Result[0]).ToString(".0000") + "\t";
                    maxError = Math.Max(maxError, result.Error);
                }
                if (!previousStepIsSetted) {
                    previousStepIndex = testNumber;
                    maxErrorAtPreviousStep = maxError;
                } else if (testNumber - previousStepIndex == INTERVAL_OF_NULL_RESULT) {
                    if (Math.Abs(maxError - maxErrorAtPreviousStep) < NULL_RESULT_SIZE) {
                        Console.WriteLine(output);
                        Console.WriteLine("Not learned ANN. Please, change the start weights!");
                        break;
                    } else {
                        previousStepIndex = testNumber;
                        maxErrorAtPreviousStep = maxError;
                    }
                }
                output += "\tERROR = " + (maxError * 100f).ToString("00.0000") + "%";
                if (testNumber % 100 == 0 || maxError <= 0.01f) {
                    Console.WriteLine(output);
                }
            } while (maxError > 0.01f);

#if DEBUG
            Console.Write("Press any key to close program");
            Console.ReadKey( );
#endif
        }
    }
}