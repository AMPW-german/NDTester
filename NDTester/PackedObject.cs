using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class PackedObject
    {
        public OrthogonalObject OrthogonalType { get; private set; }
        public double[] Position {  get; set; }
        /// <summary>
        /// Rotation by 90° in the corresponding dimension.
        /// Since these are parallelepipeds there are only 2 possible orientations per axis which can be represented with a bool.
        /// The length for the orientation vector is n * (n-1) / 2
        /// The rotation planes are ordered lexicographically, e.g. n = 4:
        /// (0|1), (0|2), (0|3), (1|2), (1|3), (2|3)
        /// </summary>
        public BitArray Orientation { get; set; }
        public Container ParentContainer { get; private set; }

        public static double[] GetSize(OrthogonalObject obj, BitArray orientation)
        {
            void intToDim(int i, out int a, out int b)
            {
                int count = 0;

                a = 0;
                b = 1;

                for (a = 0; a < obj.dimension - 1; a++)
                {
                    for (b = a + 1; b < obj.dimension; b++)
                    {
                        if (count == i)
                        {
                            return;
                        }
                        count++;
                    }
                }
            }

            int n = obj.dimension;
            int length = n * (n - 1) / 2;
            double[] size = new double[n];
            Array.Copy(obj.Size, size, n);

            for (int i = 0; i < length; i++)
            {
                if (orientation[i])
                {
                    intToDim(i, out int a, out int b);
                    (size[b], size[a]) = (size[a], size[b]);
                }
            }

            return size;
        }

        public PackedObject(OrthogonalObject Type, Container ParentContainer, double[] Position, BitArray orientation)
        {
            this.OrthogonalType = Type;
            this.ParentContainer = ParentContainer;
            this.Position = Position;
            int n = Type.dimension;
            this.Orientation = orientation;
        }
    }
}
