using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public class Stat
    {
        public int NowHp { set; get; }
        public int MaxHp { set; get; }
        public int Power { set; get; }
        public int Defense { set; get; }
        public int CriChance { set; get; }
        public float CriDamageValue { set; get; }

        public Stat(int maxHp, int power, int defense, int criChance, float criDamageValue = 1.5f)
        {
            MaxHp = maxHp;
            Power = power;
            Defense = defense;
            CriChance = criChance;
            CriDamageValue = criDamageValue;
            NowHp = MaxHp;
        }
    }

    public class ItemStat : Stat
    {
        public string Name { private set; get; }
        public int BuyGold { private set; get; }
        public int Hungry { private set; get; }

        public ItemStat(string name, int buyGold, int maxHp, int power, int defense, int criChance, int hungry, float criDamageValue = 1.5f) : base(maxHp, power, defense, criChance, criDamageValue)
        {
            Name = name;
            BuyGold = buyGold;
            Hungry = hungry;
        }
    }

    public class MonsterStat: Stat
    {
        public string Name { private set; get; }
        public int DropGold { private set; get; }

        public MonsterStat(string name, int maxHp, int power, int defense, int criChance, int dropGold, float criDamageValue = 1.5f) : base(maxHp, power, defense, criChance, criDamageValue)
        {
            Name = name;
            DropGold = dropGold;
        }
    }

    public class PlayerStat : Stat
    {
        public int Hungry { set; get; }
        public int HungryMax { set; get; }
        public int StartGold { private set; get; }

        public PlayerStat(int maxHp, int power, int defense, int criChance, int startGold, int hungryMax = 50, float criDamageValue = 1.5f) : base(maxHp, power, defense, criChance, criDamageValue)
        {
            HungryMax = hungryMax;
            Hungry = HungryMax;
            StartGold = startGold;
        }
    }
}
