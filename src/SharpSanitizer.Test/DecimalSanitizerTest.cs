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
    public class DecimalSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        private const int MaxRef = 2;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "DecimalMaxDecimalsPlaces", new Constraint(ConstraintType.MaxDecimalPlaces, MaxRef) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestString()
        {
            var fooModel = new FooModel()
            {
                DecimalMaxDecimalsPlaces = 0.123456789M
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(fooModel.DecimalMaxDecimalsPlaces == 0.12M);
        }

        [TestMethod]
        public void Truncates_To_Two_Decimals_Positive()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 0.123456789M
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(0.12M, fooModel.DecimalMaxDecimalsPlaces, "Should truncate, not round, to 2 decimals");
        }

        [TestMethod]
        public void Truncates_To_Two_Decimals_Negative()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = -123.4567M
            };

            _sharpSanitizer.Sanitize(fooModel);

            // MidpointRounding.ToZero: -123.4567 -> -123.45
            Assert.AreEqual(-123.45M, fooModel.DecimalMaxDecimalsPlaces, "Negative values should be truncated toward zero");
        }

        [TestMethod]
        public void Leaves_Value_With_Exactly_Two_Decimals_Unchanged()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 123.45M
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(123.45M, fooModel.DecimalMaxDecimalsPlaces, "Exactly 2 decimals should remain unchanged");
        }

        [TestMethod]
        public void Leaves_Value_With_Fewer_Than_Two_Decimals_Unchanged()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 10M
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(10M, fooModel.DecimalMaxDecimalsPlaces, "Integer value should remain unchanged");
        }

        [TestMethod]
        public void Boundary_Not_Rounded_Up()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 1.239M
            };

            _sharpSanitizer.Sanitize(fooModel);

            // Truncate to 1.23 (not 1.24)
            Assert.AreEqual(1.23M, fooModel.DecimalMaxDecimalsPlaces, "Should truncate (not round up) at the boundary");
        }

        [TestMethod]
        public void Works_With_Large_Values()
        {
            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 9876543210.98765M
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(9876543210.98M, fooModel.DecimalMaxDecimalsPlaces, "Large values should truncate correctly");
        }

        [TestMethod]
        public void Zero_Decimals_When_MaxRef_Is_Zero()
        {
            // Override setup with MaxRef = 0 for this test
            var constraints = new Dictionary<string, Constraint>()
            {
                { "DecimalMaxDecimalsPlaces", new Constraint(ConstraintType.MaxDecimalPlaces, 0) }
            };
            var sanitizer = new SharpSanitizer<FooModel>(constraints);

            var fooModel = new FooModel
            {
                DecimalMaxDecimalsPlaces = 123.9876M
            };

            sanitizer.Sanitize(fooModel);

            Assert.AreEqual(123M, fooModel.DecimalMaxDecimalsPlaces, "With MaxRef=0 should drop all fractional digits (truncate)");
        }
    }
}
