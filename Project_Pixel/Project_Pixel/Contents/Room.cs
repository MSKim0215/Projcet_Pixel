using Project_Pixel.Contents.Shop;
using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public class Room
    {
        public Position Position { private set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }

        public Position CenterPosition { private set; get; }

        public bool isVisited;

        public Room(Position position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            isVisited = false;

            CenterPosition = new Position(position.X + width / 2, position.Y + height / 2);
        }
    }

    public class Corridor
    {
        public bool isVisited;

        public List<Position> Positions { private set; get; } = new List<Position>();

        public Corridor(List<Position> positions)
        {
            Positions = positions;

            isVisited = false;
        }
    }

    public class CorridorPosComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position posA, Position posB)
        {
            if (ReferenceEquals(posA, posB)) return true;
            if (ReferenceEquals(posA, null) || ReferenceEquals(posB, null)) return false;
            return posA.X == posB.X && posA.Y == posB.Y;
        }

        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }
    }
}
