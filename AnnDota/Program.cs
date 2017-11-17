using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnnDota {
    class Program {
        static void Main(string[ ] args) {
            foreach (var heroName in Utils.Heroes) {
                Console.WriteLine(heroName);
            }
#if DEBUG
            Console.ReadLine( );
#endif
        }
    }
}
