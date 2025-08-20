using System;
using System.Collections.Generic;
using RegularHexMath.Coordinates;
using static System.Math;

namespace RegularHexMath
{
 
    public readonly partial struct HexGrid
    {
        /// <summary>
        /// Total count of edges in one Hex
        /// </summary>
        public const int EdgesCount = 6;

        public static readonly float Sqrt3 = (float) Sqrt(3);

        /// <summary>
        /// Inscribed radius of the hex
        /// </summary>
        public readonly float InnerRadius;

        /// <summary>
        /// Described radius of hex
        /// </summary>
        public readonly float OuterRadius;

        /// <summary>
        /// Hexagon side length
        /// </summary>
        public float Side => OuterRadius;

        /// <summary>
        /// Inscribed diameter of hex
        /// </summary>
        public float InnerDiameter => InnerRadius * 2;

        /// <summary>
        /// Described diameter of hex
        /// </summary>
        public float OuterDiameter => OuterRadius * 2;

        /// <summary>
        /// Orientation and layout of this grid
        /// </summary>
        public readonly HCellType Type;

        /// <summary>
        /// Offset between hex and its right-side neighbour on X axis
        /// </summary>
        public float HorizontalOffset
        {
            get
            {
                switch (Type)
                {
                    case HCellType.OddH:
                    case HCellType.EvenH:
                        return InnerRadius * 2.0f;
                    case HCellType.OddV:
                    case HCellType.EvenV:
                        return OuterRadius * 1.5f;
                    default:
                        throw new HexagonalException($"Can't get {nameof(HorizontalOffset)} with unexpected {nameof(Type)}", this);
                }
            }
        }

        /// <summary>
        /// Offset between hex and its up-side neighbour on Y axis
        /// </summary>
        public float VerticalOffset
        {
            get
            {
                switch (Type)
                {
                    case HCellType.OddH:
                    case HCellType.EvenH:
                        return OuterRadius * 1.5f;
                    case HCellType.OddV:
                    case HCellType.EvenV:
                        return InnerRadius * 2.0f;
                    default:
                        throw new HexagonalException($"Can't get {nameof(VerticalOffset)} with unexpected {nameof(Type)}", this);
                }
            }
        }

        /// <summary>
        /// The angle between the centers of any hex and its first neighbor relative to the vector (0, 1) clockwise
        /// </summary>
        /// <exception cref="HexagonalException"></exception>
        public float AngleToFirstNeighbor
        {
            get
            {
                switch (Type)
                {
                    case HCellType.OddH:
                    case HCellType.EvenH:
                        return 0.0f;
                    case HCellType.OddV:
                    case HCellType.EvenV:
                        return 30.0f;
                    default:
                        throw new HexagonalException($"Can't get {nameof(AngleToFirstNeighbor)} with unexpected {nameof(Type)}", this);
                }
            }
        }

        /// <summary>
        /// Base constructor for hexagonal grid
        /// </summary>
        /// <param name="type">Orientation and layout of the grid</param>
        /// <param name="radius">Inscribed radius</param>
        public HexGrid(HCellType type, float radius)
        {
            Type = type;
            InnerRadius = radius;
            OuterRadius = (float) (radius / Cos(PI / EdgesCount));
        }

        #region ToOffset

        /// <summary>
        /// Convert Cube coordinate to offset
        /// </summary>
        public Offset ToOffset(Cube coord)
        {
            switch (Type)
            {
                case HCellType.OddH:
                    {
                    var col = coord.X + (coord.Z - (coord.Z & 1)) / 2;
                    var row = coord.Z;
                    return new Offset(col, row);
                }
                case HCellType.EvenH:
                    {
                    var col = coord.X + (coord.Z + (coord.Z & 1)) / 2;
                    var row = coord.Z;
                    return new Offset(col, row);
                }
                  case HCellType.OddV:
                    {
                    var col = coord.X;
                    var row = coord.Z + (coord.X - (coord.X & 1)) / 2;
                    return new Offset(col, row);
                }
                case HCellType.EvenV:
                {
                    var col = coord.X;
                    var row = coord.Z + (coord.X + (coord.X & 1)) / 2;
                    return new Offset(col, row);
                }
                default:
                    throw new HexagonalException($"{nameof(ToOffset)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
            }
        }

        /// <summary>
        /// Convert axial coordinate to offset
        /// </summary>
        public Offset ToOffset(Axial axial)
        {
            return ToOffset(ToCube(axial));
        }

        /// <summary>
        /// Returns the offset coordinate of the hex which contains a point
        /// </summary>
        public Offset ToOffset(float x, float y)
        {
            return ToOffset(ToCube(x, y));
        }

        /// <summary>
        /// Returns the offset coordinate of the hex which contains a point
        /// </summary>
        public Offset ToOffset((float X, float Y) point)
        {
            return ToOffset(ToCube(point.X, point.Y));
        }

        #endregion

        #region ToAxial

        /// <summary>
        /// Convert Cube coordinate to axial
        /// </summary>
        public Axial ToAxial(Cube Cube)
        {
            return new Axial(Cube.X, Cube.Z);
        }

        /// <summary>
        /// Convert offset coordinate to axial
        /// </summary>
        public Axial ToAxial(Offset offset)
        {
            return ToAxial(ToCube(offset));
        }

        /// <summary>
        /// Returns the axial coordinate of the hex which contains a point
        /// </summary>
        public Axial ToAxial(float x, float y)
        {
            return ToAxial(ToCube(x, y));
        }

        /// <summary>
        /// Returns the axial coordinate of the hex which contains a point
        /// </summary>
        public Axial ToAxial((float X, float Y) point)
        {
            return ToAxial(ToCube(point.X, point.Y));
        }

        #endregion

        #region ToCube

        /// <summary>
        /// Convert offset coordinate to Cube
        /// </summary>
        public Cube ToCube(Offset coord)
        {
            switch (Type)
            {
                case HCellType.OddH:
                {
                    var x = coord.X - (coord.Y - (coord.Y & 1)) / 2;
                    var z = coord.Y;
                    var y = -x - z;
                    return new Cube(x, y, z);
                }
                case HCellType.EvenH:
                {
                    var x = coord.X - (coord.Y + (coord.Y & 1)) / 2;
                    var z = coord.Y;
                    var y = -x - z;
                    return new Cube(x, y, z);
                }
                case HCellType.OddV:
                {
                    var x = coord.X;
                    var z = coord.Y - (coord.X - (coord.X & 1)) / 2;
                    var y = -x - z;
                    return new Cube(x, y, z);
                }
                case HCellType.EvenV:
                {
                    var x = coord.X;
                    var z = coord.Y - (coord.X + (coord.X & 1)) / 2;
                    var y = -x - z;
                    return new Cube(x, y, z);
                }
                default:
                    throw new HexagonalException($"{nameof(ToCube)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
            }
        }

        /// <summary>
        /// Convert axial coordinate to Cube
        /// </summary>
        public Cube ToCube(Axial axial)
        {
            return new Cube(axial.Q, -axial.Q - axial.R, axial.R);
        }

        /// <summary>
        /// Returns the Cube coordinate of the hex which contains a point
        /// </summary>
        public Cube ToCube(float x, float y)
        {
            switch (Type)
            {
                case HCellType.OddH:
                case HCellType.EvenH:
                {
                    var q = (x * Sqrt3 / 3.0f - y / 3.0f) / Side;
                    var r = y * 2.0f / 3.0f / Side;
                    return new Cube(q, -q - r, r);
                }
                case HCellType.OddV:
                case HCellType.EvenV:
                    {
                    var q = x * 2.0f / 3.0f / Side;
                    var r = (-x / 3.0f + Sqrt3 / 3.0f * y) / Side;
                    return new Cube(q, -q - r, r);
                }
                default:
                    throw new HexagonalException($"{nameof(ToCube)} failed with unexpected {nameof(Type)}", this, (nameof(x), x), (nameof(y), y));
            }
        }

        /// <summary>
        /// Returns the Cube coordinate of the hex which contains a point
        /// </summary>
        public Cube ToCube((float X, float Y) point)
        {
            return ToCube(point.X, point.Y);
        }

        #endregion

        #region ToPoint2

        /// <summary>
        /// Convert hex based on its offset coordinate to it center position in 2d space
        /// </summary>
        public (float X, float Y) ToPoint2(Offset coord)
        {
            return ToPoint2(ToAxial(coord));
        }

        /// <summary>
        /// Convert hex based on its axial coordinate to it center position in 2d space
        /// </summary>
        public (float X, float Y) ToPoint2(Axial coord)
        {
            switch (Type)
            {
                case HCellType.OddH:
                case HCellType.EvenH:
                    {
                    var x = Side * (Sqrt3 * coord.Q + Sqrt3 / 2 * coord.R);
                    var y = Side * (3.0f / 2.0f * coord.R);
                    return (x, y);
                }
                case HCellType.OddV:
                case HCellType.EvenV:
                    {
                    var x = Side * (3.0f / 2.0f * coord.Q);
                    var y = Side * (Sqrt3 / 2 * coord.Q + Sqrt3 * coord.R);
                    return (x, y);
                }
                default:
                    throw new HexagonalException($"{nameof(ToPoint2)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
            }
        }

        /// <summary>
        /// Convert hex based on its Cube coordinate to it center position in 2d space
        /// </summary>
        public (float X, float Y) ToPoint2(Cube coord)
        {
            return ToPoint2(ToAxial(coord));
        }

        #endregion

        #region GetCornerPoint

        /// <summary>
        /// Returns corner point in 2d space  of given coordinate
        /// </summary>
        public (float X, float Y) GetCornerPoint(Offset coord, int edge)
        {
            return GetCornerPoint(coord, edge, ToPoint2);
        }

        /// <summary>
        /// Returns corner point in 2d space  of given coordinate
        /// </summary>
        public (float X, float Y) GetCornerPoint(Axial coord, int edge)
        {
            return GetCornerPoint(coord, edge, ToPoint2);
        }

        /// <summary>
        /// Returns corner point in 2d space  of given coordinate
        /// </summary>
        public (float X, float Y) GetCornerPoint(Cube coord, int edge)
        {
            return GetCornerPoint(coord, edge, ToPoint2);
        }

        /// <summary>
        /// Returns corner point in 2d space  of given coordinate
        /// </summary>
        private (float X, float Y) GetCornerPoint<T>(T coord, int edge, Func<T, (float X, float Y)> toPoint)
            where T : struct
        {
            edge = NormalizeIndex(edge);
            var angleDeg = 60 * edge;
            if (Type == HCellType.EvenH || Type == HCellType.OddH)
            {
                angleDeg -= 30;
            }

            var center = toPoint(coord);
            var angleRad = PI / 180 * angleDeg;
            var x = (float) (center.X + OuterRadius * Cos(angleRad));
            var y = (float) (center.Y + OuterRadius * Sin(angleRad));
            return (x, y);
        }

        #endregion

        #region GetNeighbor

        /// <summary>
        /// Returns the neighbor at the specified index.
        /// </summary>
        public Offset GetNeighbor(Offset coord, int neighborIndex)
        {
            return coord + GetNeighborsOffsets(coord)[NormalizeIndex(neighborIndex)];
        }

        /// <summary>
        /// Returns the neighbor at the specified index.
        /// </summary>
        public Axial GetNeighbor(Axial coord, int neighborIndex)
        {
            return coord + _axialNeighbors[NormalizeIndex(neighborIndex)];
        }

        /// <summary>
        /// Returns the neighbor at the specified index.
        /// </summary>
        public Cube GetNeighbor(Cube coord, int neighborIndex)
        {
            return coord + _CubeNeighbors[NormalizeIndex(neighborIndex)];
        }

        #endregion

        #region GetNeighbors

        /// <summary>
        /// Return all neighbors of the hex
        /// </summary>
        public IEnumerable<Offset> GetNeighbors(Offset hex)
        {
            foreach (var offset in GetNeighborsOffsets(hex))
            {
                yield return offset + hex;
            }
        }

        /// <summary>
        /// Return all neighbors of the hex
        /// </summary>
        public IEnumerable<Axial> GetNeighbors(Axial hex)
        {
            foreach (var offset in _axialNeighbors)
            {
                yield return offset + hex;
            }
        }

        /// <summary>
        /// Return all neighbors of the hex
        /// </summary>
        public IEnumerable<Cube> GetNeighbors(Cube hex)
        {
            foreach (var offset in _CubeNeighbors)
            {
                yield return offset + hex;
            }
        }

        #endregion

        #region IsNeighbors

        /// <summary>
        /// Checks whether the two hexes are neighbors or no
        /// </summary>
        public bool IsNeighbors(Offset coord1, Offset coord2)
        {
            return IsNeighbors(coord1, coord2, GetNeighbor);
        }

        /// <summary>
        /// Checks whether the two hexes are neighbors or no
        /// </summary>
        public bool IsNeighbors(Axial coord1, Axial coord2)
        {
            Func<Axial, int, Axial> getNeighbor = GetNeighbor;
            return IsNeighbors(coord1, coord2, getNeighbor);
        }

        /// <summary>
        /// Checks whether the two hexes are neighbors or no
        /// </summary>
        public bool IsNeighbors(Cube coord1, Cube coord2)
        {
            return IsNeighbors(coord1, coord2, GetNeighbor);
        }

        /// <summary>
        /// Checks whether the two hexes are neighbors or no
        /// </summary>
        public bool IsNeighbors<T>(T coord1, T coord2, in Func<T, int, T> getNeighbor)
            where T : struct, IEqualityComparer<T>
        {
            for (var neighborIndex = 0; neighborIndex < EdgesCount; neighborIndex++)
            {
                var neighbor = getNeighbor(coord1, neighborIndex);
                if (neighbor.Equals(coord2))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region GetNeighborsRing

        /// <summary>
        /// Returns a ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Offset> GetNeighborsRing(Offset center, int radius)
        {
            return GetNeighborsRing(center, radius, GetNeighbor);
        }

        /// <summary>
        /// Returns a ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Axial> GetNeighborsRing(Axial center, int radius)
        {
            return GetNeighborsRing(center, radius, GetNeighbor);
        }

        /// <summary>
        /// Returns a ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Cube> GetNeighborsRing(Cube center, int radius)
        {
            return GetNeighborsRing(center, radius, GetNeighbor);
        }

        /// <summary>
        /// Returns a ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        private static IEnumerable<T> GetNeighborsRing<T>(T center, int radius, Func<T, int, T> getNeighbor)
            where T : struct
        {
            if (radius == 0)
            {
                yield return center;
                yield break;
            }

            for (var i = 0; i < radius; i++)
            {
                center = getNeighbor(center, 4);
            }

            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < radius; j++)
                {
                    yield return center;
                    center = getNeighbor(center, i);
                }
            }
        }

        #endregion

        #region GetNeighborsAround

        /// <summary>
        /// Returns a all hexes in the ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Offset> GetNeighborsAround(Offset center, int radius)
        {
            return GetNeighborsAround(center, radius, GetNeighborsRing);
        }

        /// <summary>
        /// Returns a all hexes in the ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Axial> GetNeighborsAround(Axial center, int radius)
        {
            return GetNeighborsAround(center, radius, GetNeighborsRing);
        }

        /// <summary>
        /// Returns a all hexes in the ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        public IEnumerable<Cube> GetNeighborsAround(Cube center, int radius)
        {
            return GetNeighborsAround(center, radius, GetNeighborsRing);
        }

        /// <summary>
        /// Returns a all hexes in the ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
        /// </summary>
        private static IEnumerable<T> GetNeighborsAround<T>(T center, int radius, Func<T, int, IEnumerable<T>> getNeighborRing)
            where T : struct
        {
            for (var i = 0; i < radius; i++)
            {
                foreach (var hex in getNeighborRing(center, i))
                {
                    yield return hex;
                }
            }
        }

        #endregion

        #region GetNeighborIndex

        /// <summary>
        /// Returns the bypass index to the specified neighbor
        /// </summary>
        public byte GetNeighborIndex(Offset center, Offset neighbor)
        {
            return GetNeighborIndex(center, neighbor, GetNeighbors);
        }

        /// <summary>
        /// Returns the bypass index to the specified neighbor
        /// </summary>
        public byte GetNeighborIndex(Axial center, Axial neighbor)
        {
            return GetNeighborIndex(center, neighbor, GetNeighbors);
        }

        /// <summary>
        /// Returns the bypass index to the specified neighbor
        /// </summary>
        public byte GetNeighborIndex(Cube center, Cube neighbor)
        {
            return GetNeighborIndex(center, neighbor, GetNeighbors);
        }

        /// <summary>
        /// Returns the bypass index to the specified neighbor
        /// </summary>
        private byte GetNeighborIndex<T>(T center, T neighbor, Func<T, IEnumerable<T>> getNeighbors)
            where T : struct, IEqualityComparer<T>
        {
            byte neighborIndex = 0;
            foreach (var current in getNeighbors(center))
            {
                if (current.Equals(neighbor))
                {
                    return neighborIndex;
                }

                neighborIndex++;
            }

            throw new HexagonalException($"Can't find bypass index", this, (nameof(center), center), (nameof(neighbor), neighbor));
        }

        #endregion

        #region GetPointBetweenTwoNeighbours

        /// <summary>
        /// Returns the midpoint of the boundary segment of two neighbors
        /// </summary>
        public (float x, float y) GetPointBetweenTwoNeighbours(Offset coord1, Offset coord2)
        {
            return GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);
        }

        /// <summary>
        /// Returns the midpoint of the boundary segment of two neighbors
        /// </summary>
        public (float x, float y) GetPointBetweenTwoNeighbours(Axial coord1, Axial coord2)
        {
            return GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);
        }

        /// <summary>
        /// Returns the midpoint of the boundary segment of two neighbors
        /// </summary>
        public (float x, float y) GetPointBetweenTwoNeighbours(Cube coord1, Cube coord2)
        {
            return GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);
        }

        /// <summary>
        /// Returns the midpoint of the boundary segment of two neighbors
        /// </summary>
        private (float x, float y) GetPointBetweenTwoNeighbours<T>(T coord1, T coord2, Func<T, T, bool> isNeighbor, Func<T, (float X, float Y)> toPoint)
            where T : struct
        {
            if (!isNeighbor(coord1, coord2))
            {
                throw new HexagonalException($"Can't calculate point between not neighbors", this, (nameof(coord1), coord1), (nameof(coord2), coord2));
            }

            var c1 = toPoint(coord1);
            var c2 = toPoint(coord2);

            return ((c1.X + c2.X) / 2, (c1.Y + c2.Y) / 2);
        }

        #endregion

        #region CubeDistance

        /// <summary>
        /// Manhattan distance between two hexes
        /// </summary>
        public int CubeDistance(Offset h1, Offset h2)
        {
            var CubeFrom = ToCube(h1);
            var CubeTo = ToCube(h2);
            return CubeDistance(CubeFrom, CubeTo);
        }

        /// <summary>
        /// Manhattan distance between two hexes
        /// </summary>
        public int CubeDistance(Axial h1, Axial h2)
        {
            var CubeFrom = ToCube(h1);
            var CubeTo = ToCube(h2);
            return CubeDistance(CubeFrom, CubeTo);
        }

        /// <summary>
        /// Manhattan distance between two hexes
        /// </summary>
        public static int CubeDistance(Cube h1, Cube h2)
        {
            return (Abs(h1.X - h2.X) + Abs(h1.Y - h2.Y) + Abs(h1.Z - h2.Z)) / 2;
        }

        #endregion

        #region Neighbors

        /// <summary>
        /// Return all neighbors offsets of the hex
        /// </summary>
        private IReadOnlyList<Offset> GetNeighborsOffsets(Offset coord)
        {
            switch (Type)
            {
                case HCellType.OddH:
                    return Abs(coord.Y % 2) == 0 ? _pointyEvenNeighbors : _pointyOddNeighbors;
                case HCellType.EvenH:
                    return Abs(coord.Y % 2) == 1 ? _pointyEvenNeighbors : _pointyOddNeighbors;
                case HCellType.OddV:
                    return Abs(coord.X % 2) == 0 ? _flatEvenNeighbors : _flatOddNeighbors;
                case HCellType.EvenV:
                    return Abs(coord.X % 2) == 1 ? _flatEvenNeighbors : _flatOddNeighbors;
                default:
                    throw new HexagonalException($"{nameof(GetNeighborsOffsets)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
            }
        }

        private static readonly List<Offset> _pointyOddNeighbors = new List<Offset>
        {
            new Offset(+1, 0), new Offset(+1, -1), new Offset(0, -1),
            new Offset(-1, 0), new Offset(0, +1), new Offset(+1, +1),
        };

        private static readonly List<Offset> _pointyEvenNeighbors = new List<Offset>
        {
            new Offset(+1, 0), new Offset(0, -1), new Offset(-1, -1),
            new Offset(-1, 0), new Offset(-1, +1), new Offset(0, +1),
        };

        private static readonly List<Offset> _flatOddNeighbors = new List<Offset>
        {
            new Offset(+1, +1), new Offset(+1, 0), new Offset(0, -1),
            new Offset(-1, 0), new Offset(-1, +1), new Offset(0, +1),
        };

        private static readonly List<Offset> _flatEvenNeighbors = new List<Offset>
        {
            new Offset(+1, 0), new Offset(+1, -1), new Offset(0, -1),
            new Offset(-1, -1), new Offset(-1, 0), new Offset(0, +1),
        };

        private static readonly List<Axial> _axialNeighbors = new List<Axial>
        {
            new Axial(+1, 0), new Axial(+1, -1), new Axial(0, -1),
            new Axial(-1, 0), new Axial(-1, +1), new Axial(0, +1),
        };

        private static readonly List<Cube> _CubeNeighbors = new List<Cube>
        {
            new Cube(+1, -1, 0), new Cube(+1, 0, -1), new Cube(0, +1, -1),
            new Cube(-1, +1, 0), new Cube(-1, 0, +1), new Cube(0, -1, +1),
        };

        #endregion

        private static int NormalizeIndex(int index)
        {
            index = index % EdgesCount;
            if (index < 0)
            {
                index += EdgesCount;
            }

            return index;
        }
    }
}