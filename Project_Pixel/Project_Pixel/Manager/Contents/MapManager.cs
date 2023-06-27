using Project_Pixel.Contents;
using Project_Pixel.Manager.Contents;
using Project_Pixel.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Position point1 = room1.CenterPosition;
            Position point2 = room2.CenterPosition;

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

            Corridor corridor = new Corridor(positions);
            corridors.Add(corridor);
        }


        private void DrawWallsAroundCorridors()
        {
            foreach (Corridor corridor in corridors)
            {
                for(int i = 0; i < corridor.Positions.Count; i++)
                {
                    for(int y = corridor.Positions[i].Y - 1; y <= corridor.Positions[i].Y + 1; y++)
                    {
                        for(int x = corridor.Positions[i].X - 1; x <= corridor.Positions[i].X + 1; x++)
                        {
                            Position wallPos = new Position(x, y);

                            if(x != corridor.Positions[i].X && y != corridor.Positions[i].Y)
                            {
                                if(!IsPositionInsideAnyRoom(wallPos) && 
                                    !corridor.Positions.Contains(wallPos) &&
                                    Maps[x, y] == Managers.UI.TilePatterns[(int)TileTypes.Empty])
                                {
                                    Maps[x, y] = Managers.UI.TilePatterns[(int)TileTypes.Wall];
                                }
                            }
                        }
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
            int startY = 2;

            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Console.SetCursorPosition(startX, startY + y);

                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    if (Maps[x, y] == "☆")
                    {
                        Maps[x, y] = Managers.UI.TilePatterns[(int)TileTypes.Empty];
                    }

                    if (!VisitedMaps[x, y])
                    {
                        Console.Write(Managers.UI.TilePatterns[(int)TileTypes.Fog]);
                    }
                    else
                    {
                        Console.Write(Maps[x, y]);
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

            Managers.Game.Player.Direct = (Direct)dir;
            Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y] = Managers.UI.PlayerPatterns[dir];

            if (IsMove(Managers.Game.Player.CurrPos, Managers.Game.Player.PrevPos, dir))
            {
                Util.Swap(ref Maps[Managers.Game.Player.CurrPos.X, Managers.Game.Player.CurrPos.Y],
                  ref Maps[Managers.Game.Player.PrevPos.X, Managers.Game.Player.PrevPos.Y]);
            }

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

            Corridor currentCorridor = GetCorridorAtPosition(Managers.Game.Player.CurrPos);
            if (currentCorridor != null && !currentCorridor.isVisited)
            {
                currentCorridor.isVisited = true;

                foreach (Position corridorPos in currentCorridor.Positions)
                {
                    VisitedMaps[corridorPos.X, corridorPos.Y - 1] = true;
                    VisitedMaps[corridorPos.X, corridorPos.Y] = true;
                    VisitedMaps[corridorPos.X, corridorPos.Y + 1] = true;
                    VisitedMaps[corridorPos.X + 1, corridorPos.Y] = true;
                    VisitedMaps[corridorPos.X - 1, corridorPos.Y] = true;
                    VisitedMaps[corridorPos.X - 1, corridorPos.Y - 1] = true;
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
    }
}
