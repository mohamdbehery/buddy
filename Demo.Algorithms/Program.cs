using System;
using System.Collections.Generic;

namespace Demo.Algorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            //int[] given_array = new[] { 1, 4, 14, 18, 3, 2 };
            //var sorted_array = QuickSort(given_array, 0, given_array.Length);
            //foreach (var item in sorted_array)
            //{
            //    Console.Write($" > {item}");
            //}
            List<int> df = new List<int>()
            {
                1, 2
            };
            
            Console.WriteLine(df[2]);
            Console.ReadLine();
        }

        public int Factorial(int n)
        {
            if (n >= 1) { return n * Factorial(n - 1); }
            else { return 1; }
        }

        public int Fibonacci(int n)
        {
            if (n >= 3) { return Fibonacci(n - 1) + Fibonacci(n + 2); }
            else { return 1; }
        }

        public static int BinarySearch(int[] source, int target)
        {
            int left = 0, middle = 0;
            int right = source.Length - 1;
            while (left < right)
            {
                middle = (left + right) / 2;
                if (source[middle] == target)
                    return middle;
                else if (target < source[middle])
                    right = middle - 1;
                else if (target > source[middle])
                    left = middle + 1;
            }
            return middle;
        }

        # region Quick Sort
        public static int[] QuickSort(int[] given_array, int start, int end)
        {
            if (start < end)
            {
                int pivotIndex = QuickSortPartition(given_array, start, end);
                QuickSort(given_array, start, pivotIndex);
                QuickSort(given_array, pivotIndex + 1, end);
            }
            return given_array;
        }

        public static int QuickSortPartition(int[] given_array, int left, int right)
        {
            int pivot = given_array[left];
            int swapIndex = left;
            for (int i = left + 1; i < right; i++)
            {
                if (given_array[i] < pivot)
                {
                    swapIndex++;
                    SwapArray(ref given_array, i, swapIndex); // sort these 2 elements
                }
            }
            SwapArray(ref given_array, left, swapIndex); // move pivot to its sorted location
            return swapIndex;
        }

        public static void SwapArray(ref int[] given_array, int firstIndex, int secondIndex)
        {
            int temp = given_array[firstIndex];
            given_array[firstIndex] = given_array[secondIndex];
            given_array[secondIndex] = temp;
        }
        #endregion

    }
}
