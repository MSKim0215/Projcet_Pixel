using Project_Pixel.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Utils
{
    public interface IMonsterAttack
    {
        void Attack(Player player);
    }

    public interface IPlayerAttack
    {
        void Attack(Monster monster);
    }
}
