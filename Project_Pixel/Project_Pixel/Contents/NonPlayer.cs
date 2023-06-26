using Project_Pixel.Contents.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public class NonPlayer : Character
    {
        protected NonPlayer() : base(CharacterType.NPC) { }
    }

    public class Peddler : NonPlayer
    {
        public Inventory Inven { private set; get; }

        public Peddler() : base()
        {
            Inven = new Inventory();
        }
    }
}
