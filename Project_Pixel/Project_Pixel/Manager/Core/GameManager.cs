using Project_Pixel.Contents;
using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Manager
{
    public class GameManager
    {
        public MapManager MapManager { private set; get; }
        public Player Player { private set; get; }

        public void Init()
        {
            Player = new Player();
            MapManager = new MapManager();

            MapManager.Generate();
        }
    }
}
