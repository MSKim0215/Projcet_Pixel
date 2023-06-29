using Project_Pixel.Manager.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

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
        private List<Node> targetPath = new List<Node>();

        public MonsterType MonsterType { protected set; get; } = MonsterType.None;
        public int SightRange { protected set; get; } = 5;

        protected Monster(MonsterType type) : base(CharacterType.Monster)
        {
            MonsterType = type;

            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);
        }

        public bool IsPlayerInSight()
        {
            bool isInXRange = Math.Abs(CurrPos.X - Managers.Game.Player.CurrPos.X) <= SightRange;
            bool isInYRange = Math.Abs(CurrPos.Y - Managers.Game.Player.CurrPos.Y) <= SightRange;

            if (isInXRange && isInYRange)
            {
                Managers.UI.Print_GameLog("플레이어가 몬스터의 시야 범위 내에 있습니다.");

                targetPath = PathManager.FindPath(CurrPos, Managers.Game.Player.CurrPos);
                


                return true;
            }

            return false;
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
