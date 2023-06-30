using Project_Pixel.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.Manager;

namespace Project_Pixel.Manager.Contents
{
    public class PathManager
    {
        public static List<Node> FindPath(Position start, Position end, int maxPathLength)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            bool pathFound = false; // 도달 가능 여부를 추적하는 변수

            Node startNode = new Node(start, null);
            startNode.GCost = 0;
            startNode.HCost = CalculateHeuristic(start.X, start.Y, end.X, end.Y);
            startNode.TotalCost = startNode.GCost + startNode.HCost;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(n => n.TotalCost).First();

                if (currentNode.Position.X == end.X && currentNode.Position.Y == end.Y)
                {
                    pathFound = true; // 도달 가능한 경로를 찾았음을 표시
                    break;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                Node[] adjacentNodes = new Node[]
                {
            new Node(new Position(currentNode.Position.X - 1, currentNode.Position.Y - 1), currentNode),     // Up-Left
            new Node(new Position(currentNode.Position.X - 1, currentNode.Position.Y), currentNode),         // Up
            new Node(new Position(currentNode.Position.X - 1, currentNode.Position.Y + 1), currentNode),     // Up-Right
            new Node(new Position(currentNode.Position.X, currentNode.Position.Y - 1), currentNode),         // Left
            new Node(new Position(currentNode.Position.X, currentNode.Position.Y + 1), currentNode),         // Right
            new Node(new Position(currentNode.Position.X + 1, currentNode.Position.Y - 1), currentNode),     // Down-Left
            new Node(new Position(currentNode.Position.X + 1, currentNode.Position.Y), currentNode),         // Down
            new Node(new Position(currentNode.Position.X + 1, currentNode.Position.Y + 1), currentNode)      // Down-Right
                };

                foreach (var adjacentNode in adjacentNodes)
                {
                    if (adjacentNode.Position.X < 0 || adjacentNode.Position.X >= MapManager.MAP_WIDTH ||
                        adjacentNode.Position.Y < 0 || adjacentNode.Position.Y >= MapManager.MAP_HEIGHT ||
                        closedList.Contains(adjacentNode) ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.TilePatterns[(int)TileTypes.Wall] ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Slime] ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Skeleton] ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.PocketMouse] ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.NPCPatterns[(int)NPCTile.Paddler])
                    {
                        continue;
                    }

                    adjacentNode.GCost = currentNode.GCost + 1; // Assuming a cost of 1 to move to an adjacent node
                    adjacentNode.HCost = CalculateHeuristic(adjacentNode.Position.X, adjacentNode.Position.Y, end.X, end.Y);
                    adjacentNode.TotalCost = adjacentNode.GCost + adjacentNode.HCost;

                    var existingNode = openList.Find(n => n.Position.X == adjacentNode.Position.X && n.Position.Y == adjacentNode.Position.Y);
                    if (existingNode != null)
                    {
                        if (adjacentNode.GCost < existingNode.GCost)
                        {
                            existingNode.Parent = currentNode;
                            existingNode.GCost = adjacentNode.GCost;
                            existingNode.TotalCost = adjacentNode.TotalCost;
                        }
                    }
                    else
                    {
                        openList.Add(adjacentNode);
                    }
                }

                // 일정 값 이상의 경로 길이 체크
                if (closedList.Count >= maxPathLength)
                {
                    break;
                }
            }

            if (!pathFound)
            {
                // 도달할 수 없는 경우 처리
                return null;
            }

            // 경로 노드들을 역순으로 가져옴
            List<Node> path = new List<Node>();
            Node pathNode = closedList.Last();
            while (pathNode != null)
            {
                path.Add(pathNode);
                pathNode = pathNode.Parent;
            }
            path.Reverse();

            return path;
        }


        private static int CalculateHeuristic(int startX, int startY, int targetX, int targetY)
        {
            // Simple Manhattan distance heuristic
            return Math.Abs(startX - targetX) + Math.Abs(startY - targetY);
        }
    }
}
