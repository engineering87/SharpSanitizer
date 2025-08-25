// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSanitizer.Entity;
using SharpSanitizer.Enum;
using SharpSanitizer.Test.Model;
using System;
using System.Collections.Generic;

namespace SharpSanitizer.Test
{
    [TestClass]
    public class ObjectSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "ListPropertyNotNull", new Constraint(ConstraintType.NotNull) },
                { "ObjectPropertyNoDbNull", new Constraint(ConstraintType.NoDbNull) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestList()
        {
            var fooModel = new FooModel()
            {
                ListPropertyNotNull = null,
                ObjectPropertyNoDbNull = DBNull.Value
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsFalse(fooModel.ListPropertyNotNull == null);
            Assert.IsFalse(fooModel.ObjectPropertyNoDbNull == DBNull.Value);
        }

        [TestMethod]
        public void NotNull_List_IsInitialized_WhenNull()
        {
            var fooModel = new FooModel
            {
                ListPropertyNotNull = null
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsNotNull(fooModel.ListPropertyNotNull, "List should be initialized when null due to NotNull constraint.");
            Assert.AreEqual(0, fooModel.ListPropertyNotNull.Count, "Newly created list should be empty.");
        }

        [TestMethod]
        public void NotNull_List_PreservesExistingInstance()
        {
            var original = new List<string> { "a", "b" };
            var fooModel = new FooModel
            {
                ListPropertyNotNull = original
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsNotNull(fooModel.ListPropertyNotNull, "Existing list should remain non-null.");
            Assert.AreSame(original, fooModel.ListPropertyNotNull, "Sanitizer should not replace an already non-null instance.");
            CollectionAssert.AreEquivalent(new[] { "a", "b" }, fooModel.ListPropertyNotNull, "Existing contents should be preserved.");
        }

        [TestMethod]
        public void NoDbNull_Object_ReplacesDbNullWithNull()
        {
            var fooModel = new FooModel
            {
                ObjectPropertyNoDbNull = DBNull.Value
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsNull(fooModel.ObjectPropertyNoDbNull, "DBNull.Value should be converted to null by NoDbNull constraint.");
        }

        [TestMethod]
        public void NoDbNull_Object_LeavesNullAsNull()
        {
            var fooModel = new FooModel
            {
                ObjectPropertyNoDbNull = null
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsNull(fooModel.ObjectPropertyNoDbNull, "Null should remain null (NoDbNull only converts DBNull.Value).");
        }

        [TestMethod]
        public void NoDbNull_Object_LeavesNonDbNullUnchanged()
        {
            var sentinel = "hello";
            var fooModel = new FooModel
            {
                ObjectPropertyNoDbNull = sentinel
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.AreSame(sentinel, fooModel.ObjectPropertyNoDbNull, "Non-DBNull values should be preserved by NoDbNull.");
        }

        [TestMethod]
        public void Combined_ListAndObject_BehaveAsExpected()
        {
            var fooModel = new FooModel
            {
                ListPropertyNotNull = null,
                ObjectPropertyNoDbNull = DBNull.Value
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsNotNull(fooModel.ListPropertyNotNull, "List should be initialized.");
            Assert.IsNull(fooModel.ObjectPropertyNoDbNull, "DBNull.Value should be converted to null.");
        }
    }
}
