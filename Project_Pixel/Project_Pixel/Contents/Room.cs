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

        public bool IsVisited { get; set; }

        public List<Room> Neighbors { get; set; } = new List<Room>();

        public Room(Position position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;

            CenterPosition = new Position(position.X + width / 2, position.Y + height / 2);
        }

        public double GetDistance(Position target) => Math.Sqrt(Math.Pow(CenterPosition.X - target.X, 2) + Math.Pow(CenterPosition.Y - target.Y, 2));
    }
}
