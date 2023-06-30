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

        protected void SetDebuff(DebuffType debuff)
        {
            if (MyDebuffs.Contains(new Debuff(debuff), new DebuffComparer())) return;
            MyDebuffs.Add(new Debuff(debuff));
        }

        protected void RemoveDebuff(DebuffType debuff)
        {
            Debuff newDebuff = new Debuff(debuff);
            if (!MyDebuffs.Contains(newDebuff, new DebuffComparer())) return;

            List<Debuff> debuffs = MyDebuffs.ToList();
            for(int i = 0; i < debuffs.Count; i++)
            {
                if (debuffs[i].Name == newDebuff.Name)
                {
                    MyDebuffs.Remove(debuffs[i]);
                    break;
                }
            }
        }
    }
}
