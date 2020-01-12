using Microsoft.VisualStudio.TestTools.UnitTesting;
using EFS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFS.Utilities.Tests
{
    [TestClass()]
    public class DrawingUIUtilitiesTests
    {
        [TestMethod()]
        public void GetPixelsWidthToDrawTest()
        {
            Assert.AreEqual(DrawingUIUtilities.GetPixelsWidthToDraw(1000, 50.0), 500);
            Assert.AreEqual(DrawingUIUtilities.GetPixelsWidthToDraw(100, 50.0), 50);
            Assert.AreEqual(DrawingUIUtilities.GetPixelsWidthToDraw(100, 75.0), 75);
            Assert.AreEqual(DrawingUIUtilities.GetPixelsWidthToDraw(516, 100), 516);
        }
    }
}