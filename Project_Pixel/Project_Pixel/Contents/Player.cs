using Project_Pixel.Contents.Debuff_System;
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
            Status = new PlayerStat(50, 3, 0, 10);      // 체력, 공격력, 방어력, 치명타 확률      
        }

        public override void OnDamaged(Character attacker)
        {
            Random random = new Random();
            int critical = random.Next(0, 101);
            int damage = (attacker.GetPower() - Status.Defense <= 0) ? 0 : attacker.GetPower() - Status.Defense;

            if (critical <= attacker.Status.CriChance)
            {
                damage = (int)(damage * attacker.Status.CriDamageValue);
            }

            Status.NowHp -= damage;

            if (IsDead())
            {
                Status.NowHp = 0;
            }
        }

        public void OnDamaged(int damage = 1)
        {
            Status.NowHp -= damage;

            if (IsDead())
            {
                Status.NowHp = 0;
            }
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

        public void OnAdjustHunger(int amount = -1)
        {
            Status.Hungry += amount;

            if (GetHungry() >= GetMaxHungry())
            {
                Status.Hungry = GetMaxHungry();
                RemoveDebuff(DebuffType.Hunger);
                RemoveDebuff(DebuffType.Starvation);
            }
            else if (GetHungry() <= 0)
            {
                Status.Hungry = 0;

                // TODO: 0% 되면 굶주림 상태
                OnDebuffDamage(DebuffType.Starvation);
            }
            else if (GetHungry() <= GetMaxHungry() / 3)
            {   // TODO: 30% 아래가 되면 배고픔 상태
                OnDebuffDamage(DebuffType.Hunger);
            }

            Managers.UI.Print_Status(this);
        }

        public void OnDebuffDamage(DebuffType type)
        {
            SetDebuff(type);

            Debuff debuff = MyDebuffs.Where(what => what.Type == type).First();
            if(debuff != null)
            {
                if(debuff.Turn > 0)
                {
                    debuff.DecreaseTurn();
                    OnDamaged(debuff.GetDamage());
                    debuff.ClearTurn();
                }
                else
                {
                    MyDebuffs.Remove(debuff);
                }
            }

            Managers.UI.Print_Status(this);
            Managers.UI.Print_State(this);
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
