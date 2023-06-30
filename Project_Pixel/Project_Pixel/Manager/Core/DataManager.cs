using Project_Pixel.Contents;
using Project_Pixel.Contents.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Threading.Manager
{
    public class DataManager
    {
        // 플레이어 스탯 원본 데이터
        public PlayerStatData playerStatData;

        // 몬스터 스탯 원본 데이터
        public Dictionary<MonsterType, MonsterStatData> monsterStatDatas = new Dictionary<MonsterType, MonsterStatData>();

        // 아이템 스탯 원본 데이터
        public Dictionary<ItemType, ItemStatData> itemStatDatas = new Dictionary<ItemType, ItemStatData>();

        public void Init()
        {
            SetPlayerData();
            SetMonsterData();
            SetItemData();
        }

        private void SetPlayerData()
        {
            playerStatData = new PlayerStatData(50, 3, 0, 10, 1.5f, 350, 0);
        }

        public PlayerStat GetPlayerStatData()
        {
            if(playerStatData != null)
            {
                return new PlayerStat(
                        Managers.Data.playerStatData.MaxHp,
                        Managers.Data.playerStatData.Power,
                        Managers.Data.playerStatData.Defense,
                        Managers.Data.playerStatData.CriChance,
                        Managers.Data.playerStatData.HungryMax,
                        Managers.Data.playerStatData.CriDamageValue);      // 체력, 공격력, 방어력, 치명타 확률   
            }
            return null;
        }

        private void SetMonsterData()
        {
            monsterStatDatas.Add(MonsterType.Slime, new MonsterStatData("슬라임", 20, 1, 1, 10, 1.5f, 100));
            monsterStatDatas.Add(MonsterType.PocketMouse, new MonsterStatData("주머니 쥐", 25, 2, 0, 10, 1.5f, 150));
            monsterStatDatas.Add(MonsterType.Skeleton, new MonsterStatData("스켈레톤", 30, 3, 1, 10, 1.5f, 200));
        }

        public MonsterStat GetMonsterStatData(MonsterType type)
        {
            if(monsterStatDatas != null)
            {
                return new MonsterStat(
                    Managers.Data.monsterStatDatas[type].Name,
                    Managers.Data.monsterStatDatas[type].MaxHp,
                    Managers.Data.monsterStatDatas[type].Power,
                    Managers.Data.monsterStatDatas[type].Defense,
                    Managers.Data.monsterStatDatas[type].CriChance,
                    Managers.Data.monsterStatDatas[type].DropGold,
                    Managers.Data.monsterStatDatas[type].CriDamageValue
                    );
            }
            return null;
        }

        private void SetItemData()
        {
            itemStatDatas.Add(ItemType.LongSword, new ItemStatData("롱소드", 0, 1, 0, 0, 0, 150));
            itemStatDatas.Add(ItemType.ChainMale, new ItemStatData("사슬 갑옷", -5, 0, 1, 0, 0, 200));
            itemStatDatas.Add(ItemType.MagicGlove, new ItemStatData("마법이 깃든 장갑", 3, 0, 0, 5, 0, 250));
            itemStatDatas.Add(ItemType.RedPotion, new ItemStatData("회복 물약", 5, 0, 0, 0, 0, 50));
            itemStatDatas.Add(ItemType.Food, new ItemStatData("도시락", 0, 0, 0, 0, 0, 80, 50));
            itemStatDatas.Add(ItemType.Pendant, new ItemStatData("목걸이", 0, 0, 0, 10, 30, 400));
            itemStatDatas.Add(ItemType.Scroll, new ItemStatData("탈출 주문서", 0, 0, 0, 0, 0, 100));
        }

        public ItemStat GetItemStatData(ItemType type)
        {
            if(itemStatDatas != null)
            {
                return new ItemStat(
                    Managers.Data.itemStatDatas[type].Name,
                    Managers.Data.itemStatDatas[type].BuyGold,
                    Managers.Data.itemStatDatas[type].MaxHp,
                    Managers.Data.itemStatDatas[type].Power,
                    Managers.Data.itemStatDatas[type].Defense,
                    Managers.Data.itemStatDatas[type].CriChance,
                    Managers.Data.itemStatDatas[type].Hungry,
                    Managers.Data.itemStatDatas[type].CriDamageValue);
            }
            return null;
        }
    }
}
