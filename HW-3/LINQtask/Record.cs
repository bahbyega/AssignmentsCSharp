using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQtask
{
    class Record
    {
        public User Author { get; set; }
        public String Message { get; set; }
        public Record(User author, String message)
        {
            this.Author = author;
            this.Message = message;
        }
    }

}
