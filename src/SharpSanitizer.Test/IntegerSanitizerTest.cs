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
                { "MinIntegerProperty", new Constraint(ConstraintType.MinValue, MinRef) },
                { "MaxIntegerProperty", new Constraint(ConstraintType.MaxValue, MaxRef) }
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

            Assert.AreEqual(MaxRef, fooModel.MaxIntegerProperty);
            Assert.AreEqual(100, fooModel.MinIntegerProperty);
        }

        [TestMethod]
        public void TestInteger_AboveMax_IsClampedDown()
        {
            var fooModel = new FooModel
            {
                MaxIntegerProperty = 1000, // > MaxRef
                MinIntegerProperty = 100   // >= MinRef → unchanged
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(MaxRef, fooModel.MaxIntegerProperty, "Max should be clamped to MaxRef");
            Assert.AreEqual(100, fooModel.MinIntegerProperty, "Min within bounds should remain unchanged");
        }

        [TestMethod]
        public void Min_BelowMin_IsClampedUp()
        {
            var fooModel = new FooModel
            {
                MinIntegerProperty = 10  // < MinRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(MinRef, fooModel.MinIntegerProperty, "Min below MinRef should be clamped up to MinRef");
        }

        [TestMethod]
        public void Min_AtMin_Remains()
        {
            var fooModel = new FooModel
            {
                MinIntegerProperty = MinRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(MinRef, fooModel.MinIntegerProperty, "Min at boundary should remain unchanged");
        }

        [TestMethod]
        public void Max_BelowMax_Remains()
        {
            var fooModel = new FooModel
            {
                MaxIntegerProperty = 80 // < MaxRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(80, fooModel.MaxIntegerProperty, "Max below MaxRef should remain unchanged");
        }

        [TestMethod]
        public void Max_AtMax_Remains()
        {
            var fooModel = new FooModel
            {
                MaxIntegerProperty = MaxRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(MaxRef, fooModel.MaxIntegerProperty, "Max at boundary should remain unchanged");
        }

        [TestMethod]
        public void Min_Negative_IsClampedUpToMin()
        {
            var fooModel = new FooModel
            {
                MinIntegerProperty = -123 // << MinRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(MinRef, fooModel.MinIntegerProperty, "Negative min should be clamped up to MinRef");
        }

        [TestMethod]
        public void Both_WithinBounds_RemainUnchanged()
        {
            var fooModel = new FooModel
            {
                MinIntegerProperty = 60, // >= MinRef
                MaxIntegerProperty = 90  // <= MaxRef
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(60, fooModel.MinIntegerProperty, "Min within bounds should remain unchanged");
            Assert.AreEqual(90, fooModel.MaxIntegerProperty, "Max within bounds should remain unchanged");
        }
    }
}
