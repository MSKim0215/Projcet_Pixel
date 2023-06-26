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
        Abrelshud,
        Illiakan,
        Lazalam
    }

    public class Monster : Character
    {
        public MonsterType MonsterType { protected set; get; } = MonsterType.None;

        public string textScript = null;

        protected Monster(MonsterType type) : base(CharacterType.Monster)
        {
            MonsterType = type;
        }
    }

    public class Abrelshud : Monster
    {
        public Abrelshud(): base(MonsterType.Abrelshud)
        {
            MonsterType = MonsterType.Abrelshud;
            SetStatus(new Stat(100, 20, 1, 10));        // 체력, 공격력, 방어력, 치명타 확률

            textScript = "깰 수 없는 꿈은 현실일지니";
        }
    }

    public class Illiakan : Monster
    {
        public Illiakan() : base(MonsterType.Illiakan)
        {
            MonsterType = MonsterType.Illiakan;
            SetStatus(new Stat(200, 25, 2, 30));        // 체력, 공격력, 방어력, 치명타 확률

            textScript = "네놈들의 끈질긴 운명도, 공허에 휩쓸려 가라앉을 것이다!";
        }
    }
   
    public class Lazalam : Monster
    {
        public Lazalam() : base(MonsterType.Lazalam)
        {
            MonsterType = MonsterType.Lazalam;
            SetStatus(new Stat(300, 35, 3, 50));        // 체력, 공격력, 방어력, 치명타 확률

            textScript = ". . .";
        }
    }
}
