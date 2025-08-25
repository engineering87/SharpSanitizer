// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSanitizer.Entity;
using SharpSanitizer.Enum;
using System.Collections.Generic;

namespace SharpSanitizer.Test
{
    [TestClass]
    public class NumberSanitizerExtendedTest
    {
        private class NumberModel
        {
            public sbyte SByteValue { get; set; }
            public byte ByteValue { get; set; }
            public short Int16Value { get; set; }
            public ushort UInt16Value { get; set; }
            public int Int32Value { get; set; }
            public uint UInt32Value { get; set; }
            public long Int64Value { get; set; }
            public ulong UInt64Value { get; set; }
        }

        [TestMethod]
        public void SByte_NotNegative_ClampsToZero()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["SByteValue"] = new Constraint(ConstraintType.NotNegative)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { SByteValue = -5 };
            sanitizer.Sanitize(m);

            Assert.AreEqual((sbyte)0, m.SByteValue);
        }

        [TestMethod]
        public void SByte_MinValue_ClampsUp()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["SByteValue"] = new Constraint(ConstraintType.MinValue, 10)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { SByteValue = 3 }; // < 10 -> 10
            sanitizer.Sanitize(m);

            Assert.AreEqual((sbyte)10, m.SByteValue);
        }

        [TestMethod]
        public void SByte_MaxValue_ClampsDown()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["SByteValue"] = new Constraint(ConstraintType.MaxValue, 7)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { SByteValue = 12 }; // > 7 -> 7
            sanitizer.Sanitize(m);

            Assert.AreEqual((sbyte)7, m.SByteValue);
        }

        [TestMethod]
        public void Int16_Min_And_Max_RespectBoundaries()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["Int16Value"] = new Constraint(ConstraintType.MinValue, 100)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { Int16Value = 50 }; // < 100 -> 100
            sanitizer.Sanitize(m);
            Assert.AreEqual((short)100, m.Int16Value);

            constraints["Int16Value"] = new Constraint(ConstraintType.MaxValue, 200);
            sanitizer = new SharpSanitizer<NumberModel>(constraints);

            m = new NumberModel { Int16Value = 250 }; // > 200 -> 200
            sanitizer.Sanitize(m);
            Assert.AreEqual((short)200, m.Int16Value);
        }

        [TestMethod]
        public void Int32_Min_And_Max_InsideRange_Unchanged()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["Int32Value"] = new Constraint(ConstraintType.MinValue, 10)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { Int32Value = 15 }; // >= 10 -> unchanged
            sanitizer.Sanitize(m);
            Assert.AreEqual(15, m.Int32Value);

            constraints["Int32Value"] = new Constraint(ConstraintType.MaxValue, 20);
            sanitizer = new SharpSanitizer<NumberModel>(constraints);

            m = new NumberModel { Int32Value = 18 }; // <= 20 -> unchanged
            sanitizer.Sanitize(m);
            Assert.AreEqual(18, m.Int32Value);
        }

        [TestMethod]
        public void Int64_NotNegative_ClampsUp()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["Int64Value"] = new Constraint(ConstraintType.NotNegative)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { Int64Value = -123456789L };
            sanitizer.Sanitize(m);

            Assert.AreEqual(0L, m.Int64Value);
        }

        [TestMethod]
        public void UInt16_MinValue_WithNegativeRef_IsTreatedAsZero()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["UInt16Value"] = new Constraint(ConstraintType.MinValue, -5)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { UInt16Value = 0 };
            sanitizer.Sanitize(m);

            Assert.AreEqual((ushort)0, m.UInt16Value, "Negative MinRef should be treated as 0 for unsigned");
        }

        [TestMethod]
        public void UInt32_MaxValue_ClampsDown_And_BoundaryUnchanged()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["UInt32Value"] = new Constraint(ConstraintType.MaxValue, 100)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m1 = new NumberModel { UInt32Value = 150U }; // > 100 -> 100
            sanitizer.Sanitize(m1);
            Assert.AreEqual(100U, m1.UInt32Value);

            var m2 = new NumberModel { UInt32Value = 100U }; // == 100 -> unchanged
            sanitizer.Sanitize(m2);
            Assert.AreEqual(100U, m2.UInt32Value);
        }

        [TestMethod]
        public void UInt64_MaxValue_WithNegativeRef_BecomesZero()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["UInt64Value"] = new Constraint(ConstraintType.MaxValue, -1)
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel { UInt64Value = 42UL };
            sanitizer.Sanitize(m);

            Assert.AreEqual(0UL, m.UInt64Value, "Negative MaxRef coerced to 0 for unsigned max clamp");
        }

        [TestMethod]
        public void Mixed_MultipleConstraints_DoNotInterfere()
        {
            var constraints = new Dictionary<string, Constraint>
            {
                ["Int32Value"] = new Constraint(ConstraintType.MinValue, 10),
                ["UInt32Value"] = new Constraint(ConstraintType.MaxValue, 5),
                ["Int64Value"] = new Constraint(ConstraintType.NotNegative),
            };
            var sanitizer = new SharpSanitizer<NumberModel>(constraints);

            var m = new NumberModel
            {
                Int32Value = 3,     // -> 10
                UInt32Value = 9U,    // -> 5
                Int64Value = -2L    // -> 0
            };

            sanitizer.Sanitize(m);

            Assert.AreEqual(10, m.Int32Value);
            Assert.AreEqual(5U, m.UInt32Value);
            Assert.AreEqual(0L, m.Int64Value);
        }
    }
}
