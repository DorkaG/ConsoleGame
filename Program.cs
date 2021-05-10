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
        static Timer consoleClearTimer = null;
        static Player player = null;
        static List<Bullet> playerBulletList = null;
        static List<Bullet> enemyBulletList = null;        
        
        static void Main(string[] args)
        {
            Console.WindowWidth = 80;
            Console.WindowHeight = 20;
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;            
            Enemy enemy = new Enemy(width / 2, 1, "(@__@)", 0);             
            
            Dictionary<int, Player> playerList = new Dictionary<int, Player>();            

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            
            loadPlayerFromFile();
            Initialisation();
            Play();


            void loadPlayerFromFile()
            {
                if (!File.Exists("playerList.json"))
                {
                    //ZIVOTY POTE UPRAVIT ☺
                    playerList.Add(1, new Player(width / 2, height - 1, "☺", 0, 0, 1, "Adam", new List<int>(), 40));
                    playerList.Add(2, new Player(width / 2, height - 1, "☺", 0, 0, 1, "Bedřich", new List<int>(), 40));
                    playerList.Add(3, new Player(width / 2, height - 1, "☺", 0, 0, 1, "Cecílie", new List<int>(), 40));

                }
                else
                {
                    string playerListRead = File.ReadAllText("playerList.json");
                    playerList = JsonConvert.DeserializeObject<Dictionary<int, Player>>(playerListRead, settings);
                    foreach (var item in playerList)
                    {
                        item.Value.X = width / 2;
                        item.Value.Y = height - 1;
                        item.Value.FormerX = 0;
                        item.Value.Points = 0;
                        item.Value.Life = 1;     ///TOHLE POTOM UPRAVT
                        item.Value.NumberOfBullets = 40;
                    }
                }
            }
            
            void Initialisation()
            {                
                playerBulletList = new List<Bullet>();               
                enemyBulletList = new List<Bullet>();                

                Console.CursorVisible = false;
                Console.Clear();                

                var autoEvent = new AutoResetEvent(false);

                Console.WriteLine("Vítejte ve hře. \n Nejdříve si vyberte, s jakým hráčem budete hrát. \n Hráče si můžete buď nově vytvořit, nebo si ho vybrat z existujícího seznamu. \n \n \"Chci si vytvořit nového hráče\" - stiskněte 1 \n \"Chci si vybrat již existujícího hráče\" - stiskněte 2");
                ChoosePlayer();
                
                consoleTimer = new Timer(ConsoleRender, autoEvent, 1000, 300);
                consoleClearTimer = new Timer(ClearConsole, autoEvent, 1000, 2000);
                enemyShooting = new Timer(EnemyIsShooting, autoEvent, 5000, 8000);
                enemyMove = new Timer(enemy.MoveEnemy, autoEvent, 1000, 1000);                                               
            }

            void Play()
            {
                TimeSpan threadSleepTimeSpan = TimeSpan.FromMilliseconds(301);

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
                            case ConsoleKey.Spacebar:
                                playerBulletList.Add(new PlayerBullet(player.X, player.Y - 1, "|", 0, true));
                                player.NumberOfBullets--;
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

            void ChoosePlayer()
            {
                ChoiceMaking playerChoice = new ChoiceMaking(new Dictionary<int, Action>() { { 1, () => EnterPlayerName() }, { 2, () => { if (playerList != null) ChoosePlayerFromList(); else { Console.WriteLine("Bohužel, seznam hráčů je prázdný. Stiskněte 1 a založte si nového hráče."); ChoosePlayer(); } } } });
                playerChoice.Choose();               
            }


            void EnterPlayerName()
            {
                Console.WriteLine("Zadejte jméno nového hráče:");
                bool nameEntered = false;                
                do
                {
                    bool nameAlreadyExists = false;
                    string name = Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(name)) Console.WriteLine("Tohle není jméno. Zadejte jméno s platnými znaky:");                    
                    else
                    {
                        foreach(var item in playerList)
                        {
                            if (item.Value.Name == name)
                            {
                                nameAlreadyExists = true;
                                Console.WriteLine("Toto jméno už existuje. Zadejte jiné jméno:");
                            }
                        }

                        if (!nameAlreadyExists)
                        {
                            nameEntered = true;
                            playerList.Add(playerList.Count + 1, new Player(width / 2, height - 1, "☺", 0, 0, 1, name, new List<int>(), 40));
                            player = playerList[playerList.Count];
                            Console.WriteLine("Výborně, budete hrát jako \"" + player.Name + "\". Zmáčkněte Enter a hra může začít.");
                            Console.ReadLine();
                        }                        
                    }
                } while (!nameEntered);
            }

            void ChoosePlayerFromList()
            {                
                Console.WriteLine("Zde je seznam hráčů:");
                PlayerListRender();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Zadejte číslo hráče, se kterým chcete hrát:");

                /* var actions = playerList.ToDictionary(x => x.Key, x =>
                    () => {
                        player = playerList[x.Key];
                        Console.WriteLine("Výborně, budete hrát jako \"" + player.Name + "\". Zmáčkněte Enter a hra může začít.");
                        Console.ReadLine();
                    }
                );*/

                //TOHLE JESTE TAKZ PREDELAT NA TRIDU???

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
                var playerListAlphabetically = playerList.OrderBy(p => p.Value.Name);
                foreach (var item in playerListAlphabetically)
                {
                    string highestScore = "";
                    if (item.Value.PointList.Count == 0) highestScore = "Ještě nehrál(a).";
                    else highestScore = item.Value.PointList.Max().ToString();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(item.Value.Name);
                    Console.ResetColor();
                    Console.Write(" - číslo hráče: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(item.Key);
                    Console.ResetColor();
                    Console.Write(", nejvyšší dosažené skóre: " + highestScore);
                }
            }

            void ConsoleRender(Object stateInfo)
            {
                if (player.Life == 0 || player.NumberOfBullets == 0)
                {
                    consoleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    enemyShooting.Change(Timeout.Infinite, Timeout.Infinite);
                    enemyMove.Change(Timeout.Infinite, Timeout.Infinite);
                    consoleClearTimer.Change(Timeout.Infinite, Timeout.Infinite);
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(enemy.Look);
                    Console.ResetColor();

                    Console.SetCursorPosition(player.FormerX, player.Y);
                    Console.Write(" ");
                   
                    Console.SetCursorPosition(player.X, player.Y);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(player.Look);
                    Console.ResetColor();

                    BulletRender(enemyBulletList);
                    BulletRender(playerBulletList);
                    IsEnemyShot();
                    IsPlayerShot();
                }

            }

            void ClearConsole(Object stateInfo)
            {
                if (player.Life > 0 && player.NumberOfBullets > 0) Console.Clear();
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

            void SaveTheResults()
            {
                player.PointList.Add(player.Points);  //adding points to player PointList                

                //saving into file
                string playerListWrite = JsonConvert.SerializeObject(playerList, settings);
                File.WriteAllText("playerList.json", playerListWrite);
            }

            void EndOfGame(int score)
            {              
                string finalPhrase = "";
                string points = "";

                if (score == 1) points = "bod";
                else if (score == 0 || score > 4) points = "bodů";
                else if (score > 1 && score < 5) points = "body";

                if (player.Life == 0 ) finalPhrase = "Zranění neslučitelné s životem, konec hry! Vaše skóre: " + score + " " + points;
                else if (player.NumberOfBullets == 0) finalPhrase = "Došly náboje, konec hry! Vaše skóre: " + score + " " + points;
                string question = "Co chcete udělat teď?";
                string choice1 = "Dát si další hru: stiskněte 1";
                string choice2 = "Ukončit hru: stiskněte 2";
                string choice3 = "Zobrazit výsledky hráčů: stiskněte 3";

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition((width - finalPhrase.Length) / 2, height / 2);                
                Console.WriteLine(finalPhrase);               
                Console.ResetColor();

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
                Console.SetCursorPosition(width / 2, (height / 2) + 7);
                Console.WriteLine(" ");
                Console.SetCursorPosition(width / 2, (height / 2) + 7);

                ChoiceMaking choiceEndOfGame = new ChoiceMaking(new Dictionary<int, Action>() { {1, () => { loadPlayerFromFile(); Initialisation(); } }, {2, () => Environment.Exit(0)}, {3, () => ExploreResults() } });
                choiceEndOfGame.Choose();                
            }                                    

            void ExploreResults()
            {
                Console.Clear();
                Console.WriteLine("Pojďme prozkoumat výsledky hráčů.");
                Console.WriteLine();
                Console.WriteLine("Vypsat první tři hráče s nejvyšším skóre: stiskněte 1");
                Console.WriteLine("Vyhledat konkrétního hráče dle jména: stiskněte 2");
                Console.WriteLine("Vypsat všechny hráče podle abecedy: stiskněte 3");
                Console.WriteLine("Vypsat všechny hráče podle nejvyššího počtu získaných bodů: stiskněte 4");

                ChoiceMaking exploreChoice = new ChoiceMaking(new Dictionary<int, Action>() { { 1, () => RenderBestThree() }, {2, () => RenderSpecificPlayer() }, { 3, () => RenderAlphabetically() }, {4, () => RenderFromTheBest() } });
                exploreChoice.Choose();                
            }

            void RenderBestThree()
            {
                Console.Clear();

                //a subdictionary is created, into which only player with some results are copied:
                Dictionary<int, Player> subPlayerList = new Dictionary<int, Player>();
                foreach (KeyValuePair<int, Player> item in playerList)
                {
                    if (item.Value.PointList.Count > 0) subPlayerList.Add(item.Key, item.Value);
                }
                

                //take 3 best, unless there are less than 3 players with results:
                int numberOfBest = 3;
                if (subPlayerList.Count < 3) numberOfBest = subPlayerList.Count;

                var bestthree = subPlayerList.OrderByDescending(p => p.Value.PointList.Max()).Take(numberOfBest).Select(p => p.Value);
                Console.WriteLine("První tři nejlepší: ");
                int order = 1;
                foreach(var item in bestthree)
                {
                    Console.WriteLine($"{order}: {item.Name}, nejvyšší počet získaných bodů: {item.PointList.Max()}");
                    order++;
                }
                if (numberOfBest < 3) Console.WriteLine("(Ostatní ještě nehráli.)");
                Console.WriteLine();
                Console.WriteLine("A co dál?");
                Console.WriteLine("Pokračovat v prozkoumávání výsledků - stikněte 1");
                Console.WriteLine("Ukončit hru - stiskněte 2");
                Console.WriteLine("Začít novou hru - stiskněte 3");

                ChoiceMaking choiceAfterExploring = new ChoiceMaking(new Dictionary<int, Action>() { {1, () => ExploreResults() }, { 2, () => Environment.Exit(0) }, {3, () => { loadPlayerFromFile(); Initialisation(); } } });
                choiceAfterExploring.Choose();               
            }

            void RenderSpecificPlayer()
            {
                Console.Clear();
                Console.WriteLine("Zadejte jméno hráče, kterého hledáte:");
                bool nameEntered = false;
                do
                {
                    string name = Console.ReadLine();
                    if (String.IsNullOrWhiteSpace(name)) Console.WriteLine("Tohle není jméno. Zadejte jméno s platnými znaky:");
                    else
                    {                        
                        var chosenPlayer = playerList.Where(p => p.Value.Name == name).Select(p => p.Value).ToList();
                        if (chosenPlayer.Count == 0) Console.WriteLine("Bohužel, toto jméno v seznamu hráčů neexistuje. Zadejte jiné jméno:");
                        else
                        {
                            nameEntered = true;
                            Console.WriteLine();
                            foreach(var item in chosenPlayer)
                            {
                                if (item.PointList.Count > 0) Console.WriteLine($"Hráč \"{item.Name}\", nejvyšší počet získaných bodů: {item.PointList.Max()}");
                                else Console.WriteLine($"Hráč \"{item.Name}\", ještě nehrál(a)");
                            }                            
                        } 
                    }
                } while (!nameEntered);

                Console.WriteLine();
                Console.WriteLine("A co dál?");
                Console.WriteLine("Pokračovat v prozkoumávání výsledků - stikněte 1");
                Console.WriteLine("Ukončit hru - stiskněte 2");
                Console.WriteLine("Začít novou hru - stiskněte 3");

                ChoiceMaking choiceAfterExploring = new ChoiceMaking(new Dictionary<int, Action>() { { 1, () => ExploreResults() }, { 2, () => Environment.Exit(0) }, { 3, () => { loadPlayerFromFile(); Initialisation(); } } });
                choiceAfterExploring.Choose();                
            }

            void RenderAlphabetically()
            {
                Console.Clear();
                var sortedAlphabetically = playerList.OrderBy(p => p.Value.Name).Select(p => p.Value);
                Console.WriteLine("Seznam hráčů seřazených podle abecedy: ");
                Console.WriteLine();
                int order = 1;
                foreach (var item in sortedAlphabetically)
                {
                    if (item.PointList.Count > 0) Console.WriteLine($"{order}: {item.Name}, nejvyšší počet získaných bodů: {item.PointList.Max()}");
                    else Console.WriteLine($"{order}: {item.Name}, ještě nehrál(a)");
                    order++;
                }

                Console.WriteLine();
                Console.WriteLine("A co dál?");
                Console.WriteLine("Pokračovat v prozkoumávání výsledků - stikněte 1");
                Console.WriteLine("Ukončit hru - stiskněte 2");
                Console.WriteLine("Začít novou hru - stiskněte 3");

                ChoiceMaking choiceAfterExploring = new ChoiceMaking(new Dictionary<int, Action>() { { 1, () => ExploreResults() }, { 2, () => Environment.Exit(0) }, { 3, () => { loadPlayerFromFile(); Initialisation(); } } });
                choiceAfterExploring.Choose();                
            }

            void RenderFromTheBest()
            {
                Console.Clear();

                //a subdictionary is created, into which only player with some results are copied:
                Dictionary<int, Player> subPlayerList = new Dictionary<int, Player>();
                foreach (KeyValuePair<int, Player> item in playerList)
                {
                    if (item.Value.PointList.Count > 0) subPlayerList.Add(item.Key, item.Value);
                }

                var sortedByBest = subPlayerList.OrderByDescending(p => p.Value.PointList.Max()).Select(p => p.Value);
                Console.WriteLine("Seznam hráčů seřazených podle nejlepších výsledků: ");
                Console.WriteLine();
                int order = 1;
                foreach (var item in sortedByBest)
                {
                    Console.WriteLine($"{order}: {item.Name}, nejvyšší počet získaných bodů: {item.PointList.Max()}");
                    order++;
                }
                if (subPlayerList.Count < playerList.Count) Console.WriteLine("(Ostatní ještě nehráli.)");
                Console.WriteLine();
                Console.WriteLine("A co dál?");
                Console.WriteLine("Pokračovat v prozkoumávání výsledků - stikněte 1");
                Console.WriteLine("Ukončit hru - stiskněte 2");
                Console.WriteLine("Začít novou hru - stiskněte 3");

                ChoiceMaking choiceAfterExploring = new ChoiceMaking(new Dictionary<int, Action>() { { 1, () => ExploreResults() }, { 2, () => Environment.Exit(0) }, { 3, () => { loadPlayerFromFile(); Initialisation(); } } });
                choiceAfterExploring.Choose();               
            }

        }

    }
}

