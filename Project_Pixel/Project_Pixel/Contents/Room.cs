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
        public Position SubPosition { set; get; }

        public bool isVisited;

        public Room partnerRoom;

        public Room(Position position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            isVisited = false;

            CenterPosition = new Position(position.X + width / 2, position.Y + height / 2);

            Random rand = new Random();
            SubPosition = new Position(rand.Next(position.X + 1, position.X + width - 2), rand.Next(position.Y + 1, position.Y + height - 2));
        }

        public void RefreshSubPosition()
        {
            Random rand = new Random();
            SubPosition = new Position(rand.Next(Position.X + 1, Position.X + Width - 2), rand.Next(Position.Y + 1, Position.Y + Height - 2));
        }

        public bool ContainsPosition(Position position)
        {
            if (position.X >= Position.X && position.X < Position.X + Width &&
                position.Y >= Position.Y && position.Y < Position.Y + Height)
            {
                return true;
            }
            return false;
        }
    }

    public class Corridor
    {
        public bool isVisited;

        public List<Position> Positions { private set; get; } = new List<Position>();
        public List<Position> AroundPositions { private set; get; } = new List<Position>();

        public Corridor(List<Position> positions, List<Position> arounds)
        {
            Positions = positions;
            AroundPositions = arounds;
            isVisited = false;
        }

        public bool ContainsPosition(Position position)
        {
            foreach (Position corridorPos in Positions)
            {
                if (corridorPos.X == position.X && corridorPos.Y == position.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CorridorPosComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position posA, Position posB)
        {
            return posA.X == posB.X && posA.Y == posB.Y;
        }

        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }
    }
}
