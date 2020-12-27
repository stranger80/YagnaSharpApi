using Microsoft.VisualStudio.TestTools.UnitTesting;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class DemandBuilderTests
    {
        [TestMethod]
        public void DemandBuilder_BuildsConstraintsCorrectly()
        {
            var builder = new DemandBuilder();

            Assert.AreEqual("()", builder.Constraints);

            builder.Ensure("(some.prop=123)");

            Assert.AreEqual("(some.prop=123)", builder.Constraints);

            builder.Ensure("(other.prop=abc)");

            Assert.AreEqual("(&(some.prop=123)\n\t(other.prop=abc))", builder.Constraints);

        }
    }
}
