﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace KonzolovaHra
{
    class Program
    {
        static Timer consoleTimer = null;
        static Timer enemyShooting = null;
        static Timer enemyMove = null;
        static Player player = null;
        static List<Bullet> playerBulletList = null;
        static List<Bullet> enemyBulletList = null;
        //static List<Bullet> playerBulletList = new List<Bullet>();
        //static List<Bullet> enemyBulletList = new List<Bullet>();
        static void Main(string[] args)
        {
            Console.WindowWidth = 70;
            Console.WindowHeight = 20;
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;            
            Enemy enemy = new Enemy(width / 2, 1, "(@__@)", 0);
            //Player player = new Player(width / 2, height - 1, "☺", 0, 0, 1, "Hráč 1");
            
            Dictionary<int, Player> playerList = new Dictionary<int, Player>();
            //string fileName = "";
            //string pathToFile = "";
            //string pathToDatabase = "";

            //string fileName = "playerList.json";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //string PlayerListJson;
            //string pathToFile = @"C:\ProgramData"; //do bin radeji
            //string pathToDatabase = Path.Combine(pathToFile, fileName);

            //if (!Directory.Exists(Path.Combine(@"C:\ProgramData", "playerList.txt")))
            loadPlayerFromFile();
            Play();


            void loadPlayerFromFile()
            {
                if (!File.Exists("playerList.json"))
                {
                    //ZIVOTY POTE UPRAVIT
                    playerList.Add(1, new Player(width / 2, height - 1, "☺", 0, 0, 2, "Adam", new List<int>()));
                    playerList.Add(2, new Player(width / 2, height - 1, "☺", 0, 0, 2, "Bedřich", new List<int>()));
                    playerList.Add(3, new Player(width / 2, height - 1, "☺", 0, 0, 2, "Cecílie", new List<int>()));

                }
                else
                {
                    string playerListRead = File.ReadAllText("playerList.json");
                    playerList = JsonConvert.DeserializeObject<Dictionary<int, Player>>(playerListRead, settings);
                    foreach (var item in playerList)
                    {
                        item.Value.X = width / 2;
                        item.Value.Y = height - 1;
                        item.Value.Points = 0;
                        item.Value.Life = 2;     ///TOHLE POTOM UPRAVT
                    }
                }
            }
            
            void Play()
            {
                //Player player = playerList[1];
                //Player player = new Player(width / 2, height - 1, "☺", 0, 0, 1, "pokus", new List<int>());

                //List<Bullet> playerBulletList = new List<Bullet>();
                playerBulletList = new List<Bullet>();
                //List<Bullet> enemyBulletList = new List<Bullet>();
                enemyBulletList = new List<Bullet>();

                TimeSpan threadSleepTimeSpan = TimeSpan.FromMilliseconds(301);

                Console.CursorVisible = false;
                Console.Clear();

                var autoEvent = new AutoResetEvent(false); //???

                Console.WriteLine("Vítejte ve hře. \n Nejdříve si vyberte, s jakým hráčem budete hrát. \n Hráče si můžete buď nově vytvořit, nebo si ho vybrat z existujícího seznamu. \n \n \"Chci si vytvořit nového hráče\" - stiskněte 1 \n \"Chci si vybrat hráče ze seznamu\" - stiskněte 2");
                ChoosePlayer();

                consoleTimer = new Timer(ConsoleRender, autoEvent, 1000, 300);
                var consoleClearTimer = new Timer(ClearConsole, autoEvent, 1000, 2000);
                enemyShooting = new Timer(EnemyIsShooting, autoEvent, 5000, 8000);
                enemyMove = new Timer(enemy.MoveEnemy, autoEvent, 1000, 1000);

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
            }




            void ConsoleRender(Object stateInfo)
            {
                if (player.Life == 0)
                {
                    consoleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    enemyShooting.Change(Timeout.Infinite, Timeout.Infinite);
                    enemyMove.Change(Timeout.Infinite, Timeout.Infinite);
                    int actualScore = player.Points;
                    SaveTheResults();
                    EndOfGame(actualScore);                                        
                }
                else
                {
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

            }

            void ClearConsole(Object stateInfo)
            {                 
                 if (player.Life > 0) Console.Clear();
            }

            void SaveTheResults()
            {
                player.PointList.Add(player.Points);  //adding points to player PointList                

                //saving into file
                string playerListWrite = JsonConvert.SerializeObject(playerList, settings);
                File.WriteAllText("playerList.json", playerListWrite);
            }

            void EndOfGame(int score)
            {
                string finalPhrase = "Konec hry! Vaše skóre: " + score + " bodů";
                string question = "Co chcete udělat teď?";
                string choice1 = "Dát si další hru: stiskněte 1";
                string choice2 = "Ukončit hru: stiskněte 2";
                string choice3 = "Prohledat seznam hráčů: stiskněte 3";
                Console.SetCursorPosition((width - finalPhrase.Length) / 2, height / 2);
                Console.WriteLine(finalPhrase); //30
                                                                                //jen pro kontrolu:
                                                                                //Console.WriteLine();
                                                                                //PlayerListRender();
                Console.SetCursorPosition((width - question.Length) / 2, (height / 2) + 2);
                Console.WriteLine(question);
                Console.SetCursorPosition((width - choice1.Length) / 2, (height / 2) + 4);
                Console.WriteLine(choice1);
                Console.SetCursorPosition((width - choice2.Length) / 2, (height / 2) + 5);
                Console.WriteLine(choice2);
                Console.SetCursorPosition((width - choice3.Length) / 2, (height / 2) + 6);
                Console.WriteLine(choice3);
                Console.SetCursorPosition(width - 12, height);
                Console.Write("Životy: " + player.Life);

                bool choiceDone = false;

                do
                {
                    string choice = Console.ReadLine();
                    int number;
                    bool isNumber = int.TryParse(choice, out number);

                    if (!isNumber || (number != 1 && number != 2 && number != 3))
                    {
                        Console.WriteLine("Stiskněte 1, nebo 2, nebo 3.");
                    }
                    else
                    {
                        if (number == 1)
                        {
                            choiceDone = true;
                            loadPlayerFromFile();
                            Play();
                        }
                        if (number == 2)
                        {
                            
                        }
                        if (number == 3)
                        {

                        }

                    }
                } while (!choiceDone);

               // Console.ReadLine();
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
                    if (bullet.X >= enemy.X && bullet.X <= enemy.X + 5 && bullet.Y == enemy.Y)
                    {
                        Console.SetCursorPosition(bullet.X, bullet.Y);
                        Console.Write(">BANG<");

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

            void ChoosePlayer()
            {
                bool choiceDone = false;

                do
                {
                    string playerChoice = Console.ReadLine();
                    int number;
                    bool isNumber = int.TryParse(playerChoice, out number);

                    if (!isNumber || (number != 1 && number != 2))
                    {
                        Console.WriteLine("Stiskněte 1, nebo 2.");
                    }
                    else
                    {                        
                        if (number == 1)
                        {
                            choiceDone = true;
                            EnterPlayerName();
                        }
                        if (number == 2)
                        {
                            if (playerList != null)
                            {
                                choiceDone = true;
                                ChoosePlayerFromList();
                            }
                            else Console.WriteLine("Bohužel, seznam hráčů je prázdný. Stiskněte 1 a založte si nového hráče.");
                        }
                            
                    }
                } while (!choiceDone);
            }


            void EnterPlayerName()
            {
                Console.WriteLine("Zadejte jméno nového hráče:");
                bool nameEntered = false;
                do
                {
                    string name = Console.ReadLine();
                    if (String.IsNullOrWhiteSpace(name)) Console.WriteLine("Tohle není jméno. Zadejte jméno s platnými znaky:");
                    else
                    {
                        nameEntered = true;
                        playerList.Add(playerList.Count + 1, new Player(width / 2, height - 1, "☺", 0, 0, 1, name, new List<int>()));
                        player = playerList[playerList.Count];
                        Console.WriteLine("Výborně, budete hrát jako \"" + player.Name + "\". Zmáčkněte Enter a hra může začít.");
                        Console.ReadLine();
                    }
                } while (!nameEntered);
            }

            void ChoosePlayerFromList()
            {
                Console.WriteLine("Zde je seznam hráčů:");
                PlayerListRender();
                Console.WriteLine();
                Console.WriteLine("Zadejte číslo hráče, se kterým chcete hrát:");

                bool choiceDone = false;
                do
                {
                    string playerChoice = Console.ReadLine();
                    int number;
                    bool isNumber = int.TryParse(playerChoice, out number);

                    if (!isNumber || (number < 1 || number > playerList.Count))
                    {
                        Console.WriteLine("Stiskněte platné číslo hráče:");
                    }
                    else
                    {
                        choiceDone = true;
                        player = playerList[number];
                        Console.WriteLine("Výborně, budete hrát jako \"" + player.Name + "\". Zmáčkněte Enter a hra může začít.");
                        Console.ReadLine();
                    }
                } while (!choiceDone);
            }

            void PlayerListRender()
            {
                foreach (var item in playerList)
                {
                    string highestScore = "";
                    if (item.Value.PointList.Count == 0) highestScore = "Ještě nehrál(a).";
                    else highestScore = item.Value.PointList.Max().ToString();
                    Console.WriteLine(item.Key + " " + item.Value.Name + " nejvyšší dosažené skóre: " + highestScore);
                }
            }

        }

    }
}

