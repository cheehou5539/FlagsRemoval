using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckCombination
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<int>> ListOfArrays = new List<List<int>>();
            int c = 0;
            foreach (IEnumerable<int> nums in Combinations(20))
            {
                List<int> arr = new List<int>();
                foreach (int i in nums)
                {
                    arr.Add(i);
                    Console.Write(i + ", ");

                }
                c++;
                arr.Sort();
                ListOfArrays.Add(arr);

                Console.WriteLine();
            }
            SelectPulledFlag(ListOfArrays);
            //  Console.WriteLine(c);
            Console.ReadLine();
        }
        static int SelectPulledFlag(List<List<int>> ListOfArrays)
        {
            ListOfArrays.Sort((a, b) => a.Count - b.Count);

            foreach (List<int> list in ListOfArrays)
                if (list.Count() % 2 != 0)
                    return list[list.Count - 1];

            return -1;
        }

        static IEnumerable<List<int>> Combinations(int n)
        {
            List<int> list = new List<int>() { 3, 6, 7 };
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
    }
}
