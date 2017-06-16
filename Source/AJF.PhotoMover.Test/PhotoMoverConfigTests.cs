using AJF.PhotoMover.Service;
using NUnit.Framework;

namespace AJF.PhotoMover.Test
{
    [TestFixture]
    public class PhotoMoverConfigTests
    {
        [Test]
        public void CanBeConstructed()
        {
            var sut = new PhotoMoverConfig {Path = "abc"};

            Assert.AreEqual("abc",sut.Path);
        }
    }
}