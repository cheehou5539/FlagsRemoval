using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

namespace FlagsRemoval1
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalFlags = 21;
            WriteLine("Game start by having {0} flags", totalFlags);

            do
            {

                string input;
                int pulledFlag;
                do
                {
                    WriteLine("Remaining Flags: {0}", totalFlags);
                    WriteLine("Your turn, enter the number of flag(s) that you willing to pull (1-3 flags only).");
                    //
                    input = ReadLine();



                    if (!int.TryParse(input, out pulledFlag))
                    {
                        WriteLine("Please enter number (1-3) only");
                    }

                }
                while (pulledFlag > 3 || pulledFlag <= 0 || !int.TryParse(input, out pulledFlag));

                string flagspulled = pulledFlag == 1 ? "flag" : "flags";


                totalFlags -= pulledFlag;
                string totalflagsremaining = totalFlags == 1 ? "flag" : "flags";
                WriteLine("You have pulled {0} {1}, remaining {2} {3}", pulledFlag, flagspulled, totalFlags, totalflagsremaining);


                if (totalFlags == 1)
                {
                    WriteLine("Last flag remain for computer, you won!");
                    break;
                }


                WriteLine("Computer's turn, computer is thinking...");
                WriteLine();
                Thread.Sleep(1000);

                int computerPulled = AIPulledFlag(totalFlags);
                totalFlags -= computerPulled;

                flagspulled = computerPulled == 1 ? "flag" : "flags";
                totalflagsremaining = totalFlags == 1 ? "flag" : "flags";

                WriteLine("Computer has make its decision, computer has pulled {0} {1}, " +
                    "remaining {2} {3} ", computerPulled, flagspulled, totalFlags, totalflagsremaining);
                WriteLine();
                if (totalFlags == 1)
                {
                    WriteLine("Computer has left the last flag for you, you lose!");
                }
            }
            while (totalFlags > 1);

            ReadLine();
        }



                  

        private static int AIPulledFlag(int remainingFlags)
        {
            List<int> WinningFlags = new List<int>() { 21, 17, 13, 9, 5, 1 };
            Random random = new Random();

            foreach (int i in WinningFlags)
                if (remainingFlags > i)
                    return remainingFlags - i <= 3 ? remainingFlags - i : random.Next(1, 3);

            return random.Next(1, 3);
        }
    }
}
