using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public class Staff
    {
        public string Name;
        public HashSet<Movie> isActor; // films in which staff is an actor
        public HashSet<Movie> isDirector; // films in whick staff is director

        // would be better to remove this mess with constructors (nocheckin)
        public Staff() 
        {
            isActor = new HashSet<Movie>();
            isDirector = new HashSet<Movie>();
        }
        public Staff(string name)
        {
            Name = name;
            isActor = new HashSet<Movie>();
            isDirector = new HashSet<Movie>();
        }
    }
}
