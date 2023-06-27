using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Contents
{
    public enum CharacterType
    {
        None, Player, Monster, NPC
    }

    public class Character
    {
        protected Direct direct;

        protected CharacterType characterType = CharacterType.None;
        protected Stat stat;

        public int SightRange { protected set; get; }

        public Position CurrPos { set; get; }
        public Position PrevPos { set; get; }

        public Direct Direct
        {
            get => direct;
            set
            {
                direct = value;
            }
        }

        protected Character(CharacterType type)
        {
            characterType = type;

            SightRange = 5;
        }

        public void SetStatus(Stat stat)
        {
            this.stat = stat;
        }

        public void OnDamaged(Character attacker)
        {
            Random random = new Random();
            int critical = random.Next(0, 101);
            int damage = (attacker.GetPower() - stat.Defense <= 0) ? 0 : attacker.GetPower() - stat.Defense;

            if(critical <= attacker.stat.CriChance)
            {
                damage = (int)(damage * attacker.stat.CriDamageValue);
            }

            stat.NowHp -= damage;

            if(IsDead())
            {
                stat.NowHp = 0;
            }
        }

        public void OnHealing()
        {
            stat.NowHp += 30;

            if(stat.NowHp >= stat.MaxHp)
            {
                stat.NowHp = stat.MaxHp;
            }
        }

        public int GetNowHp() => stat.NowHp;
        public int GetMaxHp() => stat.MaxHp;
        public int GetPower() => stat.Power;
        public int GetDefense() => stat.Defense;
        public float GetCriChance() => stat.CriChance;
        public float GetCriChanceMax() => stat.CriChanceMax;
        public float GetCriDamageValue() => stat.CriDamageValue;
        public bool IsDead() => stat.NowHp <= 0;
    }
}
