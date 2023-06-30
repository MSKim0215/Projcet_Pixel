using Project_Pixel.Contents.Debuff_System;
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

    public enum NPCTile
    {
        Paddler
    }

    public class Character
    {
        protected Direct direct;
        protected CharacterType characterType = CharacterType.None;
 
        public Stat Status { set; get; }
        public Position CurrPos { set; get; }
        public Position PrevPos { set; get; }

        public HashSet<Debuff> MyDebuffs { protected set; get; } = new HashSet<Debuff>();

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
        }

        public void SetStatus(Stat stat)
        {
            Status = stat;
        }

        protected void SetDebuff(DebuffType debuff)
        {
            if (MyDebuffs.Contains(new Debuff(debuff), new DebuffComparer())) return;
            MyDebuffs.Add(new Debuff(debuff));
        }

        protected void RemoveDebuff(DebuffType debuff)
        {
            if (!MyDebuffs.Contains(new Debuff(debuff), new DebuffComparer())) return;
            MyDebuffs.Remove(new Debuff(debuff));
        }

        public void OnHealing()
        {
            Status.NowHp += 30;

            if (Status.NowHp >= Status.MaxHp)
            {
                Status.NowHp = Status.MaxHp;
            }
        }

        public virtual int GetNowHp() => Status.NowHp;
        public virtual int GetMaxHp() => Status.MaxHp;
        public virtual int GetPower() => Status.Power;
        public virtual int GetDefense() => Status.Defense;
        public virtual float GetCriChance() => Status.CriChance;
        public virtual float GetCriChanceMax() => Status.CriChanceMax;
        public virtual float GetCriDamageValue() => Status.CriDamageValue;
        public virtual bool IsDead() => Status.NowHp <= 0;
    }
}
