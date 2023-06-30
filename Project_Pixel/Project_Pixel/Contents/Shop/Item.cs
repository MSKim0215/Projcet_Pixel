using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents.Shop
{
    public enum ItemType
    {
        LongSword,      // 롱소드
        ChainMale,      // 사슬갑옷
        MagicGlove,     // 마법이 깃든 장갑
        RedPotion,      // 빨간 물약
        Food,           // 도시락
        Pendant,        // 목걸이
        Scroll          // 탈출 스크롤
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
            return x.Info.Stat.Name == y.Info.Stat.Name;
        }

        public int GetHashCode(Item obj)
        {
            return obj.Info.Stat.Name.GetHashCode();
        }
    }
}
