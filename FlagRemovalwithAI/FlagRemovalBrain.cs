using System;
using System.Collections.Generic;
using System.Linq;

namespace FlagRemovalwithAI
{

    public class FlagRemovalBrain
    {
        public FlagRemovalBrain(int _WinningCondition, int StartingFlags, List<int> _FlagsThatCanBeRemove)
        {
            IsConsecutive = !_FlagsThatCanBeRemove.Select((i, j) => i - j).Distinct().Skip(1).Any();
            FlagsThatCanBeRemove = _FlagsThatCanBeRemove;
            WinningCondition = _WinningCondition;
            IsCommonMultiple = !IsConsecutive && IfRemoveableContainCommonMultiple(FlagsThatCanBeRemove);
            (LosingStateFlagsNum, UpperHandIfStartFirst) = UpperHandAnalysis(IsConsecutive, StartingFlags);

            Console.WriteLine("Upper hand if {0}", UpperHandIfStartFirst ? "Start First" : "Start Second");
        }

        public List<int> FlagsThatCanBeRemove { get; set; }

        public List<int> LosingStateFlagsNum { get; set; }//for consecutive situation only

        public bool UpperHandIfStartFirst { get; set; }

        public int WinningCondition { get; set; }

        public bool IsConsecutive { get; set; }

        public bool IsCommonMultiple { get; set; }

        public override string ToString()
        {
            string str = "LosingStateFlagsNum: " + Environment.NewLine;
            string upperHand = UpperHandIfStartFirst ? "First" : "Later";
            str += string.Join(", ", LosingStateFlagsNum.Select(i => i)) + Environment.NewLine;
            str += "Upper Hand if start: " + upperHand + Environment.NewLine;

            return str;
        }


        public (List<int>, bool) UpperHandAnalysis(bool IsConsecutive,int StartingFlags)
        {
            if(IsConsecutive)
            {
                return WiningStrategyForConsecutive(WinningCondition, StartingFlags, FlagsThatCanBeRemove);
            }
            else
            {
                return WiningStrategyForNonConsecutive(WinningCondition, StartingFlags);
            }
        }

        public (List<int>, bool) WiningStrategyForConsecutive(int WinningCondition, int StartingFlags, IEnumerable<int> FlagsThatCanBeRemove)
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


        public (List<int>, bool) WiningStrategyForNonConsecutive(int WinningCondition, int StartingFlags)
        {
            ComputerDecision(StartingFlags - WinningCondition, out bool upperHandIfStartFirst);
            return (new List<int>(), upperHandIfStartFirst);
        }

        public int ComputerDecision(int remainingFlags, out bool upperHandIfStartFirst)
        {
            upperHandIfStartFirst = false;
            if (IsConsecutive)
            { 
                return PulledFlagDecisionConsecutive(remainingFlags);
            }
            else
            {
                List<List<int>> ListOfPaths = GenarateAllPaths(remainingFlags - WinningCondition);
                return PulledFlagDecisionNonConsecutive(ListOfPaths, out upperHandIfStartFirst);
            } 
        }


        public int PulledFlagDecisionNonConsecutive(List<List<int>> ListOfArrays, out bool upperhandStartFirst)
        {
            //ListOfArrays.Sort((a, b) => a.Count - b.Count);

            List<int> WinningSelection = new List<int>();
            List<int> LosingSelection = new List<int>();

            foreach (List<int> list in ListOfArrays)
            {
                if (list.Count() % 2 != 0)//get odd Count of list of integer
                {
                    if (!WinningSelection.Contains(list[list.Count - 1]))
                        WinningSelection.Add(list[list.Count - 1]);
                }
                else
                {
                    if (!LosingSelection.Contains(list[list.Count - 1]))
                        LosingSelection.Add(list[list.Count - 1]);
                }


                if (WinningSelection.Count == FlagsThatCanBeRemove.Count && LosingSelection.Count == FlagsThatCanBeRemove.Count)
                    break;
            }


            List<int> uniqueList = WinningSelection.Except(LosingSelection).ToList();//get unique last digit

            if(uniqueList.Count>0)
            {
                upperhandStartFirst = true;
                uniqueList.Sort();
                return uniqueList[uniqueList.Count - 1];
            }
            else
            {
               upperhandStartFirst = false;
               return ForceStaleMate(LosingSelection);
            } 
        }


        public int ForceStaleMate(List<int> LosingSelection )
        {

           List<int> StaleMateList =  FlagsThatCanBeRemove.Except(LosingSelection).ToList();
            if (StaleMateList.Count > 0)
            {
                StaleMateList.Sort();
                return StaleMateList[StaleMateList.Count - 1];
            }
            else
            {  //will lose anyway no matter choose what
                //only hope the opponent make a mistake in their turn
                Random random = new Random();
                return FlagsThatCanBeRemove[random.Next(0, FlagsThatCanBeRemove.Count)]; 
            } 
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


                if (IsCommonMultiple)
                    SumTwoAdjacent(arr, FlagsThatCanBeRemove, out arr);


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
            int pulledFlag = 0;
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

            if (pulledFlag == 0)
            {
                Random random = new Random();
                int index = random.Next(FlagsThatCanBeRemove.Count);
                pulledFlag = FlagsThatCanBeRemove[index];
            }

            return pulledFlag;

        }


        public bool IfRemoveableContainCommonMultiple(List<int> FlagsThatCanBeRemove)
        {
            FlagsThatCanBeRemove.Reverse();
            for (int i = 0; i < FlagsThatCanBeRemove.Count-1; i++) 
                if (FlagsThatCanBeRemove[i] % FlagsThatCanBeRemove[i + 1] == 0) 
                    return true;
              
 
            return false;
        }
        public List<int> SumTwoAdjacent(List<int> arr, List<int> ListToRemoved, out List<int> afterAdjustment)
        {

            bool gotAdjIdentical = false;
            List<int> NewArr = new List<int>();
            for (int i = 0; i < arr.Count; i++)
            {
                if (i < arr.Count - 1 && arr[i] == arr[i + 1] && ListToRemoved.Contains(arr[i] + arr[i + 1]))
                {
                    gotAdjIdentical = true;
                    NewArr.Add(arr[i] + arr[i + 1]);
                    i += 1;
                    continue;

                }
                NewArr.Add(arr[i]); 
            }
            afterAdjustment = NewArr;
            if (gotAdjIdentical)
            {
                SumTwoAdjacent(NewArr, ListToRemoved, out afterAdjustment);
            }
            return afterAdjustment;
        }

    }
}
