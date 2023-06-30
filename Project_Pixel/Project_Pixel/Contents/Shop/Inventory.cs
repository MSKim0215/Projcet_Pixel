using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Contents.Shop
{
    public class Inventory
    {
        public List<Item> MyItems { protected set; get; } = new List<Item>();

        public void GetItem(Item item)
        {
            if (!MyItems.Contains(item, new ItemNameComparer()))
            {
                MyItems.Add(item);
            }
            else
            {
                UseItem(item, 1);
            }
        }

        public void UseItem(int index, int count = -1)
        {
            MyItems[index].AddCount(count);
        }

        public void UseItem(Item item, int count = -1)
        {
            MyItems.Find(x => x.Info.Type == item.Info.Type).AddCount(count);
        }
    }

    public class PlayerInventory : Inventory
    {
        public int Gold { private set; get; } = 5000;      // 소지금액

        public PlayerInventory()
        {

        }

        public void ResumeGold(int gold)
        {
            //if(gold > 0)
            //{
            //    Managers.UI.Print_PlayerCardLog($"{gold} 골드를 획득했습니다.".PadRight(30, ' '));
            //}
            //else
            //{
            //    Managers.UI.Print_PlayerCardLog($"{(gold) * -1} 골드를 잃었습니다..".PadRight(30, ' '));
            //}

            Gold += gold;

            if (Gold <= 0)
            {
                Gold = 0;
            }
        }
    }
}
