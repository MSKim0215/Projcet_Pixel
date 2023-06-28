using Project_Pixel.Contents.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents.Debuff_System
{
    public enum DebuffType
    {
        Bleeding,
        Poisoning,
        Hunger,
        Starvation
    }

    public class Debuff
    {
        public DebuffType Type { private set; get; }
        public string Name { private set; get; }
        public int Turn { private set; get; }
        public int TurnMax { private set; get; }
        public int Damage { private set; get; }

        public Debuff(DebuffType type)
        {
            Type = type;
            Name = GetDebuffName(type);
            Turn = GetDebuffTurn(type);
            TurnMax = GetDebuffTurn(type);
        }

        public void DecreaseTurn(int amount = -1)
        {
            if (Type == DebuffType.Hunger) return;
            Turn += amount;
        }

        public void ClearTurn()
        {
            if (Type == DebuffType.Starvation)
            {
                if (Turn <= 0)
                {
                    Turn = TurnMax;
                }
            }
        }

        public int GetDamage()
        {
            switch(Type)
            {
                case DebuffType.Poisoning: return 1 + (Turn / 3);
                case DebuffType.Bleeding: return 1 + (Turn / 2);
                case DebuffType.Hunger: return 0;
                case DebuffType.Starvation: return (Turn <= 0) ? 1 : 0;
            }
            return 0;
        }

        private string GetDebuffName(DebuffType type)
        {
            switch (type)
            {
                case DebuffType.Bleeding: return "출혈";
                case DebuffType.Poisoning: return "중독";
                case DebuffType.Hunger: return "배고픔";
                case DebuffType.Starvation: return "굶주림";
            }
            return "";
        }

        private int GetDebuffTurn(DebuffType type)
        {
            switch (type)
            {
                case DebuffType.Bleeding: return 10;
                case DebuffType.Poisoning: return 8;
                case DebuffType.Hunger: return 1;
                case DebuffType.Starvation: return 5;
            }
            return 0;
        }
    }

    public class DebuffComparer : IEqualityComparer<Debuff>
    {
        public bool Equals(Debuff x, Debuff y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(Debuff obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
