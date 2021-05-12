using System;

namespace KonzolovaHra
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.WindowWidth = 80;
            Console.WindowHeight = 20;
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;
            
            ConsoleGame game = new ConsoleGame(Console.WindowWidth, Console.WindowHeight);
            game.loadPlayerFromFile();
            game.Initialisation();
            game.Play();            
        }
    }
}

