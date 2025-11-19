namespace NDTesterTest
{
    internal class Program
    {
        static int dimension = 2;
        static double[] sizes = new double[] { 2, 3 };
        static bool[] orientation = new bool[] { true };

        public static double[] GetSize()
        {
            void intToDim(int i, out int a, out int b)
            {
                int count = 0;

                a = 0;
                b = 1;

                for (a = 0; a < dimension - 1; a++)
                {
                    for (b = a + 1; b < dimension; b++)
                    {
                        if (count == i)
                        {
                            return;
                        }
                        count++;
                    }
                }
            }

            int n = dimension;
            int length = n * (n - 1) / 2;
            double[] size = new double[n];
            Array.Copy(sizes, size, n);

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

        static void Main(string[] args)
        {
            double[] res = GetSize();
            Console.WriteLine(string.Join(", ", res));
            Console.ReadLine();
        }
    }
}
