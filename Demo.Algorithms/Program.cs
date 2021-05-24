using System;

namespace Demo.Algorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
    }
}
