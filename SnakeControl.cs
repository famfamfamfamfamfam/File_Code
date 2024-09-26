using System.Collections.Concurrent;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;

namespace SNAKEGAME
{
    class SnakeControl
    {
        Point food;
        bool foodExist;
        int row;
        int col;
        int speed;
        public int _speed { get => speed; }
        bool _running;
        public bool running { get => _running; }
        int score;
        public int _score { get => score; }
        string direction;
        string subDirection;
        Point[] body;
        Point _head;
        string[, ] board;
        string[, ] hiddenBoard;

        public SnakeControl()
        {
            food = new Point(-1, -1);
            foodExist = false;
            row = 20;
            col = 50;
            speed = 300;
            _running = true;
            direction = "Right";
            subDirection = "Right";
            body = new Point[] { new Point(-1, -1) };
            _head = new Point(10, 1);
            board = new string[row, col];
            hiddenBoard = new string[row, col];
        }

        public void DrawBoard()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (i == 0 || j == 0 || i == row - 1 || j == col - 1)
                    {
                        board[i, j] = "~";
                    }
                    else if (i == _head.X && j == _head.Y)
                    {
                        board[i, j] = "O";
                    }
                    else
                    {
                        bool isBodyPart = false;
                        for (int count = 0; count < body.Length; count++)
                        {
                            if (i == body[count].X && j == body[count].Y)
                            {
                                board[i, j] = "o";
                                isBodyPart = true;
                                break;
                            }
                        }
                        if (!isBodyPart)
                        {
                            if (i == food.X && j == food.Y)
                            {
                                board[i, j] = "$";
                            }
                            else
                            {
                                board[i, j] = " ";
                            }
                        }
                    }
                }
            }
        }
        public void MoveSnakeHead()
        {
            switch (direction)
            {
                case "Right":
                    _head.Y += 1;
                    if (_head.Y == col - 1)
                    {
                        _head.Y = 1;
                    }
                    break;
                case "Left":
                    _head.Y -= 1;
                    if (_head.Y == 0)
                    {
                        _head.Y = col - 2;
                    }
                    break;
                case "Up":
                    _head.X -= 1;
                    if (_head.X == 0)
                    {
                        _head.X = row - 2;
                    }
                    break;
                case "Down":
                    _head.X += 1;
                    if (_head.X == row - 1)
                    {
                        _head.X = 1;
                    }
                    break;
            }
        }
        static ConcurrentQueue<string> keys = new ConcurrentQueue<string>();
        public void GetKey()
        {
            while(_running)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (subDirection == "Up" || subDirection == "Down")
                        {
                            subDirection = "Right";
                            keys.Enqueue("Right");
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (subDirection == "Up" || subDirection == "Down")
                        {
                            subDirection = "Left";
                            keys.Enqueue("Left");
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (subDirection == "Right" || subDirection == "Left")
                        {
                            subDirection = "Up";
                            keys.Enqueue("Up");
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (subDirection == "Right" || subDirection == "Left")
                        {
                            subDirection = "Down";
                            keys.Enqueue("Down");                            
                        }
                        break;
                    default:
                        _running = false;
                        break;
                }
            }
        }
        public void ForDirection()
        {
            if (keys.TryDequeue(out string? result))
            {
                direction = result;
            }
        }
        public void SetUpBoard()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if(board[i, j] != hiddenBoard[i, j])
                    {
                        if (board[i, j] == "~")
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            if (board[i, j] == "$")
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            else if (board[i, j] == "O" || board[i, j] == "o")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else {}
                    }
                    Console.SetCursorPosition(j, i);
                    Console.Write(board[i, j]);
                    }
                }
            }
            Console.ResetColor();
        }
        public void BehindTheBoard()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    hiddenBoard[i, j] = board[i, j];
                }
            }
        }
        public void PopUpFood()
        {
            Random rand = new Random();
            int x = rand.Next(1, row - 2);
            int y = rand.Next(1, col - 2);
            if (x != _head.X && y != _head.Y)
            {
                if (foodExist == false)
                {
                    food.X = x;
                    food.Y = y;
                    foodExist = true;
                }
            }
        }
        public void EatFood()
        {
            if (_head.X == food.X && _head.Y == food.Y)
            {
                Array.Resize(ref body, body.Length + 1);
                body[body.Length - 1] = new Point(-1, -1);
                foodExist = false;
                speed -= 15;
                score += 1;
            }
        }
        public void SpawnBody()
        {
            for (int i = body.Length - 1; i > 0; i--)
            {
                body[i].X = body[i - 1].X;
                body[i].Y = body[i - 1].Y;
            }
            body[0].X = _head.X;
            body[0].Y = _head.Y;
        }
        public bool EatSelf()
        {
            for (int i = 1; i < body.Length; i++)
            {
                if (_head.X == body[i].X && _head.Y == body[i].Y)
                {
                    return true;
                }
            }
            return false;
        }
        public void Stop()
        {
            ConsoleColor[] bling = { ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow };
            if (speed <= 0)
            {
                Console.SetCursorPosition(0, row + 1);
                Console.Write(new string(' ', "Press any key else to quit".Length));
                for(int count = 0; count < 6; count++)
                {
                    foreach (ConsoleColor color in bling)
                    {
                        Console.ForegroundColor = color;
                        Console.SetCursorPosition(0, row);
                        Console.WriteLine("You win  ");
                        Thread.Sleep(75);
                    }
                }
                Console.ResetColor();
            }
            else
            {
                Console.SetCursorPosition(0, row);
                Console.WriteLine("You lose ");
            }
        }
        public void ShowScore()
        {
            Console.SetCursorPosition(0, row);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Score: " + score);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press any key else to quit");
            Console.ResetColor();
        }
    }
}