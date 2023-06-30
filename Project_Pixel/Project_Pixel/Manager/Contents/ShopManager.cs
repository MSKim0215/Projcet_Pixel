using Project_Pixel.Contents.Shop;
using Project_Pixel.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;
using System.Reflection;

namespace Project_Pixel.Manager.Contents
{
    public static class ShopManager
    {
        public static void Trade()
        {
            Managers.UI.Print_GameLog("거래를 진행합니다.                      ");

            //MakeShopItem();
        }

        private static void MakeShopItem()
        {
            Random rand = new Random();
            if (Managers.Game.Peddler.Inven.MyItems.Count > 0) Managers.Game.Peddler.Inven.MyItems.Clear();

            for (int i = 0; i < 3; i++)
            {
                Item newItem = new Item();
                while (true)
                {
                    int index = rand.Next(0, (int)ItemType.Scroll + 1);
                    ItemData data = new ItemData();
                    data.Init((ItemType)index);
                    newItem.Init(data);

                    if (!Managers.Game.Peddler.Inven.MyItems.Contains(newItem, new ItemNameComparer())) break;
                }
                Managers.Game.Peddler.Inven.GetItem(newItem);
            }
        }

        public static void Buy(int itemIndex)
        {
            Item buyItem = Managers.Game.Peddler.Inven.MyItems[itemIndex];
            Item newItem = new Item();

            ItemData data = new ItemData();
            data.Init((ItemType)itemIndex);
            newItem.Init(data);

            if (buyItem.Info.Stat.BuyGold <= Managers.Game.Player.Inven.Gold)
            {
                Managers.Game.Peddler.Inven.UseItem(itemIndex);
                Managers.Game.Player.Inven.GetItem(newItem);
                Managers.Game.Player.Inven.ResumeGold(-newItem.Info.Stat.BuyGold);

                //Managers.UI.Print_Shop_PeddlerInventory(peddler);
                //Managers.UI.Print_PlayerInventory(player);

                Managers.Game.Player.UpperStatus(buyItem.Info.Stat);
                Managers.UI.Print_Status(Managers.Game.Player);
            }
            else
            {
                Managers.UI.Print_GameLog("소지금이 부족합니다.".PadRight(30, ' '));
            }
        }
    }
}
