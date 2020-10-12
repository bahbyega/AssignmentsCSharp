using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BigDataIMDB;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DataParserBenchmarks>();
        }
    }
}
