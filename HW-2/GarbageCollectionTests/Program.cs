using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


namespace GarbageCollectionTests
{
    class Program
    {
        public static void CreateLotsOfStuff()
        {
            const int LENGTH = 7000;
            const int OBJECTWEIGHT = 100;

            for (int i = 1; i <= LENGTH; i++)
            {
                new Garbage(i, OBJECTWEIGHT);
            }

        }
        public static void DoSomeWork()
        {
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine("Doing the work {0}", i);
            }
        }

        static void Main(string[] args)
        {
            CreateLotsOfStuff();
            DoSomeWork();

            // Uncomment next line to benchmark the code
            //BenchmarkRunner.Run<NameParserBenchmarks>();

            Console.ReadKey();
        }

    }


}
