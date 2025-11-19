using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDTester
{
    public class PotentialContainer : IComparable<PotentialContainer>, IComparer<PotentialContainer>
    {
        #region sorting
        public int CompareTo(PotentialContainer? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            else if (ReferenceEquals(this, null)) return 1;
            else if (ReferenceEquals(other, null)) return -1;

            for (int i = 0; i < Position.Length; i++)
                {
                    if (Position[i] < other.Position[i]) return -1;
                    if (Position[i] > other.Position[i]) return 1;
                }
            return 0;
        }

        public int Compare(PotentialContainer? x, PotentialContainer? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            else if (ReferenceEquals(x, null)) return 1;
            else if (ReferenceEquals(y, null)) return -1;

            for (int i = 0; i < x.Position.Length; i++)
            {
                if (x.Position[i] < y.Position[i]) return -1;
                if (x.Position[i] > y.Position[i]) return 1;
            }
            return 0;
        }
        #endregion

        public double[] Size { get; private set; }
        public double[] Position { get; private set; }

        public double Volume => Size.Aggregate(1.0, (a, b) => a * b);

        public PotentialContainer(double[] size, double[] position)
        {
            Size = size;
            Position = position;
        }
    }
}
