using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckCombination
{
    class Program
    {
       static List<int> list = new List<int>() {2,5,7 };
        static int targetNum = 8;
        static void Main(string[] args)
        {
            List<List<int>> ListOfArrays = new List<List<int>>();
            List<int> afterAdjustment = new List<int>();
            int c = 0;
            foreach (IEnumerable<int> nums in Combinations(targetNum))
            {
                List<int> arr = new List<int>();
                foreach (var item in nums)
                {
                    arr.Add(item);
                }
                
                c++; 

                ListOfArrays.Add(arr); 
            }
            Console.WriteLine("Target Sum Number: {0}", targetNum);
            Console.WriteLine("Total permutation: {0}", c);
            PrintPermutation(ListOfArrays);

           

            Console.ReadLine();
        }

       static void PrintPermutation(List<List<int>> ListOfArrays)
        {
            foreach (List<int> arrays in ListOfArrays)
            {
                foreach (int num in arrays)
                {
                    Console.Write(num + ", ");
                }
                Console.WriteLine();
            }
        }

        static IEnumerable<List<int>> Combinations(int n)
        {
         
            if (n < 0)
            {
                yield break;
            }
            else if (n == 0)
            {
                yield return new List<int>();
            }

            foreach (int x in list)
            {
                foreach (IEnumerable<int> combi in Combinations(n - x))
                {
                    var result = new List<int>() { x };
                    result.AddRange(combi);
                    yield return result;
                }

            }
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
