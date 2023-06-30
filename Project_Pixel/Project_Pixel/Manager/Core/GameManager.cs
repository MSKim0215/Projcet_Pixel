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
        public Peddler Peddler { private set; get; }
        public Monster[] Monsters { private set; get; } = new Monster[5];

        public void Init()
        {
            Player = new Player();
            Peddler = new Peddler();
            MapManager = new MapManager();

            Managers.UI.Print_GameScene();
            Managers.UI.Print_Status(Player);
            Managers.UI.Print_State(Player);
            Managers.UI.Print_Inventory(Player);
            Managers.UI.Print_Log();
            Managers.UI.Print_Guide();
            Managers.UI.Print_BaseGuide();
            Managers.UI.Print_Enter();

            MapManager.Generate();
        }
    }
}
