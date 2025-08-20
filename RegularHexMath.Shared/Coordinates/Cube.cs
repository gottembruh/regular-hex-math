using System;
using System.Collections.Generic;

namespace RegularHexMath.Coordinates
{
    [Serializable]
    public readonly partial struct Cube : IEquatable<Cube>, IEqualityComparer<Cube>
    {
        public static Cube Zero => new Cube(0, 0, 0);

        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Cube(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Round float coordinates to nearest valid coordinate
        /// </summary>
        public Cube(float x, float y, float z)
        {
            var rx = (int) Math.Round(x);
            var ry = (int) Math.Round(y);
            var rz = (int) Math.Round(z);

            var xDiff = Math.Abs(rx - x);
            var yDiff = Math.Abs(ry - y);
            var zDiff = Math.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            X = rx;
            Y = ry;
            Z = rz;
        }

        public static bool operator ==(Cube coord1, Cube coord2)
        {
            return (coord1.X, coord1.Y, coord1.Z) == (coord2.X, coord2.Y, coord2.Z);
        }

        public static bool operator !=(Cube coord1, Cube coord2)
        {
            return (coord1.X, coord1.Y, coord1.Z) != (coord2.X, coord2.Y, coord2.Z);
        }

        public static Cube operator +(Cube coord1, Cube coord2)
        {
            return new Cube(coord1.X + coord2.X, coord1.Y + coord2.Y, coord1.Z + coord2.Z);
        }

        public static Cube operator +(Cube coord, int offset)
        {
            return new Cube(coord.X + offset, coord.Y + offset, coord.Z + offset);
        }

        public static Cube operator -(Cube coord1, Cube coord2)
        {
            return new Cube(coord1.X - coord2.X, coord1.Y - coord2.Y, coord1.Z - coord2.Z);
        }

        public static Cube operator -(Cube coord, int offset)
        {
            return new Cube(coord.X - offset, coord.Y - offset, coord.Z - offset);
        }

        public static Cube operator *(Cube coord, int offset)
        {
            return new Cube(coord.X * offset, coord.Y * offset, coord.Z * offset);
        }

        public static Cube operator *(Cube coord, float delta)
        {
            return new Cube(coord.X * delta, coord.Y * delta, coord.Z * delta);
        }

        public bool IsValid()
        {
            return X + Y + Z == 0;
        }

        public Cube RotateToRight()
        {
            var x = -Y;
            var y = -Z;
            var z = -X;
            return new Cube(x, y, z);
        }

        public Cube RotateToRight(int times)
        {
            var cur = this;
            for (var i = 0; i < times; i++)
            {
                cur = cur.RotateToRight();
            }

            return cur;
        }

        public override bool Equals(object? obj)
        {
            return obj is Cube other && Equals(other);
        }

        public bool Equals(Cube other)
        {
            return (X, Y, Z) == (other.X, other.Y, other.Z);
        }

        public bool Equals(Cube coord1, Cube coord2)
        {
            return coord1.Equals(coord2);
        }

        public override int GetHashCode()
        {
            return (X, Y, Z).GetHashCode();
        }

        public int GetHashCode(Cube coord)
        {
            return coord.GetHashCode();
        }

        public override string ToString()
        {
            return !IsValid() ? "C-[Invalid]" : $"C-[{X}:{Y}:{Z}]";
        }
    }
}