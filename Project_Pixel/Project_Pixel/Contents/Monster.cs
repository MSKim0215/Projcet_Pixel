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

    public class Monster : Character
    {
        private List<Node> targetPath = new List<Node>();
        private MonsterType monsterType = MonsterType.None;
        private string name;


        public MonsterType MonsterType
        {
            get => monsterType;
            set
            {
                monsterType = value;

                switch(monsterType)
                {
                    case MonsterType.Slime: name = "슬라임"; break;
                    case MonsterType.PocketMouse: name = "주머니 쥐"; break;
                    case MonsterType.Skeleton: name = "스켈레톤"; break;
                }
            }
        }

        public int SightRange { protected set; get; } = 5;

        protected Monster(MonsterType type) : base(CharacterType.Monster)
        {
            MonsterType = type;

            CurrPos = new Position(0, 0);
            PrevPos = new Position(0, 0);
        }

        public bool IsPlayerInSight()
        {
            var playerPos = Managers.Game.Player.CurrPos;
            bool isInXRange = Math.Abs(CurrPos.X - playerPos.X) <= SightRange;
            bool isInYRange = Math.Abs(CurrPos.Y - playerPos.Y) <= SightRange;

            if (isInXRange && isInYRange)
            {
                //if (IsThereAWallBetween(CurrPos, playerPos))
                //{
                //    return false;
                //}

                Managers.UI.Print_GameLog($"{name}을 만났습니다.                 ");

                targetPath = PathManager.FindPath(CurrPos, playerPos);
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
                else if(targetPath.Count >= 2)
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
    }

    public class Slime : Monster
    {
        public Slime(): base(MonsterType.Slime)
        {
            MonsterType = MonsterType.Slime;
            SetStatus(new Stat(100, 1, 1, 10));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }

    public class PocketMouse : Monster
    {
        public PocketMouse() : base(MonsterType.PocketMouse)
        {
            MonsterType = MonsterType.PocketMouse;
            SetStatus(new Stat(200, 2, 2, 30));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }
   
    public class Skeleton : Monster
    {
        public Skeleton() : base(MonsterType.Skeleton)
        {
            MonsterType = MonsterType.Skeleton;
            SetStatus(new Stat(300, 3, 3, 50));        // 체력, 공격력, 방어력, 치명타 확률
        }
    }
}
