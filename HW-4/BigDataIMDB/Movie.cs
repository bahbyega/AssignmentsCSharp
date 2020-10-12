using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public struct Movie
    {
        private string MovieTitle;
        private HashSet<uint> ActorsIds;
        private uint DirectorId;
        private HashSet<Tag> Tags;
        private float AverageRating;
        private string Language;

        public Movie(string movieTitle, string language)
        {
            MovieTitle = movieTitle;
            Language = language;
            ActorsIds = new HashSet<uint>();
            DirectorId = 0;
            Tags = new HashSet<Tag>();
            AverageRating = 0;
        }
    }
}
