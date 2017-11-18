using AnnEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnnDota {
    class Program {

        public const string PATH_TO_DATASET = @"E:\Dev\AIHackaton\DataSet\";

        // 7008983
        // 3560395823
        // 3553386840
        // 3546377857

        static void Main(string[ ] args) {
            //string learningProgress = "";
            //string testingProgress = "";
            string[ ] files = Directory.GetFiles(PATH_TO_DATASET);
            //Ann ann = new Ann(230, new uint[ ] { 118, 128, 64, 32, 16, 8, 4, 2 }, 1, 0.7f, 0.3f);
            Ann ann = new Ann(230, new uint[ ] { 230 }, 1, 0.7f, 0.3f);
            uint testNumber = 0u;
            uint iterLength = 1000u;
            uint iter = 0u;
            uint trues = 0u;
            for (uint k = 0; k < 3; k++) {
                Console.WriteLine(k < 2 ? "Learning (Step " + (k + 1) + " of 2)..." : "Testing...");
                foreach (string fileName in files) {
                    float[ ] inputs = new float[230];
                    string[ ] lines = File.ReadAllLines(fileName);
                    for (uint i = 0; i < 10; i++) {
                        inputs[(i < 5 ? 0u : 115u) +
                            Utils.HeroesDictionary[uint.Parse(lines[i].Split(':')[0])]] = 0.5f;
                    }
                    float[ ] outputs = new float[ ] {
                        0.5f + (uint.Parse(lines[10]) == 0 ? 1 : -1 ) *
                        (float.Parse(lines[11].Replace('.', ',')) - 0.5f)
                    };
                    AnnResult res;
                    if (k < 2) {
                        res = ann.Learn(inputs, outputs);
                    } else {
                        float[ ] answer = ann.Run(inputs);
                        float error = Ann.GetError(answer, outputs);
                        res = new AnnResult(answer, error);
                    }
                    if (Math.Round(res.Result[0]) == Math.Round(outputs[0])) {
                        trues++;
                    }
                    testNumber++;
                    if (testNumber % iterLength == 0) {
                        iter++;
                        float percents = (float)trues / testNumber * 100.0f;
                        Console.WriteLine("{0}000.   {1}%", String.Format("{0,3:0}", iter),
                            percents.ToString((k < 2 ? "00.00" : "00.0000")));
                        if (k == 0) {
                            trues = 0u;
                            testNumber = 0;
                        }
                    }
                }
                if (k < 2) {
                    trues = 0;
                }
                iter = 0;
                testNumber = 0;
            }
            Console.WriteLine( );
            Console.WriteLine("ACCURACY: {0}%", ((float)trues / files.Length * 100.0).ToString("00.000"));
#if DEBUG
            Console.ReadLine( );
#endif
        }
    }
}
