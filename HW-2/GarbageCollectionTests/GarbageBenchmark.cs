using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Attributes.Jobs;

namespace GarbageCollectionTests
{
    // runs 3 iterations
    [SimpleJob(RunStrategy.Monitoring, targetCount: 3, invocationCount: 1)]
    public class NameParserBenchmarks
    {
        [Benchmark]
        public void CreateLotsOfStuffBEnch()
        {
            Program.CreateLotsOfStuff();
        }
    }
}
