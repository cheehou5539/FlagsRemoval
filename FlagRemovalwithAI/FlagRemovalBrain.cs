using System;
using System.Collections.Generic;
using System.Linq;

namespace FlagRemovalwithAI
{

    public class FlagRemovalBrain
    {
        public FlagRemovalBrain(int _WinningCondition, int StartingFlags, List<int> _FlagsThatCanBeRemove)
        {

            FlagsThatCanBeRemove = _FlagsThatCanBeRemove;
            WinningCondition = _WinningCondition;
            (LosingStateFlagsNum, UpperHandIfStartFirst) = GenerateWiningStrategy(_WinningCondition, StartingFlags, _FlagsThatCanBeRemove);
            IsConsecutive = !_FlagsThatCanBeRemove.Select((i, j) => i - j).Distinct().Skip(1).Any();

        }

        public List<int> FlagsThatCanBeRemove { get; set; }

        public List<int> LosingStateFlagsNum { get; set; }

        public bool UpperHandIfStartFirst { get; set; }

        public int WinningCondition { get; set; }

        public bool IsConsecutive { get; set; }

        public override string ToString()
        {
            string str = "LosingStateFlagsNum: " + Environment.NewLine;
            string upperHand = UpperHandIfStartFirst ? "First" : "Later";
            str += string.Join(", ", LosingStateFlagsNum.Select(i => i)) + Environment.NewLine;
            str += "Upper Hand if start: " + upperHand + Environment.NewLine;

            return str;
        }


        public (List<int>, bool) GenerateWiningStrategy(int WinningCondition, int StartingFlags, IEnumerable<int> FlagsThatCanBeRemove)
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



        public int ComputerDecision(int remainingFlags)
        {
            if (IsConsecutive)
                return PulledFlagDecisionConsecutive(remainingFlags);

            List<List<int>> ListOfPaths = GenarateAllPaths(remainingFlags - WinningCondition);
            return PulledFlagDecisionNonConsecutive(ListOfPaths);

        }


        public int PulledFlagDecisionNonConsecutive(List<List<int>> ListOfArrays)
        {
            ListOfArrays.Sort((a, b) => a.Count - b.Count);

            foreach (List<int> list in ListOfArrays)
                if (list.Count() % 2 != 0)
                    return list[list.Count - 1];


            //randomly pulled flag if solution not found
            Random random = new Random();
            int index = random.Next(FlagsThatCanBeRemove.Count);
            return FlagsThatCanBeRemove[index];

        }


        public List<List<int>> GenarateAllPaths(int remainingFlagsToEnd)
        {

            List<List<int>> ListOfArrays = new List<List<int>>();

            foreach (IEnumerable<int> nums in Combinations(remainingFlagsToEnd))
            {
                List<int> arr = new List<int>();
                foreach (int i in nums)
                {
                    arr.Add(i);
                }

                arr.Sort();
                ListOfArrays.Add(arr);
            }

            return ListOfArrays;
        }


        public IEnumerable<List<int>> Combinations(int n)
        {

            if (n < 0)
            {
                yield break;
            }
            else if (n == 0)
            {
                yield return new List<int>();
            }

            foreach (int x in FlagsThatCanBeRemove)
            {
                foreach (IEnumerable<int> combi in Combinations(n - x))
                {
                    var result = new List<int>() { x };
                    result.AddRange(combi);
                    yield return result;
                }

            }
        }


        public int PulledFlagDecisionConsecutive(int remainingFlags)
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
