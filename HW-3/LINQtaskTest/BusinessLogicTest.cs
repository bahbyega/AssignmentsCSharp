using System.Linq;
using LINQtask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQtaskTest
{
    [TestClass]
    public class BusinessLogicTest
    {            
        private BusinessLogic busLogic = new BusinessLogic();

        [TestMethod]
        public void GetUsersBySurname()
        {
            Assert.AreEqual("Jobs", busLogic.GetUsersBySurname("Jobs")[0].Surname);
        }
        [TestMethod]
        public void GetUsersBySurname_TrivialCase()
        {
            Assert.AreEqual(0, busLogic.GetUsersBySurname("").Count);
        }
        [TestMethod]
        public void GetUserByID()
        {
            Assert.AreEqual("Steve", busLogic.GetUserByID(1).Name);
        }
        [TestMethod]
        public void GetUserByID_WhenTwoUsersHaveTheSameId_ThrowInvalidOperationException()
        {
            Assert.ThrowsException<System.InvalidOperationException>(
                () => busLogic.GetUserByID(3));
        }
        [TestMethod]
        public void GetUsersBySubstring()
        {
            Assert.AreEqual(2, busLogic.GetUsersBySubstring("Builder").Count);
        }
        [TestMethod]
        public void GetUsersBySubstring_EmptyStringScenario()
        {
            Assert.AreEqual(4, busLogic.GetUsersBySubstring("").Count);
        }
        [TestMethod]
        public void GetAllUniqueNames()
        {
            Assert.AreEqual(3, busLogic.GetAllUniqueNames().Count);
        }
        [TestMethod]
        public void GetAllAuthors()
        {
            Assert.AreEqual(3, busLogic.GetAllAuthors().Count);
        }
        [TestMethod]
        public void GetUsersDictionary()
        {
            Assert.AreEqual("Steve", busLogic.GetUsersDictionary()[1].Name);
            Assert.AreEqual("Bill", busLogic.GetUsersDictionary()[2].Name);
            Assert.AreEqual("Bob", busLogic.GetUsersDictionary()[3].Name);
        }
        [TestMethod]
        public void GetMaxID()
        {
            int expected = 3;
            Assert.AreEqual(expected, busLogic.GetMaxID());
        }
        [TestMethod]
        public void GetOrderedUsers()
        {
            Assert.AreEqual("Steve", busLogic.GetOrderedUsers().First().Name);
            Assert.AreEqual("Bob", busLogic.GetOrderedUsers().Last().Name);
        }
        [TestMethod]
        public void GetDescendingOrderedUsers()
        {
            Assert.AreEqual("Bob", busLogic.GetDescendingOrderedUsers().First().Name);
            Assert.AreEqual("Steve", busLogic.GetDescendingOrderedUsers().Last().Name);
        }
        [TestMethod]
        public void GetReversedUsers()
        {
            Assert.AreEqual("Bob", busLogic.GetReversedUsers().First().Name);
            Assert.AreEqual("Steve", busLogic.GetReversedUsers().Last().Name);
        }


    }
}
