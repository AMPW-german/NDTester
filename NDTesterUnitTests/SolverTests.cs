using System.Diagnostics;
using System.Drawing;

namespace NDTester.Tests
{
    [TestClass()]
    public class SolverTests
    {
        [TestMethod()]
        public void Test2d()
        {
            //Console.WriteLine(Solver.UniquePermutations(new List<int> { 3, 3, 3 }));
            //Console.WriteLine(Solver.ObjectPermuations(3));

            //Container con02d = new Container(new double[2] { 20, 20 });
            //Container con12d = new Container(new double[2] { 40, 40 });
            Container con22d = new Container(new double[2] { 2000, 4000 });

            //OrthogonalObject obj02d = new OrthogonalObject(new double[2] { 10, 10 }, 4);
            OrthogonalObject obj12d = new OrthogonalObject(new double[2] { 40, 20 }, 55);
            OrthogonalObject obj22d = new OrthogonalObject(new double[2] { 2, 3 }, 500);
            OrthogonalObject obj32d = new OrthogonalObject(new double[2] { 1, 3 }, 50000);
            OrthogonalObject obj42d = new OrthogonalObject(new double[2] { 5, 5 }, 50000);

            //Container[] containers2d = [con02d, con12d, con22d];
            Container[] containers2d = [con22d];
            //OrthogonalObject[] objects2d = [obj02d, obj12d, obj32d, obj22d, obj42d];
            OrthogonalObject[] objects2d = [obj12d, obj32d, obj22d, obj42d];
            //OrthogonalObject[] objects2d = [obj22d];

            Solver solver2d = new Solver(2, objects2d, containers2d);

            Container conx2d = new Container(new double[2] { 100, 100 });
            Container[] conxs = new Container[] { conx2d };
            OrthogonalObject objx2d = new OrthogonalObject(new double[2] { 1, 1 }, 10000);
            OrthogonalObject[] objxs = new OrthogonalObject[] { objx2d };

            Solver solverx = new Solver(2, objxs, conxs);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            bool result2d = solver2d.solve();

            sw.Stop();
            Console.WriteLine($"Solve time: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");

            if (result2d)
            {
                Assert.IsTrue(true);

                for (int i = 0; i < solver2d.results.Count; i++)
                {
                    SolverResult res = solver2d.results.ElementAt(i).Key;

                    //Console.WriteLine("2d result:");
                    //foreach (OrthogonalObject obj in res.OrthogonalObjects)
                    //{
                    //    foreach (PackedObject packedObj in obj.packedList)
                    //    {
                    //        Console.WriteLine($"Size: {obj.Size[0]}|{obj.Size[1]}, Container: {packedObj.ParentContainer.Size[0]}|{packedObj.ParentContainer.Size[1]}, x|y: {packedObj.Position[0]}|{packedObj.Position[1]}, orientation: {packedObj.Orientation[0]}");
                    //    }
                    //}
                    Display2d(res.Containers, res.OrthogonalObjects, i);
                }
            }
        }

        public void Display2d(Container[] containers, OrthogonalObject[] objects, int num)
        {
            if (!OperatingSystem.IsWindows()) return;

            #pragma warning disable CA1416 // Validate platform compatibility
            Dictionary<Container, Bitmap> bitmaps = new Dictionary<Container, Bitmap>();
            Dictionary<Container, Graphics> graphics = new Dictionary<Container, Graphics>();

            Dictionary<OrthogonalObject, Color> fillColors = new Dictionary<OrthogonalObject, Color>();
            Dictionary<OrthogonalObject, Color> outlineColors = new Dictionary<OrthogonalObject, Color>();

            //Bitmap image = new Bitmap(500, 500);
            //Graphics g = Graphics.FromImage(image);
            //g.Clear(Color.White);

            //g.FillRectangle(Brushes.Black, new RectangleF(0, 0, 500, 200));
            //g.DrawRectangle(Pens.Red, new Rectangle(0, 0, 499, 199));

            int multiplier = 2;

            foreach (Container item in containers)
            {
                Bitmap bitmap = new Bitmap((int)item.Size[0] * multiplier, (int)item.Size[1] * multiplier);
                Graphics gfx = Graphics.FromImage(bitmap);

                gfx.Clear(Color.White);
                bitmaps.Add(item, bitmap);
                graphics.Add(item, gfx);
            }

            Console.WriteLine("2d result:");
            foreach (OrthogonalObject obj in objects)
            {
                Random r = new Random(obj.GetHashCode());

                Color fillColor = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
                Color outlineColor = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));

                fillColors.Add(obj, fillColor);
                outlineColors.Add(obj, outlineColor);

                foreach (PackedObject packedObj in obj.packedList)
                {
                    Console.WriteLine($"Size: {obj.Size[0]}|{obj.Size[1]}, Container: {packedObj.ParentContainer.Size[0]}|{packedObj.ParentContainer.Size[1]}, x|y: {packedObj.Position[0]}|{packedObj.Position[1]}, orientation: {packedObj.Orientation[0]}");

                    Graphics gfx = graphics[packedObj.ParentContainer];

                    gfx.FillRectangle(new SolidBrush(fillColors[obj]), (float)(packedObj.Position[0] * multiplier), (float)(packedObj.Position[1] * multiplier), (float)(PackedObject.GetSize(obj, packedObj.Orientation)[0] * multiplier) - 1, (float)(PackedObject.GetSize(obj, packedObj.Orientation)[1] * multiplier) - 1);
                    gfx.DrawRectangle(new Pen(outlineColors[obj]), (float)(packedObj.Position[0] * multiplier), (float)(packedObj.Position[1] * multiplier), (float)(PackedObject.GetSize(obj, packedObj.Orientation)[0] * multiplier) - 1, (float)(PackedObject.GetSize(obj, packedObj.Orientation)[1] * multiplier) - 1);
                }
            }

            foreach (Container c in containers)
            {
                foreach (PotentialContainer pc in c.potentialContainers)
                {
                    Graphics gfx = graphics[c];
                    gfx.DrawRectangle(Pens.Gray, (float)(pc.Position[0] * multiplier), (float)(pc.Position[1] * multiplier), (float)(pc.Size[0] * multiplier) - 1, (float)(pc.Size[1] * multiplier) - 1);
                    // Merge PCs if possible to allow more objects to be placed
                }
            }

            foreach (KeyValuePair<Container, Bitmap> kvp in bitmaps)
            {
                string savePath = $"C:\\Users\\AMPW\\Pictures\\NDImages\\Container_Combined_2_200x400\\Container_{num}_{(int)kvp.Key.Size[0]}x{(int)kvp.Key.Size[1]}.png";
                kvp.Value.Save(savePath);
            }

            #pragma warning restore CA1416 // Validate platform compatibility
        }

        [TestMethod()]
        public void Test3d()
        {
            Container con03d = new Container(new double[3] { 10, 20, 30 });
            Container con13d = new Container(new double[3] { 40, 40, 40 });
            Container con23d = new Container(new double[3] { 100, 100, 100 });
            Container con33d = new Container(new double[3] { 10000, 10000, 10000 });

            OrthogonalObject obj03d = new OrthogonalObject(new double[3] { 20, 30, 10 }, 2);
            OrthogonalObject obj13d = new OrthogonalObject(new double[3] { 10, 10, 10 }, 10);
            OrthogonalObject obj23d = new OrthogonalObject(new double[3] { 5, 5, 5 }, 350);
            OrthogonalObject obj33d = new OrthogonalObject(new double[3] { 1, 2, 3 }, 10000);
            OrthogonalObject obj43d = new OrthogonalObject(new double[3] { 1, 2, 1 }, 10000);
            OrthogonalObject obj53d = new OrthogonalObject(new double[3] { 1, 2, 2 }, 10000);
            OrthogonalObject obj63d = new OrthogonalObject(new double[3] { 1, 4, 1 }, 10000);
            // Time for 2,000,362 objects: didn't finish in over 45 minutes
            // Time for 20,362 objects: 2.5 seconds
            // Time for 200,362 objects: 1.5 minutes
            // Optimized Time for 200,362 objects: 1.6 minutes

            Container[] containers3d = new Container[] { con03d, con13d, con23d, con33d };
            OrthogonalObject[] objects3d = new OrthogonalObject[] { obj03d, obj13d, obj23d, obj33d, obj43d, obj53d, obj63d };

            Solver solver3d = new Solver(3, objects3d, containers3d);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            bool result3d = solver3d.solve();

            sw.Stop();
            Console.WriteLine($"Solve time: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");

            Assert.IsTrue(result3d);

            /*
            if (result3d)
            {

                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("3d result:");
                //foreach (OrthogonalObject obj in objects3d)
                //{
                //    foreach (PackedObject packedObj in obj.packedList)
                //    {
                //        sb.AppendLine($"Size: {obj.Size[0]}|{obj.Size[1]}|{obj.Size[2]}, Container: {packedObj.ParentContainer.Size[0]}|{packedObj.ParentContainer.Size[1]}|{packedObj.ParentContainer.Size[2]}, x|y|z: {packedObj.Position[0]}|{packedObj.Position[1]}|{packedObj.Position[2]}, orientation: {packedObj.Orientation[0]}|{packedObj.Orientation[1]}|{packedObj.Orientation[2]}");
                //    }
                //}

                //Console.WriteLine(sb.ToString());
            }
            */

            //Assert.Fail();
        }
    }
}