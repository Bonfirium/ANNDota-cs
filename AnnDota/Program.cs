using AnnEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnnDota {
    class Program {
        static void Main(string[ ] args) {
            Ann ann = new Ann(230, new uint[ ] { 118, 128, 64, 32, 16, 8, 4, 2 }, 1, 0.8f, 0.4f);
#if DEBUG
            Console.ReadLine( );
#endif
        }
    }
}
