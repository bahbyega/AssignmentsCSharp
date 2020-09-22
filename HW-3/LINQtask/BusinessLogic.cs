using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQtask
{
    class BusinessLogic
    {
        private List<User> users = new List<User>();
        private List<Record> records = new List<Record>();

        public BusinessLogic()
        {
            User userSteve = new User(1, "Steve", "Jobs");
            User userBill = new User(2, "Bill", "Gates");
            User userBob = new User(3, "Bob", "TheBuilder");
            User anotherBob = new User(3, "Bob", "TheBuilder"); // we create the same instances for testing
            User[] users = new User[] { userSteve, userBill, userBob, anotherBob };
            this.users = users.ToList();

            Record recordSteve = new Record(userSteve, null);
            Record recordBill = new Record(userBill, "RecordBill");
            Record recordBob = new Record(userBob, "RecordBob");
            Record recordAnotherBob = new Record(anotherBob, "RecordBob");
            Record[] records = new Record[] { recordSteve, recordBill, recordBob, recordAnotherBob };
            this.records = records.ToList();
        }

        public List<User> GetUsersBySurname(String surname)
        {
            var usersToFind = (from user in users
                               where String.Equals(user.Surname, surname)
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
            var uniqueNames = (from user in users
                              select user.Name + " " + user.Surname)
                              .Distinct(StringComparer.OrdinalIgnoreCase);
            return uniqueNames.ToList();
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
            return usersSorted.ToList();
        }

        public List<User> GetDescendingOrderedUsers()
        {
            var usersSorted = users.OrderByDescending(user => user.ID);
            return usersSorted.ToList();
        }

        public List<User> GetReversedUsers()
        {
            var usersReversed = (from user in users
                                 select user).Reverse();
            return usersReversed.ToList();
        }

        public List<User> GetUsersPage(int pageSize, int pageIndex)
        {
            var usersPage = users.Skip(pageSize).Take(pageIndex);
            return usersPage.ToList();
        }
    }
}
