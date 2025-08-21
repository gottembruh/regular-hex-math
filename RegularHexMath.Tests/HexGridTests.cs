using RegularHexMath.Coordinates;
using static System.Math;

namespace RegularHexMath.Tests
{
    [TestFixture(TestOf = typeof(HexGrid))]
    public class HexGridTests
    {
        private float InnerRadius => 0.5f;
        private float OuterRadius => (float) (InnerRadius / Cos(PI / HexGrid.EdgesCount));

        [TestCase(HCellType.EvenV)]
        [TestCase(HCellType.OddV)]
        public void FlatPropertiesTest(HCellType type)
        {
            var grid = new HexGrid(type, InnerRadius);
            
            Assert.That(InnerRadius, Is.EqualTo(grid.InnerRadius));
            Assert.That(OuterRadius, Is.EqualTo(grid.OuterRadius));
            Assert.That(InnerRadius * 2, Is.EqualTo(grid.InnerDiameter));
            Assert.That(OuterRadius * 2, Is.EqualTo(grid.OuterDiameter));
            Assert.That(OuterRadius * 1.5f, Is.EqualTo(grid.HorizontalOffset));
            Assert.That(InnerRadius * 2.0f, Is.EqualTo(grid.VerticalOffset));
            Assert.That(OuterRadius, Is.EqualTo(grid.Side));
            Assert.That(grid.AngleToFirstNeighbor, Is.EqualTo(30.0f));
        }

        [TestCase(HCellType.EvenH)]
        [TestCase(HCellType.OddH)]
        public void PointyPropertiesTest(HCellType type)
        {
            var grid = new HexGrid(type, InnerRadius);
            Assert.That(InnerRadius, Is.EqualTo(grid.InnerRadius));
            Assert.That(OuterRadius, Is.EqualTo(grid.OuterRadius));
            Assert.That(InnerRadius * 2, Is.EqualTo(grid.InnerDiameter));
            Assert.That(OuterRadius * 2, Is.EqualTo(grid.OuterDiameter));
            Assert.That(InnerRadius * 2.0f, Is.EqualTo(grid.HorizontalOffset));
            Assert.That(OuterRadius * 1.5f, Is.EqualTo(grid.VerticalOffset));
            Assert.That(OuterRadius, Is.EqualTo(grid.Side));
            Assert.That(0.0f, Is.EqualTo(grid.AngleToFirstNeighbor));
        }

        public void CoordinateConversionTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cube = grid.ToCube(offset);
            Assert.Multiple(() =>
            {
                Assert.That(cube.IsValid(), Is.True, $"Invalid cubic coordinate: {cube.X}-{cube.Y}-{cube.Z}");
                Assert.That(grid.ToOffset(axial), Is.EqualTo(offset));
                Assert.That(grid.ToOffset(cube), Is.EqualTo(offset));
                Assert.That(grid.ToAxial(offset), Is.EqualTo(axial));
                Assert.That(grid.ToAxial(cube), Is.EqualTo(axial));
                Assert.That(grid.ToCube(offset), Is.EqualTo(cube));
                Assert.That(grid.ToCube(axial), Is.EqualTo(cube));
            });
        }

        public void PointConversionTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cubic = grid.ToCube(offset);

            var fromOffset = grid.ToPoint2(offset);
            var fromAxial = grid.ToPoint2(axial);
            var fromCubic = grid.ToPoint2(cubic);

            Assert.Multiple(() =>
            {
                Assert.That(fromOffset.SimilarTo(fromAxial), Is.True, $"Expected: {fromAxial.X}:{fromAxial.Y}; Actual: {fromOffset.X}:{fromOffset.Y}");
                Assert.That(fromOffset.SimilarTo(fromCubic), Is.True, $"Expected: {fromCubic.X}:{fromCubic.Y}; Actual: {fromOffset.X}:{fromOffset.Y}");
                Assert.That(grid.ToOffset(fromOffset), Is.EqualTo(offset));
                Assert.That(grid.ToAxial(fromAxial), Is.EqualTo(axial));
                Assert.That(grid.ToCube(fromCubic), Is.EqualTo(cubic));
            });
        }

        public void IsNeighborTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY,
            [Values(-1, 0, 1, 2, 3, 4, 5, 6)] int neighborIndex)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cubic = grid.ToCube(offset);
            Assert.That(cubic.IsValid(), Is.True, $"Invalid cubic coordinate: {cubic.X}-{cubic.Y}-{cubic.Z}");

            var oNeighbor = grid.GetNeighbor(offset, neighborIndex);
            var aNeighbor = grid.GetNeighbor(axial, neighborIndex);
            var cNeighbor = grid.GetNeighbor(cubic, neighborIndex);

            Assert.Multiple(() =>
            {
                Assert.That(cNeighbor.IsValid(), Is.True, $"Invalid cubic coordinate: {cNeighbor.X}-{cNeighbor.Y}-{cNeighbor.Z}");
                Assert.That(grid.IsNeighbors(offset, oNeighbor), Is.True, $"Neighbor1={offset}; Neighbor2={oNeighbor}; Index={neighborIndex};");
                Assert.That(grid.IsNeighbors(axial, aNeighbor), Is.True, $"Neighbor1={axial}; Neighbor2={aNeighbor}; Index={neighborIndex};");
                Assert.That(grid.IsNeighbors(cubic, cNeighbor), Is.True, $"Neighbor1={cubic}; Neighbor2={cNeighbor}; Index={neighborIndex};");
            });
        }

        public void NeighborsOrderTest(
            [Values] HCellType type,
            [Values(-13, -8, 0, 15, 22)] int offsetX,
            [Values(-13, -8, 0, 15, 22)] int offsetY,
            [Values(-1, 0, 1, 2, 3, 4, 5, 6)] int neighborIndex)
        {
            var grid = new HexGrid(type, InnerRadius);
            var offset = new Offset(offsetX, offsetY);
            var axial = grid.ToAxial(offset);
            var cubic = grid.ToCube(offset);

            var oNeighbor = grid.GetNeighbor(offset, neighborIndex);
            var aNeighbor = grid.GetNeighbor(axial, neighborIndex);
            var cNeighbor = grid.GetNeighbor(cubic, neighborIndex);

            Assert.That(cNeighbor.IsValid(), Is.True, $"Invalid cubic coordinate: {cNeighbor.X}-{cNeighbor.Y}-{cNeighbor.Z}");

            var fromAxial = grid.ToOffset(aNeighbor);
            var fromCubic = grid.ToOffset(cNeighbor);

            Assert.Multiple(() =>
            {
                Assert.That(fromAxial, Is.EqualTo(oNeighbor), $"Center=({offset} - {axial}); Current=({oNeighbor} - {aNeighbor}); Index={neighborIndex};");
                Assert.That(fromCubic, Is.EqualTo(oNeighbor), $"Center=({offset} - {cubic}); Current=({oNeighbor} - {cNeighbor}); Index={neighborIndex};");
            });
        }
    }
}