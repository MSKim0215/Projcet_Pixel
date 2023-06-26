using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Manager
{
    public class DataManager
    {
        public void Init()
        {
            Console.WriteLine("데이터 매니저 초기화");
        }

        public void OnUpdate()
        {
            Console.WriteLine("데이터 매니저 업데이트");
        }
    }
}
