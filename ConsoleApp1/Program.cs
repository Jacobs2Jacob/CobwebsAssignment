using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static ConcurrentBag<Attemp> attemps = new ConcurrentBag<Attemp>();
        // values are hardcoded since the exam note didn't mention otherwise
        private static int basketWeight = new Random().Next(40, 140);
        private static List<PlayerBase> players = new List<PlayerBase>();
        
        static void Main(string[] args)
        {
            int totalPlayers;
            string type;
            string[] playerTypes = GetPlayerTypes();

            try
            {
                do
                {
                    Console.WriteLine("Please enter number of participating players between 2-8");
                    totalPlayers = int.Parse(Console.ReadLine());
                } while (totalPlayers < 2 || totalPlayers > 8);
                 
                for (int i = 0; i < totalPlayers; i++)
                {
                    Console.WriteLine("Please enter name of player {0}", i + 1);
                    string name = Console.ReadLine();
                    int _type;

                    do
                    {
                        Console.WriteLine("Please select type of player {0}", i + 1);

                        for (int j = 0; j < playerTypes.Length; j++)
                        {
                            Console.WriteLine($"{j} {playerTypes[j].Replace("Player", "")}");
                        }

                        _type = int.Parse(Console.ReadLine());
                        type = playerTypes[_type];
                    } while (!playerTypes.Contains(type));

                    // create instance using reflections
                    PlayerBase instance = (PlayerBase)Activator.CreateInstance(Type.GetType($"ConsoleApp1.{type}", false, true));
                    instance.Name = name;
                    players.Add(instance);
                }

                Console.WriteLine("Basket weight is: {0}", basketWeight);

                var tasks = new List<Task>();

                foreach (var item in players)
                {
                    tasks.Add(StartGame(item));
                }

                var timeout = !Task.WhenAny(tasks).Wait(1500);

                // ensure no task is faulted
                var faulted = tasks.FirstOrDefault(a => a.Status == TaskStatus.Faulted);

                if (faulted != null)
                    throw faulted.Exception;
                
                // timeout has reached
                if (timeout)
                {
                    Console.WriteLine("Time has ran out");
                    PrintClosestWinner();
                    Console.ReadKey();
                    return;
                }
                
                var winner = attemps.FirstOrDefault(a => a.Guess == basketWeight);

                if (winner != null)
                {
                    int winnerAttemps = attemps.Count(a => a.Name == winner.Name);
                    Console.WriteLine("{0} has won the game with {1} attemps!", winner.Name, winnerAttemps);
                }
                else
                {
                    PrintClosestWinner();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.ReadKey();
        }

        private static async Task StartGame(PlayerBase player)
        {
            int guess;
            var playerType = player.GetType();
            MethodInfo method;
            object[] parameters = null;

            // can be more practical by checking if the player overrides Cheat method
            if (playerType.Name == "CheaterPlayer" || playerType.Name == "ThoroughCheaterPlayer")
                method = playerType.GetMethod("Cheat");
            else
                method = playerType.GetMethod("Guess");

            do
            {
                parameters = method == playerType.GetMethod("Cheat") ? new object[] { attemps.Select(a => a.Guess).ToArray() } : null;
                guess = (int)method.Invoke(player, parameters);
                attemps.Add(new Attemp { Time = DateTime.Now, Guess = guess, Name = player.Name });

                // attemps number exceeded
                if (attemps.Count == 100)
                    break;
                // wrong guess
                else
                    await Task.Delay(Math.Abs(basketWeight - guess));

            } while (guess != basketWeight);
        }

        private static void PrintClosestWinner()
        {
            // find closest winner/s
            Attemp closestGuess = attemps.Aggregate((a, b) => Math.Abs(a.Guess - basketWeight) < Math.Abs(b.Guess - basketWeight) ? a : b);

            // multiple closest winners, check for the faster one
            if (attemps.Select(a => a.Guess == closestGuess.Guess).Count() > 1) 
            {
                Attemp newerGuess = attemps.Aggregate((a, b) => a.Time < b.Time ? a : b);
                Console.WriteLine("{0} was the closest player that guessed {1} !", newerGuess.Name, closestGuess.Guess);
            }
            else
            {
                Console.WriteLine("{0} was the closest player that guessed {1} !", closestGuess.Name, closestGuess.Guess);
            }
        }

        private static string[] GetPlayerTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(PlayerBase)))
                       .Select(a => a.Name).ToArray();
        }
    }

    class Attemp
    {
        public int Guess { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
    }
}
