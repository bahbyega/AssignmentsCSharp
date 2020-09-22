using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCollectionTests 
{
    class Garbage 
    {
        // number of the object created
        private int id;

        // a flag to check if we write the garbage collection timing
        private static bool collectionStarted = false;

        // heavy weighted field serving as garbage
        private double[] arrayOfDoubles;

        // time fields to benchmark garbage collection
        private static DateTime startTime;
        private static DateTime endTime;

        public Garbage(int id, int length)
        {
            this.id = id;
            Console.WriteLine("Creating {0}th object", id);

            // initialize smth heavy weighted
            arrayOfDoubles = new double[length];

            if (collectionStarted)
            {
                    endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                        DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                    TimeSpan elapsedTime = endTime - startTime;

                    Console.WriteLine("\t" + elapsedTime + "\n");
                    collectionStarted = false;
            }
        }

        ~Garbage()
        {
            collectionStarted = true;
            Console.WriteLine("\tDeleting {0}th object", id);

            startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            
        }

    }
}
