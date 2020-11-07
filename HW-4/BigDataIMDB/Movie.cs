using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public struct Movie
    {
        public string MovieTitle;
        public HashSet<Staff> Staff;
        public HashSet<Tag> Tags;
        public float AverageRating;
        public float WeightedRating;
        private string Language;

        public Movie(string movieTitle, string language)
        {
            MovieTitle = movieTitle;
            Staff = new HashSet<Staff>();
            Tags = new HashSet<Tag>();
            AverageRating = 0;
            WeightedRating = 0;
            Language = language;
        }
    }
}
