﻿using BenchmarkDotNet.Running;

namespace CarvedRock.Benchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<JsonComparisons>();
        }
    }
}