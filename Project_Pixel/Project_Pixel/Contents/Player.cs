using Project_Pixel.Contents.Shop;
using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Contents
{
    public enum Direct
    {
        Up, Down, Right, Left
    }

    public class Player : Character
    {
        public PlayerInventory Inven { private set; get; }

        public Player() : base(CharacterType.Player)
        {
            Inven = new PlayerInventory();
            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);

            SetStatus(new Stat(130, 30, 1, 10));        // 체력, 공격력, 방어력, 치명타 확률
        }

        public void UpperStatus(Stat stat)
        {
            this.stat.MaxHp += stat.MaxHp;
            this.stat.NowHp += stat.MaxHp;
            this.stat.Power += stat.Power;
            this.stat.Defense += stat.Defense;
            this.stat.CriChance += stat.CriChance;
            this.stat.CriDamageValue += (stat.CriDamageValue / 100f);
        }

        public void ResumeGold(int gold) => Inven.ResumeGold(gold);
    }
}
