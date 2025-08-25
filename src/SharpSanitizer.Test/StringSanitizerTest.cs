// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSanitizer.Entity;
using SharpSanitizer.Enum;
using SharpSanitizer.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpSanitizer.Test
{
    [TestClass]
    public class StringSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        private const int MaxNotNullRef = 10;

        private const int MaxRef = 4;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "StringMaxNotNull", new Constraint(ConstraintType.MaxNotNull, MaxNotNullRef) },
                { "StringMax", new Constraint(ConstraintType.MaxLength, MaxRef) },
                { "StringNoWhiteSpace", new Constraint(ConstraintType.NoWhiteSpace) },
                { "StringNoSpecialCharacters", new Constraint(ConstraintType.NoSpecialCharacters) },
                { "StringOnlyDigit", new Constraint(ConstraintType.OnlyDigit) },
                { "ValidDatetime", new Constraint(ConstraintType.ValidDatetime) },
                { "ForceToValidDatetime", new Constraint(ConstraintType.ForceToValidDatetime) },
                { "StringSingleChar", new Constraint(ConstraintType.SingleChar) },
                { "StringValidGuid", new Constraint(ConstraintType.ValidGuid) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestString()
        {
            var fooModel = new FooModel()
            {
                StringMaxNotNull = null,
                StringMax = "abcdefghilmnopqrstuvz",
                StringNoWhiteSpace = "fjd auwc s 111",
                StringNoSpecialCharacters = "%test&''^@",
                StringOnlyDigit = "svvev 79326 .-",
                StringSingleChar = " yes",
                StringValidGuid = "6F9619FF-8B86-D011-notvalid-00C04FC964FF"
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(fooModel.StringMax.Length <= MaxNotNullRef);
            Assert.IsNotNull(fooModel.StringMaxNotNull);
            Assert.IsFalse(fooModel.StringNoWhiteSpace.Any(Char.IsWhiteSpace));
            Assert.IsFalse(fooModel.StringNoSpecialCharacters.Any(ch => !Char.IsLetterOrDigit(ch)));
            Assert.IsTrue(fooModel.StringOnlyDigit.All(char.IsDigit));
            Assert.IsTrue(fooModel.StringSingleChar.Length == 1);
            Assert.IsTrue(Guid.TryParse(fooModel.StringValidGuid, out _));
        }

        [TestMethod]
        public void TestValidDatetime()
        {
            var fooModel = new FooModel()
            {
                ValidDatetime = "2024-25-01",
                ForceToValidDatetime = "2024-25-01"
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(fooModel.ValidDatetime == string.Empty);
            Assert.IsTrue(fooModel.ForceToValidDatetime == DateTime.MinValue.ToString());
        }

        [TestMethod]
        public void MaxLength_BelowBoundary_RemainsUnchanged()
        {
            var fooModel = new FooModel
            {
                StringMax = "ABC" // below 4
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual("ABC", fooModel.StringMax);
        }

        [TestMethod]
        public void MaxNotNull_NullBecomesEmpty_And_TruncatesWhenLonger()
        {
            var fooModel = new FooModel
            {
                StringMaxNotNull = null
            };

            _sharpSanitizer.Sanitize(fooModel);
            Assert.IsNotNull(fooModel.StringMaxNotNull);
            Assert.AreEqual(string.Empty, fooModel.StringMaxNotNull, "Null should become empty for MaxNotNull");

            fooModel.StringMaxNotNull = new string('x', MaxNotNullRef + 5);
            _sharpSanitizer.Sanitize(fooModel);
            Assert.IsTrue(fooModel.StringMaxNotNull.Length <= MaxNotNullRef, "Should truncate to MaxNotNullRef");
        }

        [TestMethod]
        public void NoWhiteSpace_RemovesSpacesTabsNewlines()
        {
            var fooModel = new FooModel
            {
                StringNoWhiteSpace = " a \t b \r\n c "
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual("abc", fooModel.StringNoWhiteSpace, "All whitespace (spaces/tabs/newlines) should be removed");
        }

        [TestMethod]
        public void NoSpecialCharacters_AllowsUnderscoreAndDot()
        {
            var fooModel = new FooModel
            {
                StringNoSpecialCharacters = "file_name.v1"
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual("file_name.v1", fooModel.StringNoSpecialCharacters);
        }

        [TestMethod]
        public void OnlyDigit_PreservesLeadingZeros_And_RemovesOthers()
        {
            var fooModel = new FooModel
            {
                StringOnlyDigit = "  00-12 3a4 "
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual("001234", fooModel.StringOnlyDigit, "Non-digits removed, leading zeros preserved");
        }

        [TestMethod]
        public void SingleChar_EmptyOrWhitespace_ReturnsEmpty()
        {
            var fooModel = new FooModel
            {
                StringSingleChar = "   "
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(string.Empty, fooModel.StringSingleChar, "Whitespace should yield empty after trimming");
        }

        [TestMethod]
        public void ValidDatetime_ValidValue_RemainsTrimmed()
        {
            var fooModel = new FooModel
            {
                ValidDatetime = " 2024-01-25 "
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual("2024-01-25", fooModel.ValidDatetime, "Valid date should be preserved and trimmed");
        }

        [TestMethod]
        public void ForceToValidDatetime_NullBecomesMinValueString()
        {
            var fooModel = new FooModel
            {
                ForceToValidDatetime = null
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(DateTime.MinValue.ToString(), fooModel.ForceToValidDatetime);
        }

        [TestMethod]
        public void ValidGuid_Relaxed_InvalidGetsReplacedWithNewGuid()
        {
            var original = "invalid-guid";
            var fooModel = new FooModel
            {
                StringValidGuid = original
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsTrue(Guid.TryParse(fooModel.StringValidGuid, out _), "Relaxed mode should replace invalid with a new GUID");
            Assert.AreNotEqual(original, fooModel.StringValidGuid);
        }

        [TestMethod]
        public void ValidGuid_Strict_InvalidThrows()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "StringValidGuid", new Constraint(ConstraintType.ValidGuid) }
            };
            var strictSanitizer = new SharpSanitizer<FooModel>(constraints, ValidationSeverity.Strict);

            var fooModel = new FooModel { StringValidGuid = "invalid-guid" };

            Assert.ThrowsExactly<ArgumentException>(() => strictSanitizer.Sanitize(fooModel),
                "Strict mode should throw for invalid GUID");
        }

        [TestMethod]
        public void ValidDatetime_InvalidBecomesEmpty_And_ForceToValidDatetime_InvalidBecomesMinValue()
        {
            var fooModel = new FooModel
            {
                ValidDatetime = "2024-25-01",        // invalid
                ForceToValidDatetime = "2024-25-01"  // invalid
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreEqual(string.Empty, fooModel.ValidDatetime);
            Assert.AreEqual(DateTime.MinValue.ToString(), fooModel.ForceToValidDatetime);
        }
    }
}
