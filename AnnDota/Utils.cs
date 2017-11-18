using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnnDota {
    public static class Utils {
        public static readonly IReadOnlyDictionary<uint, uint> HeroesDictionary;
        public static readonly string[ ] Heroes;

        static Utils( ) {
            string[ ] heroes = File.ReadAllLines("Resources/Heroes.txt");
            Dictionary<uint, uint> result = new Dictionary<uint, uint>( );
            foreach (string hero in heroes) {
                string[ ] split = hero.Split(':');
                result[uint.Parse(split[1])] = uint.Parse(split[0]);
            }
            HeroesDictionary = result;
        }
    }
}
