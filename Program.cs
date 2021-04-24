using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace KonzolovaHra
{
    class Program
    {
        static Timer consoleTimer = null;
        static void Main(string[] args)
        {
            Console.WindowWidth = 70;
            Console.WindowHeight = 20;
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;            
            Enemy enemy = new Enemy(width / 2, 1, "@@@@", 0);
            Player player = new Player(width / 2, height - 1, "X", 0, 0, 1);            
            List<Bullet> playerBulletList = new List<Bullet>();
            List<Bullet> enemyBulletList = new List<Bullet>();

            TimeSpan threadSleepTimeSpan = TimeSpan.FromMilliseconds(301);

            Console.CursorVisible = false;
            Console.Clear();

            var autoEvent = new AutoResetEvent(false); //???


            consoleTimer = new Timer(ConsoleRender, autoEvent, 1000, 300);
            var consoleClearTimer = new Timer(ClearConsole, autoEvent, 1000, 2000);
            var enemyShooting = new Timer(EnemyIsShooting, autoEvent, 5000, 8000);
            var enemyMove = new Timer(enemy.MoveEnemy, autoEvent, 1000, 1000);



            while (true)
            {
                if (Console.KeyAvailable)   //neceka, az neco zmacknu
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.RightArrow:                                                    
                            player.MovePlayer(1);
                            break;
                        case ConsoleKey.LeftArrow:                            
                            player.MovePlayer(-1);
                            break;
                        case ConsoleKey.UpArrow:                            
                            playerBulletList.Add(new PlayerBullet(player.X, player.Y - 1, "|", 0, true));
                            break;
                    }
                }

                while (Console.KeyAvailable)    //zajisti, ze pro jedno zmacknuti to udela jednu akci, pohyb se "nezasekava"
                {
                    Console.ReadKey(true);
                }

                Thread.Sleep(threadSleepTimeSpan);   //uspani while, aby byl pohyb plynuly
            }

            void ConsoleRender(Object stateInfo)
            {                
                if (player.Life == 0)
                {
                    Console.SetCursorPosition(width / 3, height / 2);
                    Console.Write("Konec hry! Vaše skóre: " + player.Points + " bodů");
                    Console.ReadLine();
                    consoleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }

                Console.SetCursorPosition(1, height);
                Console.Write("Body: " + player.Points);

                Console.SetCursorPosition(width - 12, height);
                Console.Write("Životy: " + player.Life);

                Console.SetCursorPosition(enemy.FormerX, enemy.Y);
                Console.Write(" ");

                Console.SetCursorPosition(enemy.X, enemy.Y);
                Console.Write(enemy.Look);

                Console.SetCursorPosition(player.FormerX, player.Y);
                Console.Write(" ");

                Console.SetCursorPosition(player.X, player.Y);
                Console.Write(player.Look);
            
                BulletRender(enemyBulletList);
                BulletRender(playerBulletList);
                IsEnemyShot();
                IsPlayerShot();

            }

            void ClearConsole(Object stateInfo)
            {
                Console.Clear();
            }

            void EnemyIsShooting(Object stateInfo)
            {
                enemyBulletList.Add(new EnemyBullet(enemy.X, 1, "O", 0, true));
            }


            void BulletRender(List<Bullet> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].IsShooting)
                    {
                        Console.SetCursorPosition(list[i].X, list[i].Y);
                        Console.Write(list[i].Look);
                        list[i].Shoot();
                        Console.SetCursorPosition(list[i].X, list[i].FormerY);
                        Console.Write(" ");

                        Console.SetCursorPosition(list[i].X, list[i].Y);
                        Console.Write(list[i].Look);
                    }
                }                
            }

            void IsEnemyShot()
            {
                foreach (PlayerBullet bullet in playerBulletList)
                {
                    if (bullet.X >= enemy.X && bullet.X <= enemy.X + 3 && bullet.Y == enemy.Y)
                    {
                        Console.SetCursorPosition(bullet.X, bullet.Y);
                        Console.Write("####");

                        player.Points++;
                    }
                }
            }

            void IsPlayerShot()
            {
                foreach (EnemyBullet bullet in enemyBulletList)
                {
                    if (bullet.X == player.X && bullet.Y == player.Y)
                    {
                        Console.SetCursorPosition(bullet.X, bullet.Y);
                        Console.Write("####");

                        player.Life--;
                    }
                }
            }
            
        }

    }
}

