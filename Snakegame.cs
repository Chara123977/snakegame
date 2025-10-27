// SnakeGame.cs
using System;
using System.Collections.Generic;
using System.Threading;

namespace snakegame
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class SnakeGame
    {
        private List<Position> snake;
        private Position food;
        private int score = 0;
        private ConsoleKey lastKey = ConsoleKey.RightArrow;
        private const int Width = 40;
        private const int Height = 20;
        public int Score => score;

        public SnakeGame()
        {
            snake = new List<Position> { new Position { X = 10, Y = 10 } };
            GenerateFood();
        }

        public void Run()
        {
            Console.CursorVisible = false;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (IsValidDirection(key)) lastKey = key;
                }

                MoveSnake();
                if (CheckCollision()) break;

                Draw();
                Thread.Sleep(150); // 控制速度
            }

            Console.Clear();
            Console.WriteLine($"游戏结束！最终得分: {score}");

            Console.Write("请输入你的名字（直接回车则匿名）: ");
            string playerName = Console.ReadLine()?.Trim() ?? "Anonymous";

            // 保存到数据库
            var recordService = new GameRecordService();
            recordService.SaveScoreAsync(score, playerName).Wait();

            Console.ReadKey();
        }

        private void MoveSnake()
        {
            Position head = new Position { X = snake[0].X, Y = snake[0].Y };

            switch (lastKey)
            {
                case ConsoleKey.UpArrow: head.Y--; break;
                case ConsoleKey.DownArrow: head.Y++; break;
                case ConsoleKey.LeftArrow: head.X--; break;
                case ConsoleKey.RightArrow: head.X++; break;
            }

            snake.Insert(0, head);

            if (head.X == food.X && head.Y == food.Y)
            {
                score += 10;
                GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private bool CheckCollision()
        {
            Position head = snake[0];
            // 撞墙
            if (head.X < 0 || head.X >= Width || head.Y < 0 || head.Y >= Height)
                return true;
            // 撞自己
            for (int i = 1; i < snake.Count; i++)
            {
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                    return true;
            }
            return false;
        }

        private void GenerateFood()
        {
            Random rand = new Random();
            do
            {
                food = new Position
                {
                    X = rand.Next(0, Width),
                    Y = rand.Next(0, Height)
                };
            } while (snake.Exists(s => s.X == food.X && s.Y == food.Y));
        }

        private void Draw()
        {
            Console.Clear();
            // 绘制边框
            for (int x = 0; x < Width + 2; x++) Console.Write("#");
            Console.WriteLine();
            for (int y = 0; y < Height; y++)
            {
                Console.Write("#");
                for (int x = 0; x < Width; x++)
                {
                    if (snake.Exists(s => s.X == x && s.Y == y))
                        Console.Write("O");
                    else if (food.X == x && food.Y == y)
                        Console.Write("*");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("#");
            }
            for (int x = 0; x < Width + 2; x++) Console.Write("#");
            Console.WriteLine($"\n得分: {score}");
        }

        private bool IsValidDirection(ConsoleKey key)
        {
            return (key == ConsoleKey.UpArrow && lastKey != ConsoleKey.DownArrow) ||
                   (key == ConsoleKey.DownArrow && lastKey != ConsoleKey.UpArrow) ||
                   (key == ConsoleKey.LeftArrow && lastKey != ConsoleKey.RightArrow) ||
                   (key == ConsoleKey.RightArrow && lastKey != ConsoleKey.LeftArrow);
        }
    }
}