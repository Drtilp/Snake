using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;
            //int screenwidth = 32;
            //int screenheight = 16;
            int screenwidth = 72;
            int screenheight = 36;

           
            Snake snake = new Snake(screenwidth, screenheight);
            snake.StartPlaying();
        }

    }

    class Snake
    {
        private List<int> snakeXPosition;
        private List<int> snakeYPosition;
        private int screenwidth;
        private int screenheight;
        private Pixel headOfSnake;
        private SnakeDirection snakeDirection;
        DateTime lastUpdate;
        public int score;
        public double snakeSpeed;

        public Snake(int width, int height)
        {
            screenwidth = width;
            screenheight = height;

            score = 5;

            snakeXPosition = new List<int>();
            snakeYPosition = new List<int>();

            headOfSnake = new Pixel();
            headOfSnake.xPosition = screenwidth / 2;
            headOfSnake.yPosition = screenheight / 2;
            headOfSnake.color = ConsoleColor.Red;
            lastUpdate = DateTime.Now;
            snakeDirection = SnakeDirection.RIGHT;
            snakeSpeed = 0.3;
        }

        public void StartPlaying()
        {
            Berry berry = new Berry(screenwidth, screenheight);
            int[] berryPosition = berry.GenerateBerry();
            DrawBorder();
            while (true)
            {

                if (CheckCollisionWithWall())
                {
                    break;
                }

                if (CheckBerryCollision(berryPosition))
                {
                    do
                    {
                        berryPosition = berry.GenerateBerry();
                    }
                    while (CheckContactWithTail(berryPosition));
                    
                    score++;  
                }

                DrawSnake();

                ProcessInput();
                Move();
            }

            Console.SetCursorPosition(screenwidth / 5, screenheight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenwidth / 5, screenheight / 2 + 1);

        }

        public void DrawBorder()
        {
            for (int i = 0; i < screenwidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
            }

            for (int i = 0; i < screenwidth; i++)
            {
                Console.SetCursorPosition(i, screenheight - 1);
                Console.Write("■");
            }

            for (int i = 0; i < screenheight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
            }

            for (int i = 0; i < screenheight; i++)
            {
                Console.SetCursorPosition(screenwidth - 1, i);
                Console.Write("■");
            }
        }


        public void DrawSnake()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < snakeXPosition.Count(); i++)
            {
                Console.SetCursorPosition(snakeXPosition[i], snakeYPosition[i]);
                Console.Write("■");

                if (snakeXPosition[i] == headOfSnake.xPosition && snakeYPosition[i] == headOfSnake.yPosition)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(screenwidth / 5, screenheight / 2);
                    Console.WriteLine("Game over, Score: " + snakeXPosition.Count());
                    Console.SetCursorPosition(screenwidth / 5, screenheight / 2 + 1);
                    Environment.Exit(0);
                }
            }

            Console.SetCursorPosition(headOfSnake.xPosition, headOfSnake.yPosition);
            Console.ForegroundColor = headOfSnake.color;
            Console.Write("■");
        }

        public bool CheckBerryCollision(int[] berryPosition)
        {
            return berryPosition[0] == headOfSnake.xPosition && headOfSnake.yPosition == berryPosition[1];
        }

        public bool CheckContactWithTail(int[] position)
        {
            for (int i = 0; i < snakeXPosition.Count(); i++)
            {
                if (snakeXPosition[i] == position[0] && snakeYPosition[i] == position[1])
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckCollisionWithWall()
        {
            return headOfSnake.xPosition == screenwidth - 1 || headOfSnake.xPosition == 0 || headOfSnake.yPosition == screenheight - 1 || headOfSnake.yPosition == 0;
        }

        public void ProcessInput()
        {


            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow when snakeDirection != SnakeDirection.DOWN:
                        snakeDirection = SnakeDirection.UP;
                        break;

                    case ConsoleKey.DownArrow when snakeDirection != SnakeDirection.UP:
                        snakeDirection = SnakeDirection.DOWN;
                        break;

                    case ConsoleKey.LeftArrow when snakeDirection != SnakeDirection.RIGHT:
                        snakeDirection = SnakeDirection.LEFT;
                        break;

                    case ConsoleKey.RightArrow when snakeDirection != SnakeDirection.LEFT:
                        snakeDirection = SnakeDirection.RIGHT;
                        break;
                }
            }


        }

        enum SnakeDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public void Move()
        {
            if ((DateTime.Now - lastUpdate).TotalSeconds > snakeSpeed * 0.7)
            {
                lastUpdate = DateTime.Now;
                snakeXPosition.Add(headOfSnake.xPosition);
                snakeYPosition.Add(headOfSnake.yPosition);
                Debug.Write((lastUpdate - DateTime.Now).TotalSeconds);

                switch (snakeDirection)
                {
                    case SnakeDirection.UP:
                        headOfSnake.yPosition--;
                        break;

                    case SnakeDirection.DOWN:
                        headOfSnake.yPosition++;
                        break;

                    case SnakeDirection.LEFT:
                        headOfSnake.xPosition--;
                        break;

                    case SnakeDirection.RIGHT:
                        headOfSnake.xPosition++;
                        break;
                }

                if (snakeXPosition.Count() > score)
                {
                    Console.SetCursorPosition(snakeXPosition[0], snakeYPosition[0]);
                    Console.Write(" ");
                    snakeXPosition.RemoveAt(0);
                    snakeYPosition.RemoveAt(0);
                }
            }
        }
    }

    class Berry
    {

        private int screenwidth;
        private int screenheight;


        public Berry(int width, int height)
        {
            screenwidth = width;
            screenheight = height;
        }

        public int[] GenerateBerry()
        {
            Random randomnummer = new Random();
            int berryx = randomnummer.Next(1, screenwidth - 2);
            int berryy = randomnummer.Next(1, screenheight - 2);

            Console.SetCursorPosition(berryx, berryy);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");

            return new int[] { berryx, berryy };
        }
    }

    class Pixel
    {
        public int xPosition { get; set; }
        public int yPosition { get; set; }
        public ConsoleColor color { get; set; }
    }
}