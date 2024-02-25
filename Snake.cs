using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static System.Console;



namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Clear();
            CursorVisible = false;
           
            Snake snake = new Snake();
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
        public ConsoleColor BerryColor = ConsoleColor.Cyan;
        public ConsoleColor SnakeBodyColor = ConsoleColor.Green;
        public ConsoleColor SnakeHeadColor = ConsoleColor.Red;

        public DateTime lastUpdate;
        public int score;
        public double snakeSpeed;

        public Snake(int width = 32, int height = 16)
        {
            screenwidth = width;
            screenheight = height;

            WindowWidth = width * 3;
            WindowHeight = height * 3;

            score = 5;

            snakeXPosition = new List<int>();
            snakeYPosition = new List<int>();

            headOfSnake = new Pixel();
            headOfSnake.xPosition = screenwidth / 2;
            headOfSnake.yPosition = screenheight / 2;
            headOfSnake.color = ConsoleColor.Red;
            lastUpdate = DateTime.Now;
            snakeDirection = SnakeDirection.RIGHT;
            snakeSpeed = 0.6;
        }

        public void StartPlaying()
        {
            Berry berry = new Berry(screenwidth, screenheight);
            int[] berryPosition = berry.GenerateBerry();
            DrawBorder();
            while (true)
            {
                if (CheckCollisionWithWall() || CheckContactWithSelf())
                {
                    break;
                }

                if (CheckCollisionWithBerry(berryPosition))
                {
                    berryPosition = berry.GenerateBerry();
                    score++;
                }

                ProcessUserKeyInput();
                Move();
                DrawSnake();
            }
            DisplayScoreBoard();
        }

        private void DisplayScoreBoard()
        {
            SetCursorPosition(screenwidth / 5, screenheight / 2);
            WriteLine("Game over, Score: " + score);
            SetCursorPosition(screenwidth / 5, screenheight / 2 + 1);
        }

        private void DrawSnake()
        {
            ForegroundColor = SnakeBodyColor;

            for (int i = 0; i < snakeXPosition.Count(); i++)
            {
                SetCursorPosition(snakeXPosition[i], snakeYPosition[i]);
                Write("■");
            }

            SetCursorPosition(headOfSnake.xPosition, headOfSnake.yPosition);
            ForegroundColor = SnakeHeadColor;
            Write("■");
        }

        private bool CheckCollisionWithBerry(int[] berryPosition)
        {
            return berryPosition[0] == headOfSnake.xPosition 
                && headOfSnake.yPosition == berryPosition[1];
        }
        
        private bool CheckContactWithSelf()
        {
            for (int i = 0; i < snakeXPosition.Count(); i++)
            {
                if (snakeXPosition[i] == headOfSnake.xPosition 
                    && snakeYPosition[i] == headOfSnake.yPosition)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCollisionWithWall()
        {
            return headOfSnake.xPosition == screenwidth - 1 ||
                headOfSnake.xPosition == 0 ||
                headOfSnake.yPosition == screenheight - 1 ||
                headOfSnake.yPosition == 0;
        }

        private void ProcessUserKeyInput()
        {


            if (KeyAvailable)
            {
                ConsoleKeyInfo key = ReadKey(true);

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

        private void Move()
        {
            if ((DateTime.Now - lastUpdate).TotalSeconds > snakeSpeed * 0.7)
            {
                lastUpdate = DateTime.Now;
                snakeXPosition.Add(headOfSnake.xPosition);
                snakeYPosition.Add(headOfSnake.yPosition);
                Debug.Write((lastUpdate - DateTime.Now).TotalSeconds);

                MoveHead();
                MoveTail();
            }
        }

        private void MoveHead()
        {
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
        }

        private void MoveTail()
        {
            if (snakeXPosition.Count() > score)
            {
                SetCursorPosition(snakeXPosition[0], snakeYPosition[0]);
                Write(" ");
                snakeXPosition.RemoveAt(0);
                snakeYPosition.RemoveAt(0);
            }
        }

        private void DrawBorder()
        {
            for (int i = 0; i < screenwidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");

                SetCursorPosition(i, screenheight - 1);
                Write("■");
            }

            for (int i = 0; i < screenheight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");

                SetCursorPosition(screenwidth - 1, i);
                Write("■");
            }
        }

        public void SetSnakeHeadColor(ConsoleColor snakeHeadColor)
        {
            SnakeHeadColor = snakeHeadColor;
        }

        public void SetSnakeBodyColor(ConsoleColor snakeBodyColor)
        {
            SnakeBodyColor = snakeBodyColor;
        }
    }

    class Berry
    {

        private int screenwidth;
        private int screenheight;
        private ConsoleColor BerryColor = ConsoleColor.Cyan;


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

            SetCursorPosition(berryx, berryy);
            ForegroundColor = BerryColor;
            Write("■");

            return new int[] { berryx, berryy };
        }

        public void SetBerryColor(ConsoleColor berryColor)
        {
            BerryColor = berryColor;
        }
        
    }

    class Pixel
    {
        public int xPosition { get; set; }
        public int yPosition { get; set; }
        public ConsoleColor color { get; set; }
    }
}
