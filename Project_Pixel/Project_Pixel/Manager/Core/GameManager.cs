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
        private MapManager mapManager;

        public Player Player { private set; get; }

        public void Init()
        {
            Player = new Player();
            mapManager = new MapManager();
        }
    }
}
