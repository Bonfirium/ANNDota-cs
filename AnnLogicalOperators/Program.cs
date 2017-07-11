using System;
using System.Collections.Generic;
using AnnEngine;

namespace AnnLogicalOperators {
    internal class Program {
        public static void Main(string[ ] args) {
            Ann ann = new Ann(3, new[ ] { 4u, 4u }, 1, 0.4f, 0.2f);
            ann.Run(new float[ ] { 1, 1, 0 });
        }
    }
}