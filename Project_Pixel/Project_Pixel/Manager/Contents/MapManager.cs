using Project_Pixel.Contents;
using Project_Pixel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Threading;
using Threading.Manager;

namespace Project_Pixel.Manager.Contents
{
    public enum TileTypes
    {
        Wall, Empty, Fog
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class MapManager : Core
    {
        public const int MAP_WIDTH = 60;
        public const int MAP_HEIGHT = 45;

        private Random random = new Random();

        private int splitCount = 0, splitCountMax = 300;

        private List<Room> rooms = new List<Room>();
        private List<Corridor> corridors = new List<Corridor>();
        
        public string[,] Maps { private set; get; }
        public bool[,] VisitedMaps { private set; get; }

        public MapManager()
        {
            Maps = new string[MAP_WIDTH, MAP_HEIGHT];
            VisitedMaps = new bool[MAP_WIDTH, MAP_HEIGHT];

            Start();
        }

        public void Generate()
        {
            while (true)
            {
                splitCount = 0;
                if (rooms.Count > 0) rooms.Clear();
                if (corridors.Count > 0) corridors.Clear();

                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Maps[x, y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                        VisitedMaps[x, y] = false;
                    }
                }

                SplitRegion(new Position(0, 0), MAP_WIDTH, MAP_HEIGHT);
                RemoveRoom();

                rooms.Sort((a, b) => a.Position.X != b.Position.X ? a.Position.X.CompareTo(b.Position.X) : a.Position.Y.CompareTo(b.Position.Y));

                for (int i = 0; i < rooms.Count - 1; i++)
                {
                    DrawCorridor(rooms[i], rooms[i + 1]);
                }

                DrawWallsAroundCorridors();

                if(rooms.Count > 5)
                {
                    Managers.Game.Player.CurrPos = rooms.First().CenterPosition;
                    Managers.Game.Player.PrevPos = rooms.Last().CenterPosition;
                    Managers.Game.Player.Direct = Direct.Up;
                    Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y] = Managers.UI.PlayerPatterns[(int)Direct.Up];

                    Room currentRoom = GetRoomAtPosition(Managers.Game.Player.CurrPos);
                    if (currentRoom != null && !currentRoom.isVisited)
                    {
                        currentRoom.isVisited = true;

                        for (int y = currentRoom.Position.Y; y < currentRoom.Position.Y + currentRoom.Height; y++)
                        {
                            for (int x = currentRoom.Position.X; x < currentRoom.Position.X + currentRoom.Width; x++)
                            {
                                VisitedMaps[x, y] = true;
                            }
                        }
                    }

                    int randomIndex = random.Next(0, rooms.Count);
                    rooms[randomIndex].RefreshSubPosition();
                    while (rooms[randomIndex].SubPosition == Managers.Game.Player.CurrPos)
                    {
                        randomIndex = random.Next(0, rooms.Count);
                    }

                    Maps[rooms[randomIndex].SubPosition.X, rooms[randomIndex].SubPosition.Y] = Managers.UI.NPCPatterns[(int)NPCTile.Paddler];
                    Managers.Game.Peddler.CurrPos = new Position(rooms[randomIndex].SubPosition.X, rooms[randomIndex].SubPosition.Y);

                    Thread.Sleep(10);

                    for (int i = 0; i < Managers.Game.Monsters.Length; i++)
                    {
                        randomIndex = random.Next(0, rooms.Count);
                        rooms[randomIndex].RefreshSubPosition();
                        while (rooms[randomIndex].SubPosition == Managers.Game.Player.CurrPos)
                        {
                            randomIndex = random.Next(0, rooms.Count);
                        }

                        Thread.Sleep(10);

                        int kind = random.Next(0, 3);
                        Maps[rooms[randomIndex].SubPosition.X, rooms[randomIndex].SubPosition.Y] = Managers.UI.MonsterPatterns[kind];
                        
                        switch(kind)
                        {
                            case (int)MonsterTile.Slime: Managers.Game.Monsters[i] = new Slime(); break;
                            case (int)MonsterTile.PocketMouse: Managers.Game.Monsters[i] = new PocketMouse(); break;
                            case (int)MonsterTile.Skeleton: Managers.Game.Monsters[i] = new Skeleton(); break;
                        }

                        Managers.Game.Monsters[i].CurrPos = new Position(rooms[randomIndex].SubPosition.X, rooms[randomIndex].SubPosition.Y);
                    }

                    PrintMap();
                    break;
                }
            }
        }

        private void SplitRegion(Position position, int width, int height)
        {
            if (splitCount >= splitCountMax) return;
            if ((width < 20 && height < 15) || (height < 20 && width < 15))
            {
                Room newRoom = new Room(position, width, height);
                rooms.Add(newRoom);

                for(int i =  position.X; i < position.X + width; i++)
                {
                    Maps[i, position.Y] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                    Maps[i, position.Y + height - 1] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                }

                for (int i = position.Y; i < position.Y + height; i++)
                {
                    Maps[position.X, i] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                    Maps[position.X + width - 1, i] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                }
                return;
            }

            splitCount++;

            bool isHoriSplit;

            if (width > height) isHoriSplit = false;
            else if (width < height) isHoriSplit = true;
            else
            {
                isHoriSplit = random.Next(0, 2) == 0;
            }

            if(isHoriSplit)
            {   // 수평 분할
                int split = random.Next(Math.Min(position.Y + 3, position.Y + height - 4),
                    Math.Max(position.Y + 3, position.Y + height - 4));
                SplitRegion(position, width, split - position.Y);
                SplitRegion(new Position(position.X, split + 1), width, position.Y + height - split - 1);
            }
            else
            {   // 수직 분할
                int split = random.Next(Math.Min(position.X + 3, position.X + width - 4),
                    Math.Max(position.X + 3, position.X + width - 4)); 
                SplitRegion(position, split - position.X, height);
                SplitRegion(new Position(split + 1, position.Y), position.X + width - split - 1, height);
            }
        }

        private void RemoveRoom()
        {
            List<Room> removeRooms = new List<Room>();

            foreach(Room room in rooms)
            {
                int value = room.Width * room.Height;
                if(value <= 150)
                {
                    for (int i = room.Position.X; i < room.Position.X + room.Width; i++)
                    {
                        Maps[i, room.Position.Y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                        Maps[i, room.Position.Y + room.Height - 1] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                    }

                    for (int i = room.Position.Y; i < room.Position.Y + room.Height; i++)
                    {
                        Maps[room.Position.X, i] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                        Maps[room.Position.X + room.Width - 1, i] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                    }

                    Maps[room.CenterPosition.X, room.CenterPosition.Y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];

                    removeRooms.Add(room);
                }
            }

            for(int i = 0; i < removeRooms.Count; i++)
            {
                rooms.Remove(removeRooms[i]);
            }
        }

        private void DrawCorridor(Room room1, Room room2)
        {
            room1.RefreshSubPosition();
            room2.RefreshSubPosition();

            Position point1 = room1.SubPosition;
            Position point2 = room2.SubPosition;

            List<List<Position>> allPositions = new List<List<Position>>();     // 모든 통로의 좌표 리스트
            List<Position> positions = new List<Position>();                    // 하나의 통로에 소속된 좌표

            List<List<Position>> allArounds = new List<List<Position>>();       // 모든 통로 주변 벽의 좌표 리스트

            for (int y = Math.Min(point1.Y, point2.Y); y <= Math.Max(point1.Y, point2.Y); y++)
            {
                Position corridorPos = new Position(point1.X, y);
                Room currentRoom = GetRoomAtPositionCor(corridorPos);

                if (currentRoom == null)
                {
                    Maps[point1.X, y] = "☆";
                    positions.Add(corridorPos);
                }
                else if (currentRoom != null && positions.Count > 0)
                {
                    positions.Add(corridorPos);

                    List<Position> temps = new List<Position>();
                    for(int i = 0; i < positions.Count; i++)
                    {
                        temps.Add(positions[i]);
                    }

                    List<Position> arounds = new List<Position>();                      // 하나의 통로 주변에 소속된 좌표 리스트
                    foreach (Position corPos in positions)
                    {
                        for (int b = corPos.Y - 1; b <= corPos.Y + 1; b++)
                        {
                            for (int a = corPos.X - 1; a <= corPos.X + 1; a++)
                            {
                                Position wallPos = new Position(a, b);
                                if (!positions.Contains(wallPos, new CorridorPosComparer()) && 
                                    !IsPositionInsideAnyRoom(wallPos))
                                {
                                    arounds.Add(wallPos);
                                }
                            }
                        }
                    }

                    List<Position> aroundTemps = new List<Position>();
                    for (int i = 0; i < arounds.Count; i++)
                    {
                        aroundTemps.Add(arounds[i]);
                    }

                    allPositions.Add(temps);
                    allArounds.Add(aroundTemps);
                    positions.Clear();
                    arounds.Clear();
                }
            }

            for (int x = Math.Min(point1.X, point2.X); x <= Math.Max(point1.X, point2.X); x++)
            {
                Position corridorPos = new Position(x, point2.Y);
                Room currentRoom = GetRoomAtPositionCor(corridorPos);

                if (currentRoom == null)
                {
                    Maps[x, point2.Y] = "☆";
                    positions.Add(corridorPos);
                }
                else if (currentRoom != null && positions.Count > 0)
                {
                    positions.Add(corridorPos);

                    List<Position> temps = new List<Position>();
                    for (int i = 0; i < positions.Count; i++)
                    {
                        temps.Add(positions[i]);
                    }

                    List<Position> arounds = new List<Position>();                      // 하나의 통로 주변에 소속된 좌표 리스트
                    foreach (Position corPos in positions)
                    {
                        for (int b = corPos.Y - 1; b <= corPos.Y + 1; b++)
                        {
                            for (int a = corPos.X - 1; a <= corPos.X + 1; a++)
                            {
                                Position wallPos = new Position(a, b);
                                if (!positions.Contains(wallPos, new CorridorPosComparer()) &&
                                    !IsPositionInsideAnyRoom(wallPos))
                                {
                                    arounds.Add(wallPos);
                                }
                            }
                        }
                    }

                    List<Position> aroundTemps = new List<Position>();
                    for (int i = 0; i < arounds.Count; i++)
                    {
                        aroundTemps.Add(arounds[i]);
                    }

                    allPositions.Add(temps);
                    allArounds.Add(aroundTemps);
                    positions.Clear();
                    arounds.Clear();
                }
            }

            for (int i = 0; i < allPositions.Count; i++)
            {
                Corridor corridor = new Corridor();
                corridor.SetPosition(allPositions[i]);
                corridor.SetAroundPosition(allArounds[i]);
                corridors.Add(corridor);
            }
        }

        private void DrawWallsAroundCorridors()
        {
            foreach (Corridor corridor in corridors)
            {
                foreach(Position aroundPos in corridor.AroundPositions)
                {
                    if (!IsPositionInsideAnyRoom(aroundPos) &&
                        corridor.AroundPositions.Contains(aroundPos, new CorridorPosComparer()) &&
                        Maps[aroundPos.X, aroundPos.Y] == Managers.UI.TilePatterns[(int)TileTypes.Empty])
                    {
                        Maps[aroundPos.X, aroundPos.Y] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                    }
                }
            }
        }

        private bool IsPositionInsideAnyRoom(Position position)
        {
            foreach (Room room in rooms)
            {
                if (position.X > room.Position.X - 1 && position.X < room.Position.X + room.Width &&
                    position.Y > room.Position.Y - 1 && position.Y < room.Position.Y + room.Height)
                {
                    return true;
                }
            }
            return false;
        }

        private void PrintMap()
        {
            int startX = 5;
            int startY = 3;

            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Console.SetCursorPosition(startX, startY + y);

                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    if (x == Managers.Game.Player.CurrPos.X && y == Managers.Game.Player.CurrPos.Y)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(Maps[x, y]);
                        Console.ResetColor();
                        continue;
                    }

                    if (Maps[x, y] == "☆")
                    {
                        Maps[x, y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                    }

                    if (!VisitedMaps[x, y])
                    {   // TODO: 방문하지 않으면 안개 생성
                        Console.Write(Managers.UI.TilePatterns[(int)TileTypes.Fog]);
                    }
                    else

                    // 현재 플레이어가 방 안에 있는지 확인
                    if (IsPositionInsideAnyRoom(Managers.Game.Player.CurrPos))
                    {
                        Room currentRoom = GetRoomAtPosition(Managers.Game.Player.CurrPos);

                        if (currentRoom != null && currentRoom.ContainsPosition(new Position(x, y)))
                        {
                            // 방 안에 있는 Character에게 색상 부여
                            if (currentRoom.ContainsPosition(Managers.Game.Peddler.CurrPos) &&
                                x == Managers.Game.Peddler.CurrPos.X && y == Managers.Game.Peddler.CurrPos.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                            }
                            else if (IsTileType(x, y, MonsterTile.Slime) ||
                                     IsTileType(x, y, MonsterTile.PocketMouse) ||
                                     IsTileType(x, y, MonsterTile.Skeleton))
                            {
                                // Character에 따라 다른 색상 적용
                                Console.ForegroundColor = ConsoleColor.Red; // 예시로 빨간색을 사용
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.Write(Maps[x, y]);
                        }
                        else
                        {
                            if (IsTileType(x, y, MonsterTile.Slime) ||
                                     IsTileType(x, y, MonsterTile.PocketMouse) ||
                                     IsTileType(x, y, MonsterTile.Skeleton))
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            }
                            Console.Write(Maps[x, y]);
                        }
                        Console.ResetColor();
                    }
                    else
                    {
                        Corridor currentCorridor = GetCorridorAtPosition(Managers.Game.Player.CurrPos);

                        if(currentCorridor != null && currentCorridor.ContainsAround(new Position(x,y)))
                        {
                            if (IsTileType(x, y, MonsterTile.Slime) ||
                                     IsTileType(x, y, MonsterTile.PocketMouse) ||
                                     IsTileType(x, y, MonsterTile.Skeleton))
                            {
                                // Character에 따라 다른 색상 적용
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.Write(Maps[x, y]);
                        }
                        else
                        {
                            if (IsTileType(x, y, MonsterTile.Slime) ||
                                     IsTileType(x, y, MonsterTile.PocketMouse) ||
                                     IsTileType(x, y, MonsterTile.Skeleton))
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            }
                            Console.Write(Maps[x, y]);
                        }
                        Console.ResetColor();
                    }
                }
            }
        }

        private Room GetRoomAtPosition(Position pos)
        {
            return rooms.FirstOrDefault(r => pos.X >= r.Position.X && pos.X < r.Position.X + r.Width &&
                                             pos.Y >= r.Position.Y && pos.Y < r.Position.Y + r.Height);
        }

        private Room GetRoomAtPositionCor(Position pos)
        {
            return rooms.FirstOrDefault(r => pos.X >= r.Position.X + 1 && pos.X < r.Position.X + r.Width - 1 &&
                                             pos.Y >= r.Position.Y + 1 && pos.Y < r.Position.Y + r.Height - 1 );
        }

        private Corridor GetCorridorAtPosition(Position pos)
        {
            foreach (Corridor corridor in corridors)
            {
                if (corridor.Positions.Contains(pos, new CorridorPosComparer()))
                {
                    return corridor;
                }
            }
            return null;
        }

        public override void Update()
        {
            char moveInput = Console.ReadKey(true).KeyChar;
            int dir = GetDirect(moveInput);

            if (dir == -1) return;

            Managers.Game.Player.Direct = (Direct)dir;
            Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y] = Managers.UI.PlayerPatterns[dir];

            if (IsMove(Managers.Game.Player))
            {
                // 배고픔 수치 다운
                Managers.Game.Player.OnAdjustHunger();

                // 이미지 처리
                Util.Swap(ref Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y],
                  ref Maps[Managers.Game.Player.PrevPos.X, Managers.Game.Player.PrevPos.Y]);

                // 몬스터 움직임
                for(int i = 0; i < Managers.Game.Monsters.Length; i++)
                {
                    if (!Managers.Game.Monsters[i].IsPlayerInSight())
                    {   // 시야에 없으면 자유 이동
                        Managers.Game.Monsters[i].Direct = (Direct)random.Next(0, 4);

                        if (IsMove(Managers.Game.Monsters[i]))
                        {
                            Util.Swap(ref Maps[Managers.Game.Monsters[i].CurrPos.X, Managers.Game.Monsters[i].CurrPos.Y],
                                ref Maps[Managers.Game.Monsters[i].PrevPos.X, Managers.Game.Monsters[i].PrevPos.Y]);
                        }
                    }          
                }
            }

            // TODO: 방문 체크 (방)
            Room currentRoom = GetRoomAtPosition(Managers.Game.Player.CurrPos);
            if (currentRoom != null && !currentRoom.isVisited)
            {
                currentRoom.isVisited = true;

                for (int y = currentRoom.Position.Y; y < currentRoom.Position.Y + currentRoom.Height; y++)
                {
                    for (int x = currentRoom.Position.X; x < currentRoom.Position.X + currentRoom.Width; x++)
                    {
                        VisitedMaps[x, y] = true;
                    }
                }
            }

            // TODO: 방문 체크 (복도)
            Corridor currentCorridor = GetCorridorAtPosition(Managers.Game.Player.CurrPos);
            if (currentCorridor != null && !currentCorridor.isVisited)
            {
                currentCorridor.isVisited = true;

                foreach (Position corridorPos in currentCorridor.Positions)
                {
                    VisitedMaps[corridorPos.X, corridorPos.Y] = true;
                }

                foreach (Position corridorWallPos in currentCorridor.AroundPositions)
                {
                    VisitedMaps[corridorWallPos.X, corridorWallPos.Y] = true;
                }
            }
            PrintMap();
        }

        private bool IsMove(Character target)
        {
            int[] dirX = { 0, 0, 1, -1 };        // X 방향 좌표
            int[] dirY = { -1, 1, 0, 0 };        // Y 방향 좌표

            target.PrevPos.X = target.CurrPos.X;
            target.PrevPos.Y = target.CurrPos.Y;

            if (IsTileType(target.CurrPos.X + dirX[(int)target.Direct], target.CurrPos.Y + dirY[(int)target.Direct], MonsterTile.Slime) ||
                IsTileType(target.CurrPos.X + dirX[(int)target.Direct], target.CurrPos.Y + dirY[(int)target.Direct], MonsterTile.PocketMouse) ||
                IsTileType(target.CurrPos.X + dirX[(int)target.Direct], target.CurrPos.Y + dirY[(int)target.Direct], MonsterTile.Skeleton))
            {
                return false;
            }

            if (IsTileType(target.CurrPos.X + dirX[(int)target.Direct], target.CurrPos.Y + dirY[(int)target.Direct], NPCTile.Paddler))
            {   
                // TODO: 거래 진행
                return false;
            }

            if (!IsTileType(target.CurrPos.X + dirX[(int)target.Direct], target.CurrPos.Y + dirY[(int)target.Direct], TileTypes.Wall))
            {   // 벽이 아니면 이동
                target.CurrPos.X += dirX[(int)target.Direct];
                target.CurrPos.Y += dirY[(int)target.Direct];
                return true;
            }
            return false;
        }

        private int GetDirect(char moveInput)
        {
            switch (moveInput)
            {
                case 'W': case 'w': return 0;
                case 'S': case 's': return 1;
                case 'D': case 'd': return 2;
                case 'A': case 'a': return 3;
            }
            return -1;
        }

        private bool IsTileType(int x, int y, TileTypes type)
        {
            if (Maps[x, y] == Managers.UI.TilePatterns[(int)type]) return true;
            return false;
        }

        private bool IsTileType(int x, int y, NPCTile type)
        {
            if (Maps[x, y] == Managers.UI.NPCPatterns[(int)type]) return true;
            return false;
        }

        private bool IsTileType(int x, int y, MonsterTile type)
        {
            if (Maps[x, y] == Managers.UI.MonsterPatterns[(int)type]) return true;
            return false;
        }
    }
}
