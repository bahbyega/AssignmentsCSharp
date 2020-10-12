using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Engines;
using BigDataIMDB;

namespace Benchmarks
{
    /// <summary>
    /// Benmcharks for DataParser class
    /// </summary>
    [MemoryDiagnoser]
    [InProcess]
    public class DataParserBenchmarks
    {
        private DataParser dataParser = new DataParser();

        [Benchmark]
        public void Benchmark_of_DataParser_ParseMovieCods()
        {
            dataParser.ParseMovieCodes();
        }

    }
}
