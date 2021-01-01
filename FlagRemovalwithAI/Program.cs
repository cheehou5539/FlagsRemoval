using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Console;

namespace FlagRemovalwithAI
{
    public class Program
    {

        static void Main(string[] args)
        {
            int[] FLAGS_TO_REMOVED = new[] { 3,4 };

            const int STARTING_FLAGS = 21;
            const int WINNING_CONDITION = 1;

            PrintRules(STARTING_FLAGS, WINNING_CONDITION, FLAGS_TO_REMOVED);
            string playAgain = string.Empty;
            string input = string.Empty;
            bool startFirst = false;

            do
            {
                GameStart(STARTING_FLAGS, WINNING_CONDITION, FLAGS_TO_REMOVED, startFirst);
                WriteLine("Play Again? Y/N");
                playAgain = ReadLine();
            }
            while (playAgain.ToUpper() != "N" || !int.TryParse(input, out _));
        }



        internal static void GameStart(int STARTING_FLAGS, int WINNING_CONDITION, int[] arrayFlagsToRemoved, bool isHumanPlay)
        { 
          
            string input;
            

            List<int> FlagsToRemove = arrayFlagsToRemoved.OfType<int>().ToList();

            FlagRemovalBrain brain = new FlagRemovalBrain(WINNING_CONDITION, STARTING_FLAGS, FlagsToRemove);

            int remainingFlags = STARTING_FLAGS;
            int turn = 0;
            bool humanWon = false;
            bool draw = false;

            List<ResultTable> ListOfResultTable = new List<ResultTable>();//keep track the result of each turn

            do
            {

                WriteLine("Your turn, enter the number of flag(s) that you willing to pull ({0} only).", string.Join(" or ", FlagsToRemove.Select(i => i)));
                input = ReadLine();

                if (!int.TryParse(input, out int pulledFlag) || !FlagsToRemove.Contains(pulledFlag) || pulledFlag > remainingFlags)
                {
                    WriteLine("Please enter number {0 } only", string.Join(" or ", FlagsToRemove.Select(i => i)));
                    continue;
                }


                /**Human's turn  */
                (remainingFlags, turn) = ProcessDecision(remainingFlags, turn, pulledFlag, brain, ListOfResultTable, true);
                if (remainingFlags <= WINNING_CONDITION)
                { humanWon = true; break; }


                brain.FlagsThatCanBeRemove.RemoveAll(t => t > remainingFlags);//remove availabe flags

                if (brain.FlagsThatCanBeRemove.Count == 0)//draw
                { draw = true; break; }

                /**Computer's turn  */
                (remainingFlags, turn) = ProcessDecision(remainingFlags, turn, pulledFlag, brain, ListOfResultTable, false);
                if (remainingFlags <= WINNING_CONDITION) break;

                FlagsToRemove.RemoveAll(t => t > remainingFlags);//remove availabe flags

                if (FlagsToRemove.Count == 0)//draw
                { draw = true; break; }


            } while (remainingFlags > WINNING_CONDITION);

            if (draw)
                WriteLine("Game Ended! Draw. \n\n\n");
            else if (humanWon)
                WriteLine("Game Ended! Good Job, you beat the computer! You are a good strategist!!! \n\n\n");
            else
                WriteLine("Game Ended! You were beaten by computer! You sucks! \n\n\n"); 
        }

        internal static void PrintRules(int STARTING_FLAGS, int WINNING_CONDITION, int[] FlagsToRemove)
        {
            Console.WriteLine("-----------------------------RULES-----------------------------");
            Console.WriteLine("Game starts by having {0} flags", STARTING_FLAGS);
            Console.WriteLine("Game will ends if remain {0} flag/s or less, player who have {0} flag/s or less during his/her turn consider lose ", WINNING_CONDITION);
            Console.WriteLine("The number of flags that can and have to be removed per turn are: ");
            Console.Write(string.Join(", ", FlagsToRemove.Select(i => i)));
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Game start, remaining flag: {0}", STARTING_FLAGS);
        }

        class ResultTable
        {
            public int Turn { get; set; }
            public string Move { get; set; }
            public int Pulled { get; set; }
            public int Remaining { get; set; }
        }

        static void DisplayResultTable(List<ResultTable> resultTables)
        {
            var table = new ConsoleTable("Turn", "Move", "Flag/s Pulled", "Flag/s Remaining");
            foreach (ResultTable resultTable in resultTables)
            {
                Clear();
                table.AddRow(resultTable.Turn, resultTable.Move, resultTable.Pulled, resultTable.Remaining);
                WriteLine(table.ToStringAlternative());
                WriteLine("{0} has pulled {1} {2}, remaining {3} {4} ", resultTable.Turn % 2 == 0 ? "Computer AI " : "You",
                           resultTable.Pulled, resultTable.Pulled > 1 ? "flags" : "flag", resultTable.Remaining, resultTable.Remaining > 1 ? "flags" : "flag");
            }
        }

        static (int, int) ProcessDecision(int remainingFlags, int turn, int HumanPulledFlag, FlagRemovalBrain brain, List<ResultTable> resultTables, bool isHumanTurn)
        {
            if (isHumanTurn)
            {
                Thread.Sleep(1000);//pretending that the computer is taking time to think
                WriteLine("Computer AI's turn, computer is thinking...");
                WriteLine();
            }

            ResultTable resultTable = new ResultTable
            {
                Turn = ++turn,
                Move = turn % 2 == 0 ? "AI " : "You",
                Pulled = isHumanTurn ? HumanPulledFlag : brain.ComputerDecision(remainingFlags)
            };
            resultTable.Remaining = remainingFlags - resultTable.Pulled;

            resultTables.Add(resultTable);
            DisplayResultTable(resultTables);
            return (resultTable.Remaining, resultTable.Turn);
        }
    }
}
