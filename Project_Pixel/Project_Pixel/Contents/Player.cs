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
        public new PlayerStat Status { private set; get; }

        public Player() : base(CharacterType.Player)
        {
            Inven = new PlayerInventory();
            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);
            Status = new PlayerStat(50, 3, 1, 10);      // 체력, 공격력, 방어력, 치명타 확률      
        }

        public void UpperStatus(Stat stat)
        {
            Status.MaxHp += stat.MaxHp;
            Status.NowHp += stat.MaxHp;
            Status.Power += stat.Power;
            Status.Defense += stat.Defense;
            Status.CriChance += stat.CriChance;
            Status.CriDamageValue += (stat.CriDamageValue / 100f);
        }

        public void ResumeGold(int gold) => Inven.ResumeGold(gold);

        public int GetHungry() => Status.Hungry;
        public int GetMaxHungry() => Status.HungryMax; 
        public override int GetNowHp() => Status.NowHp;
        public override int GetMaxHp() => Status.MaxHp;
        public override int GetPower() => Status.Power;
        public override int GetDefense() => Status.Defense;
        public override float GetCriChance() => Status.CriChance;
        public override float GetCriChanceMax() => Status.CriChanceMax;
        public override float GetCriDamageValue() => Status.CriDamageValue;
        public override bool IsDead() => Status.NowHp <= 0;
    }
}
