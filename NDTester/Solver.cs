using System.Collections.Concurrent;

namespace NDTester
{
    internal static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }

    public class SolverResult
    {
        public bool valid;

        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public SolverResult(bool valid, OrthogonalObject[] orthogonalObjects, Container[] containers)
        {
            this.valid = valid;
            OrthogonalObjects = orthogonalObjects;
            Containers = containers;
        }
    }

    public class PermutationInfo
    {
        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public PermutationInfo(OrthogonalObject[] orthogonalObjects, Container[] containers)
        {
            OrthogonalObjects = orthogonalObjects.ToArray();
            Containers = containers.ToArray();
        }
    }

    public class Solver
    {
        public bool result { get; set; }
        public ConcurrentDictionary<SolverResult, bool> results { get; set; } = new ConcurrentDictionary<SolverResult, bool>();

        public int Dimension { get; private set; }

        public int[] LoadDirection { get; set; }

        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public static int UniquePermutations(List<int> ObjectCounts)
        {
            int count = ObjectCounts.Sum();
            int res = 1;
            for (int i = 2; i <= count; i++) res *= i;

            int objectCount = ObjectCounts.Count();
            int divider = 1;
            for (int i = 2; i <= objectCount; i++) divider *= i;
            int divider2 = divider;
            for (int i = 1; i < objectCount; i++) divider *= divider2;

            return res / divider;
        }

        public static int ObjectPermutations(int objectCount) => Enumerable.Range(1, objectCount).Aggregate(1, (a, b) => a * b);

        public bool solve(int maxPermutations = 8192, int maxProcessors = 0, bool stopEarly = true)
        {
            // Different solve methods should be implemented with multi threading
            // Results can compare empty volume after packing with the empty volume in load direction (aka the empty space at the "top" of the container) to account for enclosed empty space that is wasted
            // Results comparison is not scope of this project as it's only intended to find out if packing is possible, not how well it packs
            // Changing the container order also adds possible permutations but that would increase the permutation count too much

            // Unique permutations are not solved yet
            int permutations = Solver.ObjectPermutations(OrthogonalObjects.Length);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxProcessors > 0 ? maxProcessors : Environment.ProcessorCount
            };

            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            result = false;

            PermutationInfo[] permutationList;

            if (permutations < maxPermutations)
            {
                permutationList = new PermutationInfo[permutations];
                //Console.WriteLine("Permutation count less than max permutations, using all permutations.");

                int initialCapacity = permutations;
                results = new ConcurrentDictionary<SolverResult, bool>(concurrencyLevel, initialCapacity);

                int objectCount = OrthogonalObjects.Length;
                int[] counter = new int[objectCount];

                int taskCount = 0;

                // Make deep copies for thread safety
                OrthogonalObject[] orthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    orthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();

                Container[] containersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    containersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(orthogonalObjectsCopy, containersCopy);
                taskCount++;

                int i = 0;

                while (i < objectCount)
                {
                    if (counter[i] < i)
                    {
                        if (i % 2 == 0) (OrthogonalObjects[0], OrthogonalObjects[i]) = (OrthogonalObjects[i], OrthogonalObjects[0]);
                        else (OrthogonalObjects[counter[i]], OrthogonalObjects[i]) = (OrthogonalObjects[i], OrthogonalObjects[counter[i]]);

                        // Make deep copies for thread safety
                        OrthogonalObject[] orthogonalObjectsCopy2 = new OrthogonalObject[objectCount]; // Somehow it's necessary to make a new variable here instead of reusing the previous one
                        for (int j = 0; j < objectCount; j++)
                            orthogonalObjectsCopy2[j] = OrthogonalObjects[j].EmptyCopy();

                        Container[] containersCopy2 = new Container[Containers.Length];
                        for (int j = 0; j < Containers.Length; j++)
                            containersCopy2[j] = Containers[j].EmptyCopy();

                        permutationList[taskCount] = new PermutationInfo(orthogonalObjectsCopy2, containersCopy2);
                        taskCount++;

                        counter[i]++;
                        i = 0;
                    }
                    else
                    {
                        counter[i] = 0;
                        i++;
                    }
                }
            }
            else
            {
                //Console.WriteLine("Permutation count higher than max permutations, generating max permutations");

                // TODO: Add partially random permutations for large permutation counts
                // Must be included (if enough max permutations):
                // Default order
                // Reverse order
                // Largest to smallest (volume, primary dimension)
                // Smallest to largest

                results = new ConcurrentDictionary<SolverResult, bool>(concurrencyLevel, maxPermutations);

                permutationList = new PermutationInfo[maxPermutations];

                int taskCount = 0;
                int objectCount = OrthogonalObjects.Length;

                #region defaultOrder
                OrthogonalObject[] orthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    orthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();

                Container[] containersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    containersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(orthogonalObjectsCopy, containersCopy);
                taskCount++;
                #endregion

                #region reverseOrder
                OrthogonalObject[] reverseOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    reverseOrthogonalObjectsCopy[j] = OrthogonalObjects[objectCount - 1 - j].EmptyCopy();

                Container[] reverseContainersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    reverseContainersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(reverseOrthogonalObjectsCopy, reverseContainersCopy);
                taskCount++;
                #endregion

                #region largestToSmallestVolume
                OrthogonalObject[] sortedVolumeOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    sortedVolumeOrthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();
                Array.Sort(sortedVolumeOrthogonalObjectsCopy, (a, b) => OrthogonalObject.Volume(a.Size).CompareTo(OrthogonalObject.Volume(b.Size)));

                Container[] sortedVolumeContainersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    sortedVolumeContainersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(sortedVolumeOrthogonalObjectsCopy, sortedVolumeContainersCopy);
                taskCount++;
                #endregion

                #region smallestToLargestVolume
                OrthogonalObject[] smallestSortedVolumeOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    smallestSortedVolumeOrthogonalObjectsCopy[j] = sortedVolumeOrthogonalObjectsCopy[objectCount - 1 - j].EmptyCopy();

                Container[] smallestSorterVolumeContainersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    smallestSorterVolumeContainersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(smallestSortedVolumeOrthogonalObjectsCopy, smallestSorterVolumeContainersCopy);
                taskCount++;
                #endregion

                #region largestToSmallestPrimary
                OrthogonalObject[] sortedPrimaryOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    sortedPrimaryOrthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();
                Array.Sort(sortedPrimaryOrthogonalObjectsCopy, (a, b) => a.Size[0].CompareTo(b.Size[0]));

                Container[] sortedPrimaryContainersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    sortedPrimaryContainersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(sortedPrimaryOrthogonalObjectsCopy, sortedPrimaryContainersCopy);
                taskCount++;
                #endregion

                #region smallestToLargestPrimary
                OrthogonalObject[] smallestSortedPrimaryOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    smallestSortedPrimaryOrthogonalObjectsCopy[j] = sortedPrimaryOrthogonalObjectsCopy[objectCount - 1 - j].EmptyCopy();

                Container[] smallestSortedPrimaryContainersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    smallestSortedPrimaryContainersCopy[j] = Containers[j].EmptyCopy();

                permutationList[taskCount] = new PermutationInfo(smallestSortedPrimaryOrthogonalObjectsCopy, smallestSortedPrimaryContainersCopy);
                taskCount++;
                #endregion

                #region randomOrder
                Random r = new Random();
                while (taskCount < maxPermutations)
                {
                    OrthogonalObject[] rngOrthogonalObjectsCopy = new OrthogonalObject[objectCount];
                    for (int j = 0; j < objectCount; j++)
                        rngOrthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();
                    r.Shuffle(rngOrthogonalObjectsCopy);

                    Container[] rngContainerCopy = new Container[Containers.Length];
                    for (int j = 0; j < Containers.Length; j++)
                        smallestSortedPrimaryContainersCopy[j] = Containers[j].EmptyCopy();

                    permutationList[taskCount] = new PermutationInfo(rngOrthogonalObjectsCopy, rngContainerCopy);
                    taskCount++;
                }
                #endregion
            }

            Parallel.ForEach(permutationList, parallelOptions, (item, state) =>
            {
                if (result && stopEarly)
                {
                    state.Stop();
                    return;
                }

                if (solverThread(item))
                {
                    result = true;
                    if (stopEarly) state.Stop();
                }
            });

            return results.Any(kvp => kvp.Value);
        }

        public bool solverThread(PermutationInfo info)
        {
            OrthogonalObject[] OrthogonalObjects = info.OrthogonalObjects;
            Container[] Containers = info.Containers;

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
                        SolverResult res = new SolverResult(false, OrthogonalObjects, Containers);
                        results.TryAdd(res, false);
                        return false;

                        // Failed to place objects, needs to be reworked
                        //throw new Exception("Could not place all objects.");
                    }
                }
            }

            SolverResult finalRes = new SolverResult(true, OrthogonalObjects, Containers);
            results.TryAdd(finalRes, true);

            return true;
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
