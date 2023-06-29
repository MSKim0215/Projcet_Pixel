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
        public static List<Node> FindPath(Position start, Position end)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            Node startNode = new Node(start, null);
            startNode.GCost = 0;
            startNode.HCost = CalculateHeuristic(start.X, start.Y, end.X, end.Y);
            startNode.TotalCost = startNode.GCost + startNode.HCost;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // Get the node with the lowest total cost from the open list
                Node currentNode = openList.OrderBy(n => n.TotalCost).First();

                // Check if the destination node is reached
                if (currentNode.Position.X == end.X && currentNode.Position.Y == end.Y)
                {
                    // Reconstruct the path
                    Node pathNode = currentNode;
                    while (pathNode != null)
                    {
                        //if (!IsPlayerTile(pathNode.Position) && !IsMonsterTile(pathNode.Position) && !IsNPCTile(pathNode.Position))
                        //{
                        //    Managers.Game.MapManager.Maps[pathNode.Position.X, pathNode.Position.Y] = Managers.UI.PathPattern;
                        //}
                        pathNode = pathNode.Parent;
                    }
                    break;
                }

                // Move the current node from the open list to the closed list
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Generate adjacent nodes
                Node[] adjacentNodes = new Node[]
                {
                        new Node(new Position(currentNode.Position.X - 1, currentNode.Position.Y), currentNode),
                        new Node(new Position(currentNode.Position.X + 1, currentNode.Position.Y), currentNode),
                        new Node(new Position(currentNode.Position.X, currentNode.Position.Y - 1), currentNode),
                        new Node(new Position(currentNode.Position.X, currentNode.Position.Y + 1), currentNode)
                };

                foreach (var adjacentNode in adjacentNodes)
                {
                    // Skip if the adjacent node is not a valid location or is already in the closed list
                    if (adjacentNode.Position.X < 0 || adjacentNode.Position.X >= MapManager.MAP_WIDTH ||
                        adjacentNode.Position.Y < 0 || adjacentNode.Position.Y >= MapManager.MAP_HEIGHT ||
                        closedList.Contains(adjacentNode) ||
                        Managers.Game.MapManager.Maps[adjacentNode.Position.X, adjacentNode.Position.Y] == Managers.UI.TilePatterns[(int)TileTypes.Wall])
                    {
                        continue;
                    }

                    // Calculate the cost of reaching the adjacent node
                    adjacentNode.GCost = currentNode.GCost + 1; // Assuming a cost of 1 to move to an adjacent node
                    adjacentNode.HCost = CalculateHeuristic(adjacentNode.Position.X, adjacentNode.Position.Y, end.X, end.Y);
                    adjacentNode.TotalCost = adjacentNode.GCost + adjacentNode.HCost;

                    // Check if the adjacent node is already in the open list
                    var existingNode = openList.Find(n => n.Position.X == adjacentNode.Position.X && n.Position.Y == adjacentNode.Position.Y);
                    if (existingNode != null)
                    {
                        // If the new path to the adjacent node is shorter, update its parent and costs
                        if (adjacentNode.GCost < existingNode.GCost)
                        {
                            existingNode.Parent = currentNode;
                            existingNode.GCost = adjacentNode.GCost;
                            existingNode.TotalCost = adjacentNode.TotalCost;
                        }
                    }
                    else
                    {
                        // Add the adjacent node to the open list
                        openList.Add(adjacentNode);
                    }
                }
            }
            return closedList;
        }

        private static int CalculateHeuristic(int startX, int startY, int targetX, int targetY)
        {
            // Simple Manhattan distance heuristic
            return Math.Abs(startX - targetX) + Math.Abs(startY - targetY);
        }

        private static bool IsPlayerTile(Position position)
        {
            if (position.X == Managers.Game.Player.CurrPos.X && position.Y == Managers.Game.Player.CurrPos.Y) return true;
            return false;
        }

        private static bool IsMonsterTile(Position position)
        {
            if(Managers.Game.MapManager.Maps[position.X, position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Slime] ||
                Managers.Game.MapManager.Maps[position.X, position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.PocketMouse] ||
                Managers.Game.MapManager.Maps[position.X, position.Y] == Managers.UI.MonsterPatterns[(int)MonsterTile.Skeleton])
            {
                return true;
            }
            return false;
        }

        private static bool IsNPCTile(Position position)
        {
            if (Managers.Game.MapManager.Maps[position.X, position.Y] == Managers.UI.NPCPatterns[(int)NPCTile.Paddler]) return true;
            return false;
        }
    }
}
