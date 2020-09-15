using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCollectionTests 
{
    class Garbage 
    {
        private int id;

        private bool someBoolean;
        private int someInt;
        private float someFloat;
        private double someDouble;
        private double[] arrayOfDoubles;

        public Garbage(int id, int length)
        {
            this.id = id;
            someBoolean = false;
            someInt = 0;
            someFloat = (float)2.718281828;
            someDouble = (double)3.1415926535;

            // create smth heavy weighted
            arrayOfDoubles = new double[length];
        }

        ~Garbage()
        {
            Console.WriteLine("Garbage collector working on: {0}th object", id);
        }


    }
}
