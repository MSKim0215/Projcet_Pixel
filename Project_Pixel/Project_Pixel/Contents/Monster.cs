using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Pixel.Contents
{
    public enum MonsterType
    {
        None,
        Slime,
        PocketMouse,
        Skeleton
    }

    public enum MonsterTile
    {
        Slime, PocketMouse, Skeleton
    }

    public class Monster : Character
    {
        public MonsterType MonsterType { protected set; get; } = MonsterType.None;

        protected Monster(MonsterType type) : base(CharacterType.Monster)
        {
            MonsterType = type;
        }
    }

    public class Slime : Monster
    {
        public Slime(): base(MonsterType.Slime)
        {
            MonsterType = MonsterType.Slime;
            SetStatus(new Stat(100, 20, 1, 10));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }

    public class PocketMouse : Monster
    {
        public PocketMouse() : base(MonsterType.PocketMouse)
        {
            MonsterType = MonsterType.PocketMouse;
            SetStatus(new Stat(200, 25, 2, 30));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }
   
    public class Skeleton : Monster
    {
        public Skeleton() : base(MonsterType.Skeleton)
        {
            MonsterType = MonsterType.Skeleton;
            SetStatus(new Stat(300, 35, 3, 50));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }
}
