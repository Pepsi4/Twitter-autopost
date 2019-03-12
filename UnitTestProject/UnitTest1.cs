using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalMVVM;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DateMethodTest()
        {
            WindowViewModel windowViewModel = new WindowViewModel();

            DateTime testDate1 = new DateTime(
                DateTime.Now.Year + 1,
                DateTime.Now.Month,
                DateTime.Now.Day
                );

            DateTime testDate2 = new DateTime(
               DateTime.Now.Year,
               DateTime.Now.Month + 1,
               DateTime.Now.Day
               );

            DateTime testDate3 = new DateTime(
               DateTime.Now.Year,
               DateTime.Now.Month,
               DateTime.Now.Day + 1
               );

            DateTime testDate4 = new DateTime(
               DateTime.Now.Year,
               DateTime.Now.Month,
               DateTime.Now.Day
               );

            Assert.AreEqual(true, windowViewModel.IsDateBiggerThanToday(testDate1));
            Assert.AreEqual(true, windowViewModel.IsDateBiggerThanToday(testDate2));
            Assert.AreEqual(true, windowViewModel.IsDateBiggerThanToday(testDate3));
            Assert.AreEqual(false, windowViewModel.IsDateBiggerThanToday(testDate4));
        }
    }
}
