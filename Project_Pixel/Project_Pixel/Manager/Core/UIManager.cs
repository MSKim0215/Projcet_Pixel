using Project_Pixel.Contents;
using Project_Pixel.Contents.Debuff_System;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Manager
{
    public class UIManager
    {
        private List<string> systemLogList = new List<string>();        // 시스템 로그

        public string[] TilePatterns { private set; get; } = { "■", "  ", "□" };
        public string[] PlayerPatterns { private set; get; } = { "▲", "▼", "▶", "◀" };
        public string[] NPCPatterns { private set; get; } = { "●" };
        public string[] MonsterPatterns { private set; get; } = { "슬", "쥐", "스" };
        public string PathPattern { private set; get; } = "＠";
        // 슬라임, 쥐, 스켈레톤

        public void Init()
        {
            Console.Title = "짭셀 던전";
            Console.CursorVisible = false;
            Console.SetWindowSize(185, 50);
        }

        public void Print_GameScene()
        {
            int startX = 1;
            int startY = 1;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌── {"< 게임 화면 >".PadRight(118, '─')}┐");

            for (int i = 0; i < 47; i++)
            {
                PaddingSpace(startX, startY + (1 + i), 125);
            }

            Console.SetCursorPosition(startX, startY + 48);
            Console.Write($"└{"".PadRight(125, '─')}┘");
        }

        public void Print_Status(Player player)
        {
            int startX = 130;
            int startY = 1;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌{"".PadRight(3, '─')} < 플레이어 정보 >{"".PadRight(3, '─')}┐");

            for (int i = 0; i < 14; i += 2)
            {
                PaddingSpace(startX, startY + (i + 1), 24);
            }

            Console.SetCursorPosition(startX, startY + 2);
            Console.Write($"│{"배고픔".PadLeft(6, ' ')}: {$"{player.GetHungry()} / {player.GetMaxHungry()}".PadRight(13, ' ')}│");

            Console.SetCursorPosition(startX, startY + 4);
            Console.Write($"│{"체력".PadLeft(5, ' ')}: {$"{player.GetNowHp()} / {player.GetMaxHp()}".PadRight(15, ' ')}│");

            Console.SetCursorPosition(startX, startY + 6);
            Console.Write($"│{"공격력".PadLeft(6, ' ')}: {$"{player.GetPower()}".PadRight(13, ' ')}│");

            Console.SetCursorPosition(startX, startY + 8);
            Console.Write($"│{"방어력".PadLeft(6, ' ')}: {$"{player.GetDefense()}".PadRight(13, ' ')}│");

            Console.SetCursorPosition(startX, startY + 10);
            Console.Write($"│{"치명타 확률".PadLeft(9, ' ')}: {$"{player.GetCriChance()}%".PadRight(8, ' ')}│");

            Console.SetCursorPosition(startX, startY + 12);
            Console.Write($"│{"치명타 데미지".PadLeft(10, ' ')}: {$"{player.GetCriDamageValue() * 100}%".PadRight(6, ' ')}│");

            Console.SetCursorPosition(startX, startY + 14);
            Console.Write($"└{"".PadRight(24, '─')}┘");
        }

        public void Print_State(Player player)
        {
            int startX = 156;
            int startY = 1;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌{"".PadRight(3, '─')} < 플레이어 상태 >{"".PadRight(3, '─')}┐");

            for (int i = 0; i < 14; i++)
            {
                PaddingSpace(startX, startY + (i + 1), 24);
            }

            if (player.MyDebuffs != null)
            {
                List<Debuff> debuffs = player.MyDebuffs.ToList();
                if (debuffs.Count > 0)
                {
                    for (int i = 0; i < debuffs.Count; i++)
                    {
                        Console.SetCursorPosition(startX + 2, startY + (i * 2) + 2);
                        Console.Write($"{debuffs[i].Name.PadLeft(11, ' ')}");
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Console.SetCursorPosition(startX + 2, startY + (i * 2) + 2);
                        Console.Write($"{"".PadLeft(23, ' ')}");
                    }
                }
            }

            Console.SetCursorPosition(startX, startY + 14);
            Console.Write($"└{"".PadRight(24, '─')}┘");
        }

        public void Print_Inventory(Player player)
        {
            int cursorX = 130;
            int cursorY = 19;

            Console.SetCursorPosition(cursorX, cursorY - 3);
            Console.WriteLine($"┌{"".PadRight(20,'─')} < 가방 >{"".PadRight(21, '─')}┐");
            Console.SetCursorPosition(cursorX, cursorY - 2);
            Console.WriteLine($"│{"".PadRight(7,' ')}아이템 이름{"".PadRight(6, ' ')}│{"".PadRight(7, ' ')}아이템 개수{"".PadRight(7, ' ')}│");
            Console.SetCursorPosition(cursorX, cursorY - 1);
            Console.WriteLine($"├{"".PadRight(24, '─')}┼{"".PadRight(25, '─')}┤");

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.SetCursorPosition(cursorX, cursorY + (i * 2) + j);
                    Console.Write($"│{"".PadRight(24, ' ')}│{"".PadRight(25, ' ')}│");
                }
            }

            //for (int i = 0; i < player.Inven.MyItems.Count; i++)
            //{
            //    string name = player.Inven.MyItems[i].Info.Name;
            //    string count = player.Inven.MyItems[i].Count.ToString();

            //    Console.SetCursorPosition(cursorX + 5, cursorY + (i * 2));
            //    Console.Write($"{name.PadRight(9, ' ')}");
            //    Console.SetCursorPosition(cursorX + 28, cursorY + (i * 2));
            //    Console.Write($"{count.PadRight(10, ' ')}");
            //}

            Console.SetCursorPosition(cursorX, cursorY + (5 * 2));
            Console.Write($"└{"".PadRight(24, '─')}┴{"".PadRight(25, '─')}┘");

            Console.SetCursorPosition(cursorX + 31, cursorY + (5 * 2) + 1);
            Console.Write($"보유 금액: {player.Inven.Gold} 골드    ");
        }

        public void Print_Log()
        {
            int startX = 130;
            int startY = 30;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌{"".PadRight(1, '─')} < 행동 기록 >{"".PadRight(35, '─')}┐");

            for (int i = 0; i < 9; i++)
            {
                PaddingSpace(startX, startY + (1 + i), 50);
            }

            Console.SetCursorPosition(startX, startY + 10);
            Console.Write($"└{"".PadRight(50, '─')}┘");
        }

        public void Print_Guide()
        {
            int startX = 130;
            int startY = 41;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌{"".PadRight(1, '─')} < 도움말 >{"".PadRight(38, '─')}┐");

            for (int i = 0; i < 5; i++)
            {
                PaddingSpace(startX, startY + (1 + i), 50);
            }

            Console.SetCursorPosition(startX, startY + 6);
            Console.Write($"└{"".PadRight(50, '─')}┘");
        }

        public void Print_BaseGuide()
        {
            int startX = 133;
            int startY = 43;

            Console.SetCursorPosition(startX + 7, startY);
            Console.Write("W: ↑");
            Console.SetCursorPosition(startX + 23, startY - 1);
            Console.Write($"1: 물약 먹기 (0개)");
            Console.SetCursorPosition(startX + 23, startY);
            Console.Write($"2: 음식 먹기 (0개)");
            Console.SetCursorPosition(startX + 23, startY + 1);
            Console.Write($"3: 탈출 스크롤 사용 (0개)");
            Console.SetCursorPosition(startX, startY + 2);
            Console.Write("A: ←  S: ↓   D: →");
            Console.SetCursorPosition(startX + 23, startY + 3);
            Console.Write("Space: 공격");
        }

        public void Print_Enter()
        {
            int startX = 130;
            int startY = 47;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"├{"".PadRight(50, '─')}┤");

            for (int i = 0; i < 1; i++)
            {
                PaddingSpace(startX, startY + (i + 1 ), 50);
            }

            Console.SetCursorPosition(startX, startY + 2);
            Console.Write($"└{"".PadRight(50, '─')}┘");

            Console.SetCursorPosition(startX + 3, startY + 1);
            Console.Write($"입력 >> ");
        }

        public void Print_GameLog(string getLog)
        {
            int startX = 134;
            int startY = 32;

            if (systemLogList.Count >= 4)
            {
                systemLogList.Remove(systemLogList[0]);
            }

            string log = $"{getLog.PadRight(22, ' ')}";
            systemLogList.Add(log);

            for (int i = 0; i < systemLogList.Count; i++)
            {
                Console.SetCursorPosition(startX, startY + (i * 2));
                Console.WriteLine(systemLogList[i]);
            }
        }

        private void PaddingSpace(int x, int y, int spaceCount)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"│{" ".PadRight(spaceCount, ' ')}│");
        }
    }
}
