using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class OrthogonalObject
    {
        internal int dimension { get; private set; }
        public double[] Size { get; private set; }

        public int Count { get; set; }
        public int PackedCount { get; set; }
        public List<PackedObject> packedList { get; set; }

        public PackedObject? Place(Container c)
        {
            if (PackedCount >= Count) return null;

            double Volume(double[] size) => size.Aggregate(1.0, (a, b) => a * b);

            double objectVolume = Volume(this.Size);

            int orientationLength = dimension * (dimension - 1) / 2;

            Dictionary<PotentialContainer, BitArray> potentials = new();

            c.potentialContainers.Sort();
            foreach (PotentialContainer pc in c.potentialContainers)
            {
                if (Volume(pc.Size) >= objectVolume)
                {

                    BitArray orientation = new BitArray(orientationLength);
                    ulong orientationCount = 0; 
                    // max dimension is 11 => max orientations 55 < 64
                    // with a different approach for the permutations this could be extended further

                    bool canFit = false;

                    while (true)
                    {
                        bool canFitLocal = true;

                        double[] size = PackedObject.GetSize(this, orientation);

                        for (int i = 0; i < dimension; i++)
                        {
                            if (size[i] > pc.Size[i])
                            {
                                canFitLocal = false;
                                break;
                            }
                        }

                        if (canFitLocal)
                        {
                            canFit = true;
                            break;
                        }
                        else
                        {
                            if (orientation.HasAllSet()) break;

                            orientationCount++;
                            for (int i = 0; i < orientationLength; i++) orientation[i] = (orientationCount & (1ul << i)) != 0;
                        }
                    }

                    if (canFit)
                    {
                        potentials.Add(pc, orientation);
                        break;
                    }
                }
            }

            if (potentials.Count > 0)
            {

                // minimum pc size -> leaves most remaining space for other objects but doesn't use the placement direction
                // KeyValuePair<PotentialContainer, BitArray> kvp = potentials.MinBy(kvp => Volume(kvp.Key.Size));

                // first fit -> uses the placement direction although it can lead to worse overall packing but that's where the object placement string allows for different iterations
                KeyValuePair<PotentialContainer, BitArray> kvp = potentials.First();

                c.PlaceObject(kvp.Key, PackedObject.GetSize(this, kvp.Value));
                PackedObject obj = new PackedObject(this, c, kvp.Key.Position, kvp.Value);
                packedList.Add(obj);
                PackedCount++;
                return obj;
            }

            return null;
        }

        public OrthogonalObject(double[] Size, int Count)
        {
            this.Size = Size;
            this.dimension = Size.Length;
            this.Count = Count;

            packedList = new List<PackedObject>();
        }
    }
}
