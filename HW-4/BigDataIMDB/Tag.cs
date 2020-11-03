using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public struct Tag
    {
        private string Name;
        public Dictionary<Movie, float> MoviesWithScores ;

        public Tag(string name)
        {
            Name = name;
            MoviesWithScores = new Dictionary<Movie, float>();
        }
    }
}
