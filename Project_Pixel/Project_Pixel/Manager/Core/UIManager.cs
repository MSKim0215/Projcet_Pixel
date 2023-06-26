using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Manager
{
    public class UIManager
    {
        public string[] TilePatterns = { "■", "  ", "◇" };
        // 벽, 기본, 문

        public void Init()
        {
            Console.SetWindowSize(185, 50);
        }
    }
}
