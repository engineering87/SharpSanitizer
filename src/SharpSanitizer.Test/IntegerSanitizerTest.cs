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
    public class IntegerSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        private const int MaxRef = 100;

        private const int MinRef = 50;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "MinIntegerProperty", new Constraint(ConstraintType.Min, MinRef) },
                { "MaxIntegerProperty", new Constraint(ConstraintType.Max, MaxRef) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestInteger()
        {
            var fooModel = new FooModel()
            {
                MaxIntegerProperty = 1000,
                MinIntegerProperty = 100
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(fooModel.MinIntegerProperty == MinRef);
            Assert.IsTrue(fooModel.MaxIntegerProperty == MaxRef);
        }
    }
}
