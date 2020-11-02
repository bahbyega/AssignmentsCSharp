using System;

namespace BigDataIMDB
{
    class Program
    {
        static void Main(string[] args)
        {
            DataParser dataParser = new DataParser();
            dataParser.ParseMovieCodesLinq();
            dataParser.ParseActorsDirectorsNames();
            dataParser.ParseActorsDirectorsMovieInfo();
        }
    }
}
