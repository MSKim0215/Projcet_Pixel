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

        public Room(Position position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;

            CenterPosition = new Position(position.X + width / 2, position.Y + height / 2);
        }
    }
}
