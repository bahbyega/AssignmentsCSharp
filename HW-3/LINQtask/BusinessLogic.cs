using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQtask
{
    class BusinessLogic
    {
        private List<User> users = new List<User>();
        private List<Record> records = new List<Record>();

        public BusinessLogic()
        {
            // Writing unit tests, so no reason to keep these 
            /*User user1 = new User(1, "first", "surname");
            User user2 = new User(2, "second", "surname");
            User user3 = new User(3, "third", "surname");
            User user4 = new User(4, "fourth", "surFourth");
            User user5 = new User(1, "fifth", "surFifth");
            users.Add(user1);
            users.Add(user2);
            users.Add(user3);
            users.Add(user4);
            users.Add(user5);

            Record record1 = new Record(user1, "Record1");
            Record record2 = new Record(user2, "Record2");
            Record record3 = new Record(user3, "Record3");
            Record record4 = new Record(user4, "Record4");
            Record record5 = new Record(user5, "Record5");

            records.Add(record1);
            records.Add(record2);
            records.Add(record3);
            records.Add(record4);
            records.Add(record5);*/
        }
        public BusinessLogic(IEnumerable<User> users)
        {
            this.users = users.ToList();
        }

        public List<User> GetUsersBySurname(String surname)
        {
            var usersToFind = (from user in users
                               where user.Surname == surname
                               select user).ToList();
            return usersToFind;
        }

        public User GetUserByID(int id)
        {
            var userToFind = (from user in users
                              where user.ID == id
                              select user).ToList();

            // will throw expception if multiple users found
            User oneFoundUser = userToFind.Single();
            return oneFoundUser;
        }

        public List<User> GetUsersBySubstring(String substring)
        {
            var usersToFind = (from user in users
                               where (user.Name.Contains(substring) ||
                                      user.Surname.Contains(substring))
                               select user).ToList();
            return usersToFind;
        }

        public List<String> GetAllUniqueNames()
        {
            var uniqueNames = from user in users
                              select user.Name + " " + user.Surname;
            uniqueNames.Distinct();
            return (List<string>)uniqueNames; // cast from IEnumerable to List
        }

        public List<User> GetAllAuthors()
        {
            var authors = (from record in records
                           where record.Message != null
                           select record.Author).Distinct().ToList();
            return authors;
        }

        public Dictionary<int, User> GetUsersDictionary()
        {
            var map = users.GroupBy(user => user.ID) // groups users by key property
                           .ToDictionary(user => user.Key, user => user.First()); // key is IGrouping property
            return map;
        }

        public int GetMaxID()
        {
            int maxID = (from user in users
                         select user.ID).Max();
            return maxID;
        }

        public List<User> GetOrderedUsers()
        {
            var usersSorted = users.OrderBy(user => user.ID);
            return (List<User>)usersSorted;
        }

        public List<User> GetDescendingOrderedUsers()
        {
            var usersSorted = users.OrderByDescending(user => user.ID);
            return (List<User>)usersSorted;
        }

        public List<User> GetReversedUsers()
        {
            var usersReversed = (from user in users
                                 select user).Reverse();
            return (List<User>)usersReversed;
        }

        public List<User> GetUsersPage(int pageSize, int pageIndex)
        {
            var usersPage = users.Skip(pageSize).Take(pageIndex);
            return (List<User>)usersPage;
        }
    }
}
