using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

//delegate void displayResultDel(List<ResultTable> resultTables);

namespace FlagsRemoval2
{


    delegate int RemoveAIFlagDel(int remainingflags);
    class Program
    {
        const int TOTAL_FLAGS = 21;

        static void Main(string[] args)
        {
            List<int> validGameSelectionInput = new List<int> { 1, 2, 3, 99 };

            string input;
            int game;
            
            RemoveAIFlagDel removeFlagAIdeleg = null;
            do
            {
                bool humanWon = false;
                DisplayGameMenu();
                input = ReadLine();
                if (!int.TryParse(input, out game) || !validGameSelectionInput.Contains(game))
                {
                   

                    WriteLine("Please enter number 1-3 or 99 only");
                    continue;
                }
                if (game == 99) { break; }//quit the game
                int remainingFlags = TOTAL_FLAGS;
                int gameEndFlag = 0;//game will ends if the remaining flag equal or less than this amount

                #region Select game region, assign delegate at here
                switch (game)//each game have different "game ended flag" amount
                {
                    case 1:
                        gameEndFlag = 1;
                        removeFlagAIdeleg = LastFlagLoss;//assign LastFlagLossLastFlagLoss function to delegate
                        break;
                    case 2:
                        gameEndFlag = 0;
                        removeFlagAIdeleg = NoFlagLoss;//assign NoFlagLoss function to delegate
                        break;
                    case 3:
                        gameEndFlag = 5;
                        removeFlagAIdeleg = Last5FlagLoss;//assign Last5FlagLoss function to delegate
                        break;
                    default:
                        break;
                }
                #endregion

                string inputPulledFlag;
                List<ResultTable> ListOfResultTable = new List<ResultTable>();//keep track the result of each turn
                int turn = 0;
                do
                {

                    List<int> validPulledFlagsInput = new List<int> { 1, 2, 3 };//valid flag amount that can be pulled
                    WriteLine("Your turn, enter the number of flag(s) that you willing to pull (1-3 flags only).");
                    inputPulledFlag = ReadLine();

                    if (!int.TryParse(inputPulledFlag, out int pulledFlag) || !validPulledFlagsInput.Contains(pulledFlag))
                    {
                        WriteLine("Please enter number (1-3) only");
                        continue;
                    }

                    //User's turn
                    ResultTable userResultTable = new ResultTable();
                    userResultTable = ProcessHumanDecision(remainingFlags, turn, pulledFlag, ListOfResultTable);
                    ListOfResultTable.Add(userResultTable);
                    DisplayResultTable(ListOfResultTable);

                    remainingFlags = userResultTable.Remaining;
                    turn = userResultTable.Turn;
                    if (remainingFlags <= gameEndFlag) { humanWon = true; break; }//break if human player reach winning condition



                    //Computer AI's turn
                    ResultTable ComputerAIResultTable = new ResultTable();
                    ComputerAIResultTable = ProcessComputerDecision(remainingFlags, turn, removeFlagAIdeleg, ListOfResultTable);
                    ListOfResultTable.Add(ComputerAIResultTable);
                    DisplayResultTable(ListOfResultTable);

                    remainingFlags = ComputerAIResultTable.Remaining;
                    turn = ComputerAIResultTable.Turn;

                    if (remainingFlags <= gameEndFlag) { break; }//break if computer AI reach winning condition


                } while (remainingFlags > gameEndFlag);


                if (humanWon)
                {
                    WriteLine("Game Ended! Good Job, you beat the computer! You are a good strategist!!!");
                    WriteLine();
                }
                else
                {
                    WriteLine("Game Ended! You were beaten by computer! You sucks!");
                    WriteLine();
                }
                WriteLine("Play Again?");
            } while (game != 99 || !int.TryParse(input, out _));

            ReadLine();
        }

        

        static ResultTable ProcessComputerDecision(int remainingFlags, int turn, RemoveAIFlagDel aiRemoveFlagDel, List<ResultTable> resultTables )
        {
            WriteLine("Computer AI's turn, computer is thinking...");
            WriteLine();
            Thread.Sleep(1000);//pretending that the computer is taking time to think

            turn++;

            ResultTable computerAIResultTable = new ResultTable();//assign computer AI's turn result to result table 

            computerAIResultTable.Turn = turn;
            computerAIResultTable.Move = turn % 2 == 0 ? "AI " : "You"; 
            computerAIResultTable.Pulled = aiRemoveFlagDel(remainingFlags); 
            computerAIResultTable.Remaining = remainingFlags - computerAIResultTable.Pulled;

          
            return computerAIResultTable;
        }

        static  ResultTable ProcessHumanDecision(int remainingFlags, int turn, int pulledFlag, List<ResultTable> resultTables)
        {
          
            ResultTable userResultTable = new ResultTable//assign user's turn result to result table 
            {
                Turn = ++turn,
                Move = turn % 2 == 0 ? "AI " : "You",
                Pulled = pulledFlag
            };
            userResultTable.Remaining = remainingFlags - userResultTable.Pulled;



            return userResultTable;
        }


        class ResultTable
        {
            public int Turn { get; set; }
            public string Move { get; set; }
            public int Pulled { get; set; }
            public int Remaining { get; set; }
        }
        static void DisplayGameMenu()
        {
            WriteLine("Select your game: ");
            WriteLine("--------------------------------------------------------------------------------");
            WriteLine("Press 1 for 21 flags game. Remove 1-3 flags each turn, LAST flag remaining loss the game: ");
            WriteLine("Press 2 for 21 flags game. Remove 1-3 flags each turn, NO flag remaining loss the game: ");
            WriteLine("Press 3 for 21 flags game. Remove 1-3 flags each turn, 5 or LESS flag remaining loss the game: ");
            WriteLine("Press 99 to exit the game");
            WriteLine("--------------------------------------------------------------------------------");

        }


        static void DisplayResultTable(List<ResultTable> resultTables)
        {
            var table = new ConsoleTable("Turn", "Move", "Pulled", "Remaining");

            string By = string.Empty;
            foreach (ResultTable resultTable in resultTables)
            {

                Clear();

             
                table.AddRow(resultTable.Turn, resultTable.Move, resultTable.Pulled, resultTable.Remaining);

                WriteLine(table.ToStringAlternative());
                WriteLine("{0} has pulled {1} {2}, remaining {3} {4} ", resultTable.Turn % 2 == 0 ? "Computer AI " : "You", 
                           resultTable.Pulled, resultTable.Pulled > 1 ? "flags" : "flag", resultTable.Remaining,  resultTable.Remaining > 1 ? "flags" : "flag");
            }

          
        }

        static int LastFlagLoss(int remainingFlags)
        {
            List<int> LosingFlags = new List<int>() { 21, 17, 13, 9, 5, 1 };
            Random random = new Random();

            foreach (int i in LosingFlags)
                if (remainingFlags > i)
                    return remainingFlags - i <= 3 ? remainingFlags - i : random.Next(1, 3);

            return random.Next(1, 3);//AI randomly return a value if unable to left the losing flag to user

        }

        static int NoFlagLoss(int remainingFlags)
        {
            List<int> LosingFlags = new List<int>() { 20, 16, 12, 8, 4, 0 };
            Random random = new Random();

            foreach (int i in LosingFlags)
                if (remainingFlags > i)
                    return remainingFlags - i <= 3 ? remainingFlags - i : random.Next(1, 3);

            return random.Next(1, 3);//AI randomly return a value if unable to left the losing flag to user

        }

        static int Last5FlagLoss(int remainingFlags)
        {
            List<int> LosingFlags = new List<int>() { 17, 13, 9, 5 };
            Random random = new Random();

            foreach (int i in LosingFlags)
                if (remainingFlags > i)
                    return remainingFlags - i <= 3 ? remainingFlags - i : random.Next(1, 3);

            return random.Next(1, 3);//AI randomly return a value if unable to left the losing flag to user

        }

        public static void ClearCurrentConsoleLine()
        {
            //clean console code
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
