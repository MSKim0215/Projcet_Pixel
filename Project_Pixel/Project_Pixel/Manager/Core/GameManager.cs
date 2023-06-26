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

        public void Init()
        {
            mapManager = new MapManager();
        }

        public void OnUpdate()
        {
            Console.WriteLine("게임 매니저 업데이트 진행");
        }
    }
}
