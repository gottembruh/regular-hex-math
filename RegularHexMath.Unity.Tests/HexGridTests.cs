using RegularHexMath.Coordinates;

namespace RegularHexMath.Unity.Tests
{
    [TestFixture]
    public class HexGridTests
    {
        private float InnerRadius => 0.5f;

        public void Vector2ConvertTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cubic = grid.ToCube(offset);

            var fromOffset = grid.ToVector2(offset);
            var fromAxial = grid.ToVector2(axial);
            var fromCubic = grid.ToVector2(cubic);

            Assert.IsTrue(fromOffset.SimilarTo(fromAxial), $"Expected: {fromAxial}; Actual: {fromOffset}");
            Assert.IsTrue(fromOffset.SimilarTo(fromCubic), $"Expected: {fromCubic}; Actual: {fromOffset}");

            Assert.That(grid.ToOffset(fromOffset), Is.EqualTo(offset));
            Assert.That(grid.ToAxial(fromAxial), Is.EqualTo(axial));
            Assert.That(grid.ToCube(fromCubic), Is.EqualTo(cubic));
        }

        public void Vector3ConvertTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cubic = grid.ToCube(offset);

            var fromOffset = grid.ToVector3(offset);
            var fromAxial = grid.ToVector3(axial);
            var fromCubic = grid.ToVector3(cubic);

            Assert.IsTrue(fromOffset.SimilarTo(fromAxial), $"Expected: {fromAxial}; Actual: {fromOffset}");
            Assert.IsTrue(fromOffset.SimilarTo(fromCubic), $"Expected: {fromCubic}; Actual: {fromOffset}");

            Assert.That(grid.ToOffset(fromOffset), Is.EqualTo(offset));
            Assert.That(grid.ToAxial(fromAxial), Is.EqualTo(axial));
            Assert.That(grid.ToCube(fromCubic), Is.EqualTo(cubic));
        }
    }
}