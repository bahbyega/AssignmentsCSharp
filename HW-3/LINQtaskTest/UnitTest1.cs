using System;
using LINQtask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQtaskTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetUserByID_WhenTwoUsersHaveTheSameId_ThrowInvalidOperationException()
        {
            User userSteve = new User(1, "Steve", "Jobs");
            User userBill = new User(1, "Bill", "Gates");
            User[] users = new User[] { userSteve, userBill };

            BusinessLogic busLogic = new BusinessLogic(users);
            Assert.ThrowsException<System.InvalidOperationException>(
                () => busLogic.GetUserByID(1));
        }
    }
}
