using Project_Pixel.Contents.Debuff_System;
using Project_Pixel.Contents.Shop;
using Project_Pixel.Manager.Contents;
using Project_Pixel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Threading.Manager;

namespace Project_Pixel.Contents
{
    public enum Direct
    {
        Up, Down, Right, Left
    }

    public class Player : Character, IPlayerAttack
    {
        public PlayerInventory Inven { private set; get; }
        public new PlayerStat Status { private set; get; }

        public Player() : base(CharacterType.Player)
        {
            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);

            Status = Managers.Data.GetPlayerStatData();    
            Inven = new PlayerInventory(Status.StartGold);
        }

        public void Attack(Monster monster)
        {
            Managers.UI.Print_GameLog("플레이어의 공격!              ");
            monster.OnDamaged(this);
        }

        public void OnDamaged(Monster attacker)
        {
            Random random = new Random();
            int critical = random.Next(0, 101);
            int damage = (attacker.GetPower() - Status.Defense <= 0) ? 0 : attacker.GetPower() - Status.Defense;

            if (critical <= attacker.Status.CriChance)
            {
                damage = (int)(damage * attacker.Status.CriDamageValue);
            }

            Status.NowHp -= damage;
            Managers.UI.Print_GameLog($"{attacker.Name} {Math.Max(0, damage)} 피해를 받았습니다.              ");

            if (IsDead())
            {
                OnDead();
            }
        }

        public void OnDamaged(int damage = 1)
        {
            Status.NowHp -= damage;

            if (IsDead())
            {
                OnDead();
            }
        }

        private void OnDead()
        {
            Status.NowHp = 0;
            Managers.UI.Print_GameLog($"플레이어가 죽었습니다.              ");

            Managers.Game.MapManager.Maps[CurrPos.X, CurrPos.Y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
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
            else if (GetHungry() < GetMaxHungry() / 3)
            {   // TODO: 30% 아래가 되면 배고픔 상태
                RemoveDebuff(DebuffType.Starvation);
                OnDebuffDamage(DebuffType.Hunger);
            }
            else if(GetHungry() >= GetMaxHungry() / 3)
            {
                RemoveDebuff(DebuffType.Hunger);
                RemoveDebuff(DebuffType.Starvation);
                Managers.UI.Print_State(this);
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

        public void OnHealing(int heal)
        {
            Status.NowHp += heal;

            if (Status.NowHp >= Status.MaxHp)
            {
                Status.NowHp = Status.MaxHp;
            }
        }

        public void ResumeGold(int gold) => Inven.ResumeGold(gold);

        public int GetHungry() => Status.Hungry;
        public int GetMaxHungry() => Status.HungryMax; 
        public override int GetNowHp() => Status.NowHp;
        public override int GetMaxHp() => Status.MaxHp;
        public override int GetPower() => Status.Power;
        public override int GetDefense() => Status.Defense;
        public override float GetCriChance() => Status.CriChance;
        public override float GetCriDamageValue() => Status.CriDamageValue;
        public override bool IsDead() => Status.NowHp <= 0;
    }
}
