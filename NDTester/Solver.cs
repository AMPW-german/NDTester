using System.Collections.Concurrent;
using System.Diagnostics;

namespace NDTester
{
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

    public class Solver
    {
        public ConcurrentDictionary<SolverResult, bool> results { get; set; } = new ConcurrentDictionary<SolverResult, bool>();

        public int Dimension { get; private set; }

        public int[] LoadDirection { get; set; }

        public OrthogonalObject[] OrthogonalObjects { get; private set; }
        public Container[] Containers { get; private set; }

        public static ulong UniquePermutations(List<int> ObjectCounts)
        {
            ulong count = (ulong)ObjectCounts.Sum();
            ulong res = 1;
            for (ulong i = 2; i <= count; i++) res *= i;

            ulong objectCount = (ulong)ObjectCounts.Count();
            ulong divider = 1;
            for (ulong i = 2; i <= objectCount; i++) divider *= i;
            ulong divider2 = divider;
            for (ulong i = 1; i < objectCount; i++) divider *= divider2;

            return res / divider;
        }

        public static ulong ObjectPermuations(int objectCount) => (ulong)Enumerable.Range(1, objectCount).Aggregate(1, (a, b) => a * b);

        public bool solve(ulong maxPermutations = ulong.MaxValue)
        {
            // Diffrent solve methods should be implemented with multi threading
            // Results will compare empty volume after packing with the empty volume in load direction (aka the empty space at the "top" of the container) to account for enclosed empty space that is wasted

            // Unique permutations are not solved yet
            ulong permutations = Solver.ObjectPermuations(OrthogonalObjects.Length);

            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            if (permutations < maxPermutations)
            {
                Console.WriteLine($"Permutation count: {permutations}");

                int initialCapacity = (int)permutations;
                results = new ConcurrentDictionary<SolverResult, bool>(concurrencyLevel, initialCapacity);

                int objectCount = OrthogonalObjects.Length;
                int[] counter = new int[objectCount];

                Task[] tasks = new Task[permutations];
                int taskCount = 0;

                // Make deep copies for thread safety
                OrthogonalObject[] orthogonalObjectsCopy = new OrthogonalObject[objectCount];
                for (int j = 0; j < objectCount; j++)
                    orthogonalObjectsCopy[j] = OrthogonalObjects[j].EmptyCopy();

                Container[] containersCopy = new Container[Containers.Length];
                for (int j = 0; j < Containers.Length; j++)
                    containersCopy[j] = Containers[j].EmptyCopy();

                tasks[taskCount] = new Task(() => solverThread(orthogonalObjectsCopy, containersCopy));
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

                        tasks[taskCount] = new Task(() => solverThread(orthogonalObjectsCopy2, containersCopy2));
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

                foreach (Task t in tasks)
                    t.Start();

                Task.WaitAll(tasks);
                Console.WriteLine("All tasks finished");
            }
            return true;
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
        
        public bool solverThread(OrthogonalObject[] OrthogonalObjects, Container[] Containers)
        {
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
