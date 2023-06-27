using Project_Pixel.Contents;
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
        public string[] TilePatterns { private set; get; } = { "■", "  ", "□" };
        public string[] PlayerPatterns { private set; get; } = { "△", "▽", "▷", "◁" };
        public string[] CharacterPatterns { private set; get; } = { "●" };

        public void Init()
        {
            Console.Title = "짭셀 던전";
            Console.SetWindowSize(185, 50);
            Console.CursorVisible = false;
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

        public void Print_PlayerUI(Player player)
        {
            int startX = 130;
            int startY = 1;

            Console.SetCursorPosition(startX, startY);
            Console.Write($"┌{"".PadRight(16, '─')} < 플레이어 정보 >{"".PadRight(16, '─')}┐");
            PaddingSpace(startX, startY + 1, 50);
            Console.SetCursorPosition(startX, startY + 2);
            Console.Write($"│ {"체력".PadLeft(5, ' ')}: {$"{player.GetNowHp()}  /  {player.GetMaxHp()}".PadRight(40, ' ')}│");
            PaddingSpace(startX, startY + 3, 50);
            Console.SetCursorPosition(startX, startY + 4);
            Console.Write($"│ {"공격력".PadLeft(6, ' ')}: {$"{player.GetPower()}".PadRight(38, ' ')}│");
            PaddingSpace(startX, startY + 5, 50);
            Console.SetCursorPosition(startX, startY + 6);
            Console.Write($"│ {"방어력".PadLeft(6, ' ')}: {$"{player.GetDefense()}".PadRight(38, ' ')}│");
            PaddingSpace(startX, startY + 7, 50);
            Console.SetCursorPosition(startX, startY + 8);
            Console.Write($"│ {"치명타 확률".PadLeft(9, ' ')}: {$"{player.GetCriChance()}%".PadRight(33, ' ')}│");
            PaddingSpace(startX, startY + 9, 50);
            Console.SetCursorPosition(startX, startY + 10);
            Console.Write($"│ {"치명타 데미지".PadLeft(10, ' ')}: {$"{player.GetCriDamageValue() * 100}%".PadRight(31, ' ')}│");
            PaddingSpace(startX, startY + 11, 50);
            Console.SetCursorPosition(startX, startY + 12);
            Console.Write($"└{"".PadRight(50, '─')}┘");
        }

        public void Print_PlayerInventory(Player player)
        {
            int cursorX = 130;
            int cursorY = 18;

            Console.SetCursorPosition(cursorX, cursorY - 3);
            Console.WriteLine($"┌{"".PadRight(20,'─')} < 가방 >{"".PadRight(20, '─')}┐");
            Console.SetCursorPosition(cursorX, cursorY - 2);
            Console.WriteLine($"│{"".PadRight(7,' ')}아이템 이름{"".PadRight(6, ' ')}│{"".PadRight(7, ' ')}아이템 개수{"".PadRight(6, ' ')}│");
            Console.SetCursorPosition(cursorX, cursorY - 1);
            Console.WriteLine($"├{"".PadRight(24, '─')}┼{"".PadRight(24, '─')}┤");

            for (int i = 0; i < player.Inven.MyItems.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.SetCursorPosition(cursorX, cursorY + (i * 2) + j);
                    Console.Write($"│{"".PadRight(24, ' ')}│{"".PadRight(24, ' ')}│");
                }
            }

            for (int i = 0; i < player.Inven.MyItems.Count; i++)
            {
                string name = player.Inven.MyItems[i].Info.Name;
                string count = player.Inven.MyItems[i].Count.ToString();

                Console.SetCursorPosition(cursorX + 5, cursorY + (i * 2));
                Console.Write($"{name.PadRight(9, ' ')}");
                Console.SetCursorPosition(cursorX + 28, cursorY + (i * 2));
                Console.Write($"{count.PadRight(10, ' ')}");
            }

            Console.SetCursorPosition(cursorX, cursorY + ((player.Inven.MyItems.Count) * 2));
            Console.Write($"└{"".PadRight(24, '─')}┴{"".PadRight(24, '─')}┘");

            Console.SetCursorPosition(cursorX + 31, cursorY + ((player.Inven.MyItems.Count) * 2) + 1);
            Console.Write($"보유 금액: {player.Inven.Gold} 골드    ");
        }

        private void PaddingSpace(int x, int y, int spaceCount)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"│{" ".PadRight(spaceCount, ' ')}│");
        }
    }
}
