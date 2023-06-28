using Project_Pixel.Contents;
using Project_Pixel.Manager.Contents;
using Project_Pixel.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private void DrawCorridor(Room room)
        {
            Position point = room.CenterPosition;
            Position targetPoint = GetClosestRoom(room).CenterPosition;

            List<Position> positions = new List<Position>();

            for (int y = Math.Min(point.Y, targetPoint.Y); y <= Math.Max(point.Y, targetPoint.Y); y++)
            {
                Position corridorPos = new Position(point.X, y);
                Room currentRoom = GetRoomAtPositionCor(corridorPos);

                if (currentRoom == null)
                {
                    Maps[point.X, y] = "☆";
                    positions.Add(corridorPos);
                }
            }

            for (int x = Math.Min(point.X, targetPoint.X); x <= Math.Max(point.X, targetPoint.X); x++)
            {
                Position corridorPos = new Position(x, targetPoint.Y);
                Room currentRoom = GetRoomAtPositionCor(corridorPos);

                if (currentRoom == null)
                {
                    Maps[x, targetPoint.Y] = "☆";
                    positions.Add(corridorPos);
                }
            }

            List<Position> arounds = new List<Position>();
            foreach (Position corridorPos in positions)
            {
                for (int y = corridorPos.Y - 1; y <= corridorPos.Y + 1; y++)
                {
                    for (int x = corridorPos.X - 1; x <= corridorPos.X + 1; x++)
                    {
                        Position wallPos = new Position(x, y);
                        if (!positions.Contains(wallPos, new CorridorPosComparer()))
                        {
                            arounds.Add(wallPos);
                        }
                    }
                }
            }

            for (int i = 0; i < arounds.Count; i++)
            {
                positions.Add(arounds[i]);
            }

            Corridor corridor = new Corridor(positions, arounds);
            corridors.Add(corridor);
        }

        private void DrawCorridor(Room room1, Room room2)
        {
            room1.RefreshSubPosition();
            room2.RefreshSubPosition();

            Position point1 = room1.SubPosition;
            Position point2 = room2.SubPosition;

            List<Position> positions = new List<Position>();

            for (int y = Math.Min(point1.Y, point2.Y); y <= Math.Max(point1.Y, point2.Y); y++)
            {
                Position corridorPos = new Position(point1.X, y);
                Room currentRoom = GetRoomAtPositionCor(corridorPos);

                if (currentRoom == null)
                {
                    Maps[point1.X, y] = "☆";
                    positions.Add(corridorPos);
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
            }

            List<Position> arounds = new List<Position>();
            foreach (Position corridorPos in positions)
            {
                for (int y = corridorPos.Y - 1; y <= corridorPos.Y + 1; y++)
                {
                    for (int x = corridorPos.X - 1; x <= corridorPos.X + 1; x++)
                    {
                        Position wallPos = new Position(x, y);
                        if (!positions.Contains(wallPos, new CorridorPosComparer()))
                        {
                            arounds.Add(wallPos);
                        }
                    }
                }
            }

            for (int i = 0; i < arounds.Count; i++)
            {
                positions.Add(arounds[i]);
            }

            Corridor corridor = new Corridor(positions, arounds);
            corridors.Add(corridor);
        }


        private void DrawWallsAroundCorridors()
        {
            foreach (Corridor corridor in corridors)
            {
                foreach(Position aroundPos in corridor.AroundPositions)
                {
                    if (!IsPositionInsideAnyRoom(aroundPos) &&
                        corridor.Positions.Contains(aroundPos, new CorridorPosComparer()) &&
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
                if (position.X > room.Position.X && position.X < room.Position.X + room.Width - 1 &&
                    position.Y > room.Position.Y && position.Y < room.Position.Y + room.Height - 1)
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

                    //if (!VisitedMaps[x, y])
                    //{   // TODO: 방문하지 않으면 안개 생성
                    //    Console.Write(Managers.UI.TilePatterns[(int)TileTypes.Fog]);
                    //}
                    //else
                    if (IsPositionInsideAnyRoom(Managers.Game.Player.CurrPos))
                    {   // TODO: 현재 플레이어가 방 안에 있다면
                        Room currentRoom = GetRoomAtPosition(Managers.Game.Player.CurrPos);

                        if(currentRoom != null)
                        {
                            if(currentRoom.ContainsPosition(Managers.Game.Peddler.CurrPos) &&
                                x == Managers.Game.Peddler.CurrPos.X && y == Managers.Game.Peddler.CurrPos.Y)
                            {   // TODO: 방 안에 떠돌이 상인이 있다면 색상 변경
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write(Maps[x, y]);
                                Console.ResetColor();
                            }
                            else if(currentRoom.ContainsPosition(new Position(x, y)))
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(Maps[x, y]);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(Maps[x, y]);
                            }
                        }
                        Console.ResetColor();
                    }
                    else
                    {
                        Corridor currentCorridor = GetCorridorAtPosition(Managers.Game.Player.CurrPos);
                        if (currentCorridor != null && currentCorridor.ContainsPosition(new Position(x, y)))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(Maps[x, y]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
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

            if (IsMove(Managers.Game.Player.CurrPos, Managers.Game.Player.PrevPos, dir))
            {
                Util.Swap(ref Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y],
                  ref Maps[Managers.Game.Player.PrevPos.X, Managers.Game.Player.PrevPos.Y]);
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
                    //VisitedMaps[corridorPos.X, corridorPos.Y - 1] = true;
                    //VisitedMaps[corridorPos.X, corridorPos.Y + 1] = true;
                    //VisitedMaps[corridorPos.X + 1, corridorPos.Y] = true;
                    //VisitedMaps[corridorPos.X - 1, corridorPos.Y] = true;
                    //VisitedMaps[corridorPos.X - 1, corridorPos.Y - 1] = true;
                    //VisitedMaps[corridorPos.X + 1, corridorPos.Y - 1] = true;
                    //VisitedMaps[corridorPos.X - 1, corridorPos.Y + 1] = true;
                    //VisitedMaps[corridorPos.X + 1, corridorPos.Y + 1] = true;
                }
            }
            PrintMap();
        }

        private bool IsMove(Position curPos, Position prevPos, int dir)
        {
            if (dir == -1) return false;

            int[] dirX = { 0, 0, 1, -1 };        // X 방향 좌표
            int[] dirY = { -1, 1, 0, 0 };        // Y 방향 좌표

            prevPos.X = curPos.X;
            prevPos.Y = curPos.Y;

            // TODO: 상인 판정
            if(IsTileType(curPos.X + dirX[dir], curPos.Y + dirY[dir], NPCTile.Paddler))
            {
                // TODO: 거래 진행
                return false;
            }

            // TODO: 벽 판정
            if (!IsTileType(curPos.X + dirX[dir], curPos.Y + dirY[dir], TileTypes.Wall))
            {
                curPos.X += dirX[dir];
                curPos.Y += dirY[dir];
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

        private Room GetClosestRoom(Room myRoom)
        {
            Room closestRoom = null;
            int closestDistance = int.MaxValue;

            foreach (Room room in rooms)
            {
                if (myRoom == room) continue;

                int distance = Math.Abs(room.CenterPosition.X - myRoom.Position.X) + Math.Abs(room.CenterPosition.Y - myRoom.Position.Y);

                if (distance < closestDistance && room.partnerRoom == null)
                {
                    closestRoom = room;
                    closestDistance = distance;
                }
            }

            if(closestRoom != null)
            {
                closestRoom.partnerRoom = myRoom;
            }

            return closestRoom;
        }
    }
}
