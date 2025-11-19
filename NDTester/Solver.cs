using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class Solver
    {
        public int Dimension { get; private set; }

        public int[] LoadDirection { get; set; }

        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public bool solve()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            Stopwatch containerSw = new Stopwatch();

            //Array.Sort(OrthogonalObjects);
            //Array.Sort(Containers);
            foreach (OrthogonalObject obj in OrthogonalObjects)
            {
                sw.Restart();
                for (int i = obj.PackedCount; i < obj.Count; i++)
                {
                    containerSw.Restart();
                    bool placed = false;
                    foreach (Container c in Containers)
                    {
                        if (obj.Place(c) != null)
                        {
                            placed = true;
                            break;
                        }
                    }
                    if (!placed)
                    {
                        // Failed to place objects, needs to be reworked
                        throw new Exception("Could not place all objects.");
                    }
                    containerSw.Stop();
                    Console.WriteLine($"container pack time: {containerSw.ElapsedMilliseconds} ms, {containerSw.ElapsedTicks} ticks");
                }
                sw.Stop();
                Console.WriteLine($"total count pack time: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");
            }
            return true;
#else
            //Array.Sort(OrthogonalObjects);
            //Array.Sort(Containers);
            foreach (OrthogonalObject obj in OrthogonalObjects)
            {
                for (int i = obj.PackedCount; i < obj.Count; i++)
                {
                    bool placed = false;
                    foreach (Container c in Containers)
                    {
                        if (obj.Place(c) != null)
                        {
                            placed = true;
                            break;
                        }
                    }
                    if (!placed)
                    {
                        // Failed to place objects, needs to be reworked
                        throw new Exception("Could not place all objects.");
                    }
                }
            }
            return true;
#endif
        }

        public Solver(int dimension, OrthogonalObject[] orthogonalObjects, Container[] containers)
        {
            this.OrthogonalObjects = orthogonalObjects;
            this.Containers = containers;
            this.Dimension = dimension;
            this.LoadDirection = Enumerable.Range(1, dimension - 1).ToArray();
        }
    }
}
