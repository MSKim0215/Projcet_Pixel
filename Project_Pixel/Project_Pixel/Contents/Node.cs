using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public class Node
    {
        public Position Position { set; get; }
        public Node Parent { set; get; }
        public int GCost { set; get; }
        public int HCost { set; get; }
        public int TotalCost { set; get; }

        public Node(Position position, Node parent)
        {
            Position = position;
            Parent = parent;

            GCost = 0;
            HCost = 0;
            TotalCost = 0;
        }
    }
}
