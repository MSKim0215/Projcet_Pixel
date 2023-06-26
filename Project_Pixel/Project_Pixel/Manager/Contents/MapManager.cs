using Project_Pixel.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Manager.Contents
{
    public class MapManager
    {
        private const int MAP_WIDTH = 60;
        private const int MAP_HEIGHT = 45;

        private Random random = new Random();
        private string[,] maps;

        private int splitCount = 0, splitCountMax = 300;

        private List<Room> rooms = new List<Room>();
        private List<Position> corridors = new List<Position>();

        public MapManager()
        {
            maps = new string[MAP_WIDTH, MAP_HEIGHT];

            Generate();
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
                        maps[x, y] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
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

                if (rooms.Count <= 5)
                {
                    continue;
                }

                PrintMap();

                Console.ReadKey();
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
                    maps[i, position.Y] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                    maps[i, position.Y + height - 1] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                }

                for (int i = position.Y; i < position.Y + height; i++)
                {
                    maps[position.X, i] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                    maps[position.X + width - 1, i] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                }

                maps[newRoom.CenterPosition.X, newRoom.CenterPosition.Y] = "☆";

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
                //int doorPosition = random.Next(Math.Min(position.X + 2, position.X + width - 2),
                //    Math.Max(position.X + 2, position.X + width - 2));

                //for(int i = position.X; i < position.X + width; i++)
                //{
                //    maps[i, split] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                //}

                //maps[doorPosition, split] = Managers.UI.TilePatterns[(int)TileTpyes.Door];

                SplitRegion(position, width, split - position.Y);
                SplitRegion(new Position(position.X, split + 1), width, position.Y + height - split - 1);
            }
            else
            {   // 수직 분할
                int split = random.Next(Math.Min(position.X + 3, position.X + width - 4),
                    Math.Max(position.X + 3, position.X + width - 4)); 
                //int doorPosition = random.Next(Math.Min(position.Y + 2, position.Y + height - 2),
                //    Math.Max(position.Y + 2, position.Y + height - 2));

                //for (int i = position.Y; i < position.Y + height; i++)
                //{
                //    maps[split, i] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
                //}

                //maps[split, doorPosition] = Managers.UI.TilePatterns[(int)TileTpyes.Door];

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
                        maps[i, room.Position.Y] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
                        maps[i, room.Position.Y + room.Height - 1] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
                    }

                    for (int i = room.Position.Y; i < room.Position.Y + room.Height; i++)
                    {
                        maps[room.Position.X, i] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
                        maps[room.Position.X + room.Width - 1, i] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
                    }

                    maps[room.CenterPosition.X, room.CenterPosition.Y] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];

                    removeRooms.Add(room);
                }
            }

            for(int i = 0; i < removeRooms.Count; i++)
            {
                rooms.Remove(removeRooms[i]);
            }
        }

        private void ConnectRoom()
        {
            foreach (Room room in rooms)
            {
                int minIndex = -1;
                double minDistance = double.MaxValue;

                for (int i = 0; i < rooms.Count; i++)
                {
                    if (room != rooms[i] &&
                        room.GetDistance(rooms[i].CenterPosition) < minDistance)
                    {
                        minIndex = i;
                        minDistance = room.GetDistance(rooms[i].CenterPosition);
                    }
                }

                if (minIndex != -1)
                {
                    DrawCorridor(room, rooms[minIndex]);

                    // 각 방에 연결된 방 추가
                    room.Neighbors.Add(rooms[minIndex]);
                    rooms[minIndex].Neighbors.Add(room);
                }
            }

            DrawWallsAroundCorridors();
        }


        private void DrawCorridor(Room room1, Room room2)
        {
            Position point1 = room1.CenterPosition;
            Position point2 = room2.CenterPosition;

            for (int y = Math.Min(point1.Y, point2.Y); y <= Math.Max(point1.Y, point2.Y); y++)
            {
                Position corridorPos = new Position(point1.X, y);
                maps[point1.X, y] = "☆";
                corridors.Add(corridorPos);
            }

            for (int x = Math.Min(point1.X, point2.X); x <= Math.Max(point1.X, point2.X); x++)
            {
                Position corridorPos = new Position(x, point2.Y);
                maps[x, point2.Y] = "☆";
                corridors.Add(corridorPos);
            }
        }

        private void DrawWallsAroundCorridors()
        {
            foreach (Position pos in corridors)
            {
                for (int y = pos.Y - 1; y <= pos.Y + 1; y++)
                {
                    for (int x = pos.X - 1; x <= pos.X + 1; x++)
                    {
                        Position wallPos = new Position(x, y);

                        if (x != pos.X && y != pos.Y)
                        {
                            if (!IsPositionInsideAnyRoom(wallPos) && !corridors.Contains(wallPos) && maps[x,y] == Managers.UI.TilePatterns[(int)TileTpyes.Empty])
                            {
                                maps[x, y] = Managers.UI.TilePatterns[(int)TileTpyes.Wall];
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

        public bool CheckAllRoomsAccessible()
        {
            // 모든 방을 '방문하지 않음' 상태로 초기화합니다.
            foreach (var room in rooms)
            {
                room.IsVisited = false;
            }

            Queue<Room> queue = new Queue<Room>();
            Room startRoom = rooms[0]; // 시작 방을 정합니다. 필요에 따라 변경 가능합니다.
            queue.Enqueue(startRoom);
            startRoom.IsVisited = true;

            while (queue.Count > 0)
            {
                Room currentRoom = queue.Dequeue();

                // 현재 방에서 갈 수 있는 다른 모든 방을 큐에 추가합니다.
                foreach (Room neighborRoom in currentRoom.Neighbors)
                {
                    if (!neighborRoom.IsVisited)
                    {
                        queue.Enqueue(neighborRoom);
                        neighborRoom.IsVisited = true;
                    }
                }
            }

            // 모든 방을 방문했는지 확인합니다.
            foreach (var room in rooms)
            {
                if (!room.IsVisited)
                {
                    return false; // 방문하지 않은 방이 있습니다.
                }
            }

            return true; // 모든 방을 방문했습니다.
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
                    if (maps[x, y] == "☆")
                    {
                        maps[x, y] = Managers.UI.TilePatterns[(int)TileTpyes.Empty];
                    }

                    Console.Write(maps[x, y]);
                }
            }
        }
    }
}
