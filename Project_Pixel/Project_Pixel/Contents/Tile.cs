using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public enum TileTpyes
    {
        Wall, Empty, Door
    }

    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Tile
    {
        public TileTpyes Types { private set; get; }
        public Position CurrPos { private set; get; }
        public Position PrevPos { private set; get; }

        public Tile(TileTpyes types, Position currPos, Position prevPos)
        {
            Types = types;
            CurrPos = currPos;
            PrevPos = prevPos;
        }
    }
}
