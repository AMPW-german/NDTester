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

            CombinePotentialContainers();
        }

        public void CombinePotentialContainers()
        {
            // Combine adjecent potential containers
            int containerCount = potentialContainers.Count;
            int testedCount = 0;
            while (testedCount < containerCount)
            {
                for (int i = testedCount; i < potentialContainers.Count; i++)
                {
                    bool canCombine = false;
                    int combineDim = -1;
                    // d - 1 sizes must be equal, 1 size can be different

                    PotentialContainer currentPC = potentialContainers.ElementAt(testedCount);
                    PotentialContainer testPC = potentialContainers.ElementAt(i);

                    // PCs can be combined if they are NOT adjacent in only one dimension and their sizes are equal in all other dimensions
                    for (int d = 0; d < Dimension; d++)
                    {
                        // Check if both PCs are adjacent in this dimension
                        if (currentPC.Position[d] + currentPC.Size[d] == testPC.Position[d] ||
                            testPC.Position[d] + testPC.Size[d] == currentPC.Position[d])
                        {
                            combineDim = d;
                            canCombine = true;

                            for (int sizeD = 0; sizeD < Dimension; sizeD++)
                            {
                                if (sizeD != d)
                                {
                                    if (currentPC.Size[sizeD] != testPC.Size[sizeD])
                                    {
                                        canCombine = false;
                                        combineDim = -1;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (canCombine)
                    {
                        Console.WriteLine($"Combining PCs:\n\tPC 0: {string.Join(',', currentPC.Size)}; {string.Join(',', currentPC.Position)}\n\tPC 1: {string.Join(',', testPC.Size)}; {string.Join(',', testPC.Position)}");

                        testedCount--;
                        containerCount--;
                        currentPC.Size[combineDim] += testPC.Size[combineDim];
                        currentPC.Position[combineDim] = Math.Min(currentPC.Position[combineDim], testPC.Position[combineDim]);
                        potentialContainers.Remove(testPC);
                        break;
                    }
                }
                testedCount++;
            }

            potentialContainers.Order();
        }

        public Container EmptyCopy()
        {
            double[] arr = new double[this.Size.Length];
            Array.Copy(this.Size, arr, this.Size.Length);
            Container con = new Container(arr);
            return con;
        }

        public Container(double[] Size)
        {
            this.Size = Size;
            this.Dimension = Size.Length;

            potentialContainers = new SortedSet<PotentialContainer> { new PotentialContainer(Size, new double[Dimension]) };
            PackedObjects = new SortedSet<PackedObject>();
        }
    }
}
