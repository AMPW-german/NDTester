using NDTester;

namespace NDTesterUnitTests
{
    [TestClass]
    public sealed class TestPCSetPlacement
    {
        [TestMethod]
        public void TestMethod()
        {
            double[] pcSize = new double[] { 10.0, 10.0 };

            Container con = new Container(pcSize);
            PotentialContainer initialPC = con.potentialContainers[0];

            double[] objectSize = new double[] { 10.0, 2.0 };

            con.PlaceObject(con.potentialContainers[0], objectSize);
            //con.PlaceObject(con.potentialContainers[0], objectSize);

            // Print sizes and positions of new potential containers
            foreach (var pc in con.potentialContainers)
            {
                string sizeStr = string.Join(", ", pc.Size);
                string posStr = string.Join(", ", pc.Position);
                Console.WriteLine($"Size: [{sizeStr}]  Position: [{posStr}]");
            }

            // Assert
            Assert.IsFalse(con.potentialContainers.Contains(initialPC), "Original PotentialContainer should be removed.");
            Assert.IsTrue(con.potentialContainers.Count > 0, "New PotentialContainers should be added.");

        }
    }
}
