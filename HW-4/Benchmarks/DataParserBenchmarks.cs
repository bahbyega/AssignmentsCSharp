using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Engines;
using BigDataIMDB;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    /// <summary>
    /// Benmcharks for DataParser class
    /// </summary>
    [MemoryDiagnoser]
    [ShortRunJob]
    public class DataParserBenchmarks
    {
        private DataParser dataParser = new DataParser();

        [Benchmark]
        public void Benchmark_of_DataParser_ParseMovieCodes_Linq()
        {
            dataParser.ParseMovieCodesLinq();
        }
        [Benchmark]
        public void Benchmark_of_DataParser_ParseMovieCodes_NoParallel()
        {
            dataParser.ParseMovieCodesNoParallel();
        }
        [Benchmark]
        public void Benchmark_of_DataParser_ParseMovieCodes_BlockColl()
        {
            dataParser.ParseMovieCodes_BlockColl();
        }
        //[Benchmark]
        public void Benchmark_of_DataParser_ParseMovieCodes_Split()
        {
            dataParser.ParseMovieCodes_Split();
        }
        //[Benchmark]
        public void Benchmark_of_DataParser_ParseActorsDirectorsNames()
        {
            dataParser.ParseActorsDirectorsNames();
        }

    }
}
