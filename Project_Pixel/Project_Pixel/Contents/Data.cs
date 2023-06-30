using Project_Pixel.Contents.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Contents
{
    public class StatData
    {
        public int MaxHp { private set; get; }
        public int Power { private set; get; }
        public int Defense { private set; get; }
        public int CriChance { private set; get; }
        public float CriDamageValue { private set; get; }

        public StatData(int maxHp, int power, int defense, int criChance, float criValue)
        {
            MaxHp = maxHp;
            Power = power;
            Defense = defense;
            CriChance = criChance;
            CriDamageValue = criValue;
        }
    }

    public class MonsterStatData : StatData
    {
        public string Name { private set; get; }
        public int DropGold { private set; get; }

        public MonsterStatData(string name, int maxHp, int power, int defense, int criChance, float criValue, int dropGold) : base(maxHp, power, defense, criChance, criValue)
        {
            Name = name;
            DropGold = dropGold;
        }
    }

    public class PlayerStatData : StatData
    {
        public int HungryMax { private set; get; }
        public int StartGold { private set; get; }

        public PlayerStatData(int maxHp, int power, int defense, int criChance, float criValue, int hungryMax, int startGold) : base(maxHp, power, defense, criChance, criValue)
        {
            HungryMax = hungryMax;
            StartGold = startGold;
        }
    }

    public class ItemStatData : StatData
    {
        public string Name { private set; get; }
        public int BuyGold { private set; get; }
        public int Hungry { private set; get; }

        public ItemStatData(string name, int maxHp, int power, int defense, int criChance, float criValue, int buyGold, int hungry = 0) : base(maxHp, power, defense, criChance, criValue)
        {
            Name = name;
            BuyGold = buyGold;
            Hungry = hungry;
        }
    }

    public class ItemData
    {
        public ItemStat Stat { private set; get; }

        public void Init(ItemType type)
        {
            Stat = Managers.Data.GetItemStatData(type);
        }

        public string GetItemStatus()
        {
            switch (Stat.Name)
            {
                case "롱소드":
                    {
                        return $"공격력 +{Stat.Power}";
                    }
                case "사슬 갑옷":
                    {
                        return $"최대 체력 -{Stat.MaxHp}  방어력 +{Stat.Defense}";
                    }
                case "마법이 깃든 장갑":
                    {
                        return $"최대 체력 +{Stat.MaxHp}  치명타 확률 +{Stat.CriChance}%";
                    }
                case "회복 물약":
                    {
                        return $"체력 회복 +{Stat.MaxHp}";
                    }
                case "도시락":
                    {
                        return $"배고픔 +{Stat.Hungry}";
                    }
                case "목걸이":
                    {
                        return $"치명타 확률 +{Stat.CriChance}%   치명타 데미지 +{Stat.CriDamageValue}%";
                    }
                case "탈출 스크롤":
                    {
                        return $"던전 어딘가로 이동합니다.";
                    }
            }
            return null;
        }
    }
}
