using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Managers.Instance.Start();
        }
    }
}
