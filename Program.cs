namespace SNAKEGAME
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DateTime d1 = DateTime.Now;
                
                Console.CursorVisible = false;
                SnakeControl snake = new SnakeControl();
                Thread _game = new Thread(snake.GetKey);
                _game.Start();
                while(!snake.EatSelf() && SnakeControl.running && SnakeControl._speed > 0)
                {
                    Console.Clear();
                    snake.DrawBoard();
                    snake.SetUpBoard();
                    snake.ForDirection();
                    snake.MoveSnakeHead();
                    snake.EatFood();
                    snake.SpawnBody();
                    snake.PopUpFood();
                    snake.ShowScore();
                    Thread.Sleep(SnakeControl._speed);
                }
                if (SnakeControl.running)
                {
                    snake.Stop();
                    Console.Write(new string(' ', "Press any key else to quit".Length));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Game over\nPress any key else to quit");
                }
                else
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    Console.WriteLine("Game over");
                    Console.Write(new string(' ', "Press any key else to quit".Length));
                }
                _game.Join();

                DateTime d2 = DateTime.Now;
                
                string textPath = @"\PlayHistory.txt";
                using(StreamWriter writer = new StreamWriter(textPath, true))
                {
                    writer.WriteLine(d1 + " - " + d2);
                    writer.WriteLine("Score: " + SnakeControl._score);
                    writer.WriteLine();
                }
            }
            catch(Exception e)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(e.Message);
            }
        }
    }
}