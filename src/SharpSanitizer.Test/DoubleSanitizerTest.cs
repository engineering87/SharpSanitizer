// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSanitizer.Entity;
using SharpSanitizer.Enum;
using SharpSanitizer.Test.Model;
using System.Collections.Generic;

namespace SharpSanitizer.Test
{
    [TestClass]
    public class DoubleSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        private const int MaxRef = 2;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "DoubleMaxDecimalsPlaces", new Constraint(ConstraintType.MaxDecimalPlaces, MaxRef) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestString()
        {
            var fooModel = new FooModel()
            {
                DoubleMaxDecimalsPlaces = 0.123456789
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(fooModel.DoubleMaxDecimalsPlaces == 0.12);
        }
    }
}
