using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Manager
{
    public class GameManager
    {
        public void Init()
        {
            Console.WriteLine("게임 매니저 초기화");
        }

        public void OnUpdate()
        {
            Console.WriteLine("게임 매니저 업데이트 진행");
        }
    }
}
