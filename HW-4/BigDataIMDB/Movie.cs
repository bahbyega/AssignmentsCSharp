using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public struct Movie
    {
        private string MovieTitle;
        public HashSet<Staff> Staff;
        public HashSet<Tag> Tags;
        private float AverageRating;
        private string Language;

        public Movie(string movieTitle, string language)
        {
            MovieTitle = movieTitle;
            Language = language;
            Staff = new HashSet<Staff>();
            Tags = new HashSet<Tag>();
            AverageRating = 0;
        }
    }
}
