using System;
using System.Collections.Generic;
using System.Linq;

namespace FlagRemovalwithAI
{

    public class FlagRemovalBrain
    {
        public FlagRemovalBrain(int WinningCondition, int StartingFlags, List<int> _FlagsThatCanBeRemove)
        {
            FlagsThatCanBeRemove = _FlagsThatCanBeRemove;
            (LosingStateFlagsNum, UpperHandIfStartFirst) = GenerateWiningStrategy(WinningCondition, StartingFlags, _FlagsThatCanBeRemove);

        }

        public List<int> FlagsThatCanBeRemove { get; set; }

        public List<int> LosingStateFlagsNum { get; set; }

        public bool UpperHandIfStartFirst { get; set; }

        public override string ToString()
        {
            string str = "LosingStateFlagsNum: " + Environment.NewLine;
            string upperHand = UpperHandIfStartFirst ? "First" : "Later";
            str += string.Join(", ", LosingStateFlagsNum.Select(i => i)) + Environment.NewLine;
            str += "Upper Hand if start: " + upperHand + Environment.NewLine;


            return str;
        }


        public  (List<int>, bool) GenerateWiningStrategy(int WinningCondition, int StartingFlags, List<int> FlagsThatCanBeRemove)
        {
            List<int> LosingStateFlagsNum = new List<int>();

            for (int i = WinningCondition; i <= StartingFlags; i++)
            {
                bool isLosingFlagStage = true;
                foreach (int item in LosingStateFlagsNum)
                {
                    int num = i - item;

                    if (FlagsThatCanBeRemove.Contains(num))
                    {
                        isLosingFlagStage = false;
                        break;
                    }
                }

                if (isLosingFlagStage)
                    LosingStateFlagsNum.Add(i);
            }

            bool upperHandIfStartFirst = LosingStateFlagsNum.Max() != StartingFlags;
            return (LosingStateFlagsNum, upperHandIfStartFirst);
        }

        public int PulledFlagDecision(int remainingFlags)
        {  
            int pulledFlag = -1;
            var remainingLosingStateFlagsNum = LosingStateFlagsNum.Where(x => x < remainingFlags).ToList();

            foreach (int item in remainingLosingStateFlagsNum)
            {
                int num = remainingFlags - item;
                if (FlagsThatCanBeRemove.Contains(num))
                {
                    pulledFlag = num;
                    break;
                }

            }

            if (pulledFlag == -1)
            {
                Random random = new Random();
                int index = random.Next(FlagsThatCanBeRemove.Count);
                pulledFlag = FlagsThatCanBeRemove[index];
            }

            return pulledFlag;

        }
    }
}
