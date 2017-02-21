using AJF.PhotoMover.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AJF.PhotoMover.Test
{
    [TestClass]
    public class PhotoMoverConfigTests
    {
        [TestMethod]
        public void CanBeConstructed()
        {
            var sut = new PhotoMoverConfig {Path = "abc"};

            Assert.AreEqual("abc",sut.Path);
        }
    }
}