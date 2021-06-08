using System;
using System.Collections.Generic;

namespace Demo.Algorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            #region quick sort
            //int[] given_array = new[] { 1, 4, 14, 18, 3, 2 };
            //int[] sorted_array = QuickSort(given_array, 0, given_array.Length);
            //foreach (var item in sorted_array)
            //{
            //    Console.Write($" > {item}");
            //}
            #endregion
            #region merge sort
            //int[] given_array = new[] { 1, 4, 14, 18, 3, 2 };
            //int[] sorted_array = MergeSort(given_array, 0, given_array.Length - 1);
            //foreach (var item in sorted_array)
            //{
            //    Console.Write($" > {item}");
            //}
            #endregion
            Console.WriteLine();
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

        #region Quick Sort
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

        #region Merge Sort
        public static void Merge(int[] arr, int left, int middle, int right)
        {
            int leftArrayLength = middle - left + 1;
            int rightArrayLength = right - middle;

            // Devide array 
            int[] leftArray = new int[leftArrayLength], rightArray = new int[rightArrayLength];
            for (int x = 0; x < leftArrayLength; x++)
                leftArray[x] = arr[left + x];
            for (int y = 0; y < rightArrayLength; y++)
                rightArray[y] = arr[middle + 1 + y];

            // Maintain current index of sub-arrays and main array
            int i, j, k;
            i = 0;
            j = 0;
            k = left;

            // Until we reach either end of either L or M, pick larger among
            // elements L and M and place them in the correct position at A[p..r]
            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (leftArray[i] <= rightArray[j])
                {
                    arr[k] = leftArray[i];
                    i++;
                }
                else
                {
                    arr[k] = rightArray[j];
                    j++;
                }
                k++;
            }

            // When we run out of elements in either L or M,
            // pick up the remaining elements and put in A[p..r]
            while (i < leftArrayLength)
            {
                arr[k] = leftArray[i];
                i++;
                k++;
            }

            while (j < rightArrayLength)
            {
                arr[k] = rightArray[j];
                j++;
                k++;
            }
        }

        public static int[] MergeSort(int[] given_array, int left, int right)
        {
            if (left >= right)
            {
                return given_array;
            }
            int middle = left + (right - left) / 2;
            MergeSort(given_array, left, middle);
            MergeSort(given_array, middle + 1, right);
            Merge(given_array, left, middle, right); // merge two subarrays L and M into arr

            return given_array;
        }
        #endregion
    }
}
