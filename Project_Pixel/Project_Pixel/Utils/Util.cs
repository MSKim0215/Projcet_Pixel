using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Utils
{
    public static class Util
    {
        public static void Swap(ref string targetA, ref string targetB)
        {
            string temp = targetA;
            targetA = targetB;
            targetB = temp;
        }
    }
}
