using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class Solver
    {
        public int Dimension { get; private set; }

        public int[] LoadDirection => Enumerable.Range(1, Dimension-1).ToArray();

        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public Solver(int dimension, OrthogonalObject[] orthogonalObjects, Container[] containers)
        {
            this.OrthogonalObjects = orthogonalObjects;
            this.Containers = containers;
            this.Dimension = dimension;
        }
    }
}
