using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents.Shop
{
    public enum ItemTypes
    {
        Staff,              // 스태프
        WizardRobe,         // 마법사 로브
        SpellBook,          // 마법서
        RedPotion,          // 빨간 물약
        Elixir,             // 엘릭서
        Pendant             // 목걸이
    }

    public class ItemData
    {
        public ItemTypes Type { private set; get; }
        public string Name { private set; get; }
        public int Price { private set; get; }
        public Stat Stat { private set; get; }

        public void Init(ItemTypes type, int price)
        {
            Type = type;
            Name = TranslationName();
            Price = price;
            SetStatus();
        }

        private string TranslationName()
        {
            switch(Type)
            {
                case ItemTypes.Staff: return "스태프";
                case ItemTypes.WizardRobe: return "마법사 로브";
                case ItemTypes.SpellBook: return "마법서";
                case ItemTypes.RedPotion: return "빨간 물약";
                case ItemTypes.Elixir: return "엘릭서";
                case ItemTypes.Pendant: return "목걸이";
            }
            return null;
        }

        private void SetStatus()
        {
            switch (Type)
            {
                case ItemTypes.Staff:
                    {
                        Stat = new Stat(0, 10, 0, 0, 0, 0);
                    }
                    break;
                case ItemTypes.WizardRobe:
                    {
                        Stat = new Stat(7, 0, 3, 0, 0, 0);
                    }
                    break;
                case ItemTypes.SpellBook:
                    {
                        Stat = new Stat(0, 5, 0, 5, 0, 0);
                    }
                    break;
                case ItemTypes.RedPotion:
                    {
                        Stat = new Stat(10, 0, 0, 0, 0, 0);
                    }
                    break;
                case ItemTypes.Elixir:
                    {
                        Stat = new Stat(3, 3, 3, 1, 0, 0);
                    }
                    break;
                case ItemTypes.Pendant:
                    {
                        Stat = new Stat(0, 0, 0, 10, 0, 20);
                    }
                    break;
            }
        }

        public string GetItemStatus()
        {
            switch (Type)
            {
                case ItemTypes.Staff:
                    {
                        return $"공격력 +{Stat.Power}";
                    }
                case ItemTypes.WizardRobe:
                    {
                        return $"최대 체력 +{Stat.MaxHp}  방어력 +{Stat.Defense}";
                    }
                case ItemTypes.SpellBook:
                    {
                        return $"공격력 +{Stat.Power}  치명타 확률 +{Stat.CriChance}%";
                    }
                case ItemTypes.RedPotion:
                    {
                        return $"최대 체력 +{Stat.MaxHp}";
                    }
                case ItemTypes.Elixir:
                    {
                        return $"최대 체력 +{Stat.MaxHp}   공격력 +{Stat.Power}   방어력 +{Stat.Defense}   치명타 확률 +{Stat.CriChance}%";
                    }
                case ItemTypes.Pendant:
                    {
                        return $"치명타 확률 +{Stat.CriChance}%   치명타 데미지 +{Stat.CriDamageValue}%";
                    }
            }
            return null;
        }
    }

    public class Item
    {
        public ItemData Info { protected set; get; }
        public int Count { protected set; get; }

        public void Init(ItemData data, int count = 1)
        {
            Info = data;
            Count = count;
        }

        public void AddCount(int count = 1) => Count += count;
    }

    public class ItemNameComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item x, Item y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            return x.Info.Name == y.Info.Name;
        }

        public int GetHashCode(Item obj)
        {
            return obj.Info.Name.GetHashCode();
        }
    }
}
