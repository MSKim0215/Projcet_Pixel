using Project_Pixel.Contents.Debuff_System;
using Project_Pixel.Manager.Contents;
using Project_Pixel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

    public class Monster : Character, IMonsterAttack
    {
        protected List<Node> targetPath = new List<Node>();
        protected MonsterType monsterType = MonsterType.None;
        protected bool isMeet = false;
        
        public MonsterStat Status { protected set; get; }
        public MonsterType MonsterType { private set; get; }
        public int SightRange { protected set; get; } = 5;

        protected Monster(MonsterType type) : base(CharacterType.Monster)
        {
            MonsterType = type;

            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);

            Status = Managers.Data.GetMonsterStatData(MonsterType);
        }

        public bool IsPlayerInSight()
        {
            var playerPos = Managers.Game.Player.CurrPos;
            bool isInXRange = Math.Abs(CurrPos.X - playerPos.X) <= SightRange;
            bool isInYRange = Math.Abs(CurrPos.Y - playerPos.Y) <= SightRange;

            if (isInXRange && isInYRange)
            {
                if (IsThereAWallBetween(CurrPos, playerPos))
                {
                    return false;
                }

                if (!isMeet)
                {
                    isMeet = true;
                    Managers.UI.Print_GameLog($"{Status.Name}을 만났습니다.                       ");
                }

                targetPath = PathManager.FindPath(CurrPos, playerPos, SightRange + 1);
                if(targetPath != null)
                {
                    if (targetPath.Count == 1)
                    {
                        if (targetPath[0].Position != Managers.Game.Player.CurrPos)
                        {
                            PrevPos = CurrPos;

                            if (Managers.Game.MapManager.Maps[targetPath[0].Position.X, targetPath[0].Position.Y] == Managers.UI.TilePatterns[(int)TileTypes.Wall] ||
                                Managers.Game.MapManager.Maps[targetPath[0].Position.X, targetPath[0].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Slime] ||
                                Managers.Game.MapManager.Maps[targetPath[0].Position.X, targetPath[0].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.PocketMouse] ||
                                Managers.Game.MapManager.Maps[targetPath[0].Position.X, targetPath[0].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Skeleton] ||
                                Managers.Game.MapManager.Maps[targetPath[0].Position.X, targetPath[0].Position.Y] == Managers.UI.NPCPatterns[(int)NPCTile.Paddler])
                            {
                                return true;
                            }
                            CurrPos = targetPath[0].Position;

                            Util.Swap(ref Managers.Game.MapManager.Maps[CurrPos.X, CurrPos.Y], ref Managers.Game.MapManager.Maps[PrevPos.X, PrevPos.Y]);
                            return true;
                        }
                    }
                    else if (targetPath.Count >= 2)
                    {
                        if (targetPath[1].Position != Managers.Game.Player.CurrPos)
                        {
                            PrevPos = CurrPos;

                            if (Managers.Game.MapManager.Maps[targetPath[1].Position.X, targetPath[1].Position.Y] == Managers.UI.TilePatterns[(int)TileTypes.Wall] ||
        Managers.Game.MapManager.Maps[targetPath[1].Position.X, targetPath[1].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Slime] ||
        Managers.Game.MapManager.Maps[targetPath[1].Position.X, targetPath[1].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.PocketMouse] ||
        Managers.Game.MapManager.Maps[targetPath[1].Position.X, targetPath[1].Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Skeleton] ||
        Managers.Game.MapManager.Maps[targetPath[1].Position.X, targetPath[1].Position.Y] == Managers.UI.NPCPatterns[(int)NPCTile.Paddler])
                            {
                                return true;
                            }


                            CurrPos = targetPath[1].Position;

                            Util.Swap(ref Managers.Game.MapManager.Maps[CurrPos.X, CurrPos.Y], ref Managers.Game.MapManager.Maps[PrevPos.X, PrevPos.Y]);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsThereAWallBetween(Position start, Position end)
        {
            int minX = Math.Min(start.X, end.X); // 시작 위치와 끝 위치의 X 좌표 중 작은 값
            int maxX = Math.Max(start.X, end.X); // 시작 위치와 끝 위치의 X 좌표 중 큰 값
            int minY = Math.Min(start.Y, end.Y); // 시작 위치와 끝 위치의 Y 좌표 중 작은 값
            int maxY = Math.Max(start.Y, end.Y); // 시작 위치와 끝 위치의 Y 좌표 중 큰 값

            // 정사각형 영역 내의 모든 좌표를 확인합니다.
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    // 현재 좌표에 벽이 있는지 확인합니다.
                    if (Managers.Game.MapManager.Maps[x, y] == Managers.UI.TilePatterns[(int)TileTypes.Wall])
                    {
                        return true; // 시작 위치와 끝 위치 사이에 벽이 있습니다.
                    }
                }
            }
            return false; // 시작 위치와 끝 위치 사이에 벽이 없습니다.
        }

        public virtual void Attack(Player player)
        {
            Managers.UI.Print_GameLog($"{Status.Name} 공격!                        ");
            player.OnDamaged(this);
        }

        public virtual void OnDamaged(Player attacker)
        {
            Random random = new Random();
            int critical = random.Next(0, 101);
            int damage = (attacker.Status.Power - Status.Defense <= 0) ? 0 : attacker.Status.Power - Status.Defense;

            if (critical <= attacker.Status.CriChance)
            {
                damage = (int)(damage * attacker.Status.CriDamageValue);
            }

            Status.NowHp -= damage;

            Managers.UI.Print_GameLog($"플레이어에게 {Math.Max(0, damage)} 피해를 받았습니다.                 ");
            Managers.UI.Print_GameLog($"{Status.Name}의 남은 체력: {Status.NowHp}                  ");

            if (Status.NowHp <= 0)
            {
                OnDead();
            }
        }

        public void OnDebuffDamage(DebuffType type)
        {
            SetDebuff(type);

            Debuff debuff = MyDebuffs.Where(what => what.Type == type).First();
            if (debuff != null)
            {
                if (debuff.Turn > 0)
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
        }

        public void OnDamaged(int damage = 1)
        {
            Status.NowHp -= damage;

            Managers.UI.Print_GameLog($"상태이상 {Math.Max(0,damage)} 피해를 받았습니다.                ");
            Managers.UI.Print_GameLog($"{Status.Name}의 남은 체력: {Status.NowHp}                  ");

            if (Status.NowHp <= 0)
            {
                OnDead();
            }
        }

        private void OnDead()
        {
            Status.NowHp = 0;
            Managers.UI.Print_GameLog($"{Status.Name}가 죽었습니다.                  ");

            Managers.Game.MapManager.Maps[CurrPos.X, CurrPos.Y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
            Managers.Game.Monsters.Remove(this);
        }
    }

    public class Slime : Monster
    {
        public Slime(): base(MonsterType.Slime) { }

        public override void Attack(Player player)
        {
            base.Attack(player);

            Random rand = new Random();
            if (rand.Next(0, 101) < 30)
            {
                Managers.Game.Player.OnDebuffDamage(DebuffType.Poisoning);
            }
        }
    }

    public class PocketMouse : Monster
    {
        public PocketMouse() : base(MonsterType.PocketMouse) { }

        public override void Attack(Player player)
        {
            base.Attack(player);

            Random rand = new Random();
            if (rand.Next(0, 101) < 30)
            {
                Managers.Game.Player.OnDebuffDamage(DebuffType.Bleeding);
            }
        }
    }
   
    public class Skeleton : Monster
    {
        public Skeleton() : base(MonsterType.Skeleton) { }
    }
}
