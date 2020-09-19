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
            const int OBJECTS_COUNT = 30;
            const int OBJECT_WEIGHT = 100000000;

            for (int i = 1; i <= OBJECTS_COUNT; i++)
            {
                new Garbage(i, OBJECT_WEIGHT);
            }

        }

        static void Main(string[] args)
        {
            CreateLotsOfStuff();
            Console.ReadKey();
        }

    }


}
