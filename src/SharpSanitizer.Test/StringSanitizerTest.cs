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
                { "StringMax", new Constraint(ConstraintType.Max, MaxRef) },
                { "StringNoWhiteSpace", new Constraint(ConstraintType.NoWhiteSpace) },
                { "StringNoSpecialCharacters", new Constraint(ConstraintType.NoSpecialCharacters) },
                { "StringOnlyDigit", new Constraint(ConstraintType.OnlyDigit) },
                { "ValidDatetime", new Constraint(ConstraintType.ValidDatetime) },
                { "ForceToValidDatetime", new Constraint(ConstraintType.ForceToValidDatetime) },
                { "StringSingleChar", new Constraint(ConstraintType.SingleChar) },
                { "StringValidGuid", new Constraint(ConstraintType.ForceValidGuid) }
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
    }
}
