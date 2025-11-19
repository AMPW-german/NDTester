using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class Container
    {
        public int Dimension { get; private set; }
        public double[] Size { get; private set; }

        internal SortedSet<PackedObject> PackedObjects { get; private set; }

        public SortedSet<PotentialContainer> potentialContainers;

        public void PlaceObject(PotentialContainer pc, double[] size) // This enforces an increasing axis placement direction
        {
            double Volume(double[] size) => size.Aggregate(1.0, (a, b) => a * b);

            potentialContainers.Remove(pc);
            double[] maxPos = pc.Size.ToArray();


            List<PotentialContainer> newContainers = new List<PotentialContainer>();
            int containerCount = 0;

            while (containerCount < Dimension)
            {

                // Save the position and create new PC afterwards
                double[] PCSize = new double[Dimension];
                int primaryDimension = 0;

                for (int i = 0; i < Dimension; i++)
                {
                    double[] newPCSize = maxPos.ToArray();
                    newPCSize[i] -= size[i];

                    if (Volume(newPCSize) > Volume(PCSize))
                    {
                        PCSize = newPCSize.ToArray();
                        primaryDimension = i;
                        break;
                    }
                }

                if (Volume(PCSize) == 0.0)
                {
                    containerCount++;
                    continue;
                }

                double[] newPCPos = pc.Position.ToArray();
                newPCPos[primaryDimension] += size[primaryDimension];

                potentialContainers.Add(new PotentialContainer(PCSize, newPCPos));
                containerCount++;

                maxPos[primaryDimension] = size[primaryDimension];
            }

            //potentialContainers.AddRange(newContainers);
        }

        public Container(double[] Size)
        {
            this.Size = Size;
            this.Dimension = Size.Length;

            potentialContainers = [ new PotentialContainer(Size, new double[Dimension])];
            PackedObjects = new SortedSet<PackedObject>();
        }
    }
}
