using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnnDota {
    public static class Utils {
        public static readonly string[ ] Heroes;

        static Utils( ) {
            var heroes = File.ReadAllText("Resources/Heroes.txt");
            Heroes = heroes.Split(':');
        }
    }
}
