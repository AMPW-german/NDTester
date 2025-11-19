using NDTester;
using System.Text;

namespace NDTesterUnitTests
{
    [TestClass]
    public sealed class TestOrthogonalObjectPlacement
    {
        [TestMethod]
        public void TestMethod()
        {
            double[] objSize = new double[] { 8.0, 5.0 };
            OrthogonalObject orthogonalObj = new OrthogonalObject(objSize, 1);
            orthogonalObj.Count = 1;

            double[] objSize2 = new double[] { 2.0, 5.0 };
            OrthogonalObject orthogonalObj2 = new OrthogonalObject(objSize2, 1);
            orthogonalObj2.Count = 1;

            double[] pcSize = new double[] { 5.0, 10.0 };
            Container con = new Container(pcSize);

            PackedObject packedObj = orthogonalObj.Place(con);
            PackedObject packedObj2 = orthogonalObj2.Place(con);

            if (packedObj == null || packedObj2 == null)
            {
                Assert.Fail("Object could not be placed in the container.");
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Packed Object Details:");
            stringBuilder.AppendLine($"Position: [{string.Join(", ", packedObj.Position)}]");
            stringBuilder.AppendLine($"Orientation: [{string.Join(", ", packedObj.Orientation.Cast<bool>().Select(b => b ? "1" : "0"))}]");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Packed Object Details:");
            stringBuilder.AppendLine($"Position: [{string.Join(", ", packedObj2.Position)}]");
            stringBuilder.AppendLine($"Orientation: [{string.Join(", ", packedObj2.Orientation.Cast<bool>().Select(b => b ? "1" : "0"))}]");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Container Potential Containers:");
            foreach (var pc in con.potentialContainers)
            {
                string sizeStr = string.Join(", ", pc.Size);
                string posStr = string.Join(", ", pc.Position);
                stringBuilder.AppendLine($"Size: [{sizeStr}]  Position: [{posStr}]");
            }
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
