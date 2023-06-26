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
        public int CriChanceMax { set; get; }
        public float CriDamageValue { set; get; }

        public Stat(int maxHp, int power, int defense, int criChance, int criChanceMax = 100, float criDamageValue = 1.5f)
        {
            MaxHp = maxHp;
            Power = power;
            Defense = defense;
            CriChance = criChance;
            CriChanceMax = criChanceMax;
            CriDamageValue = criDamageValue;
            NowHp = MaxHp;
        }
    }
}
