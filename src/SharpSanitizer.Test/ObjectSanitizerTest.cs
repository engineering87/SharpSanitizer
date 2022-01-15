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
    public class ObjectSanitizerTest
    {
        private ISharpSanitizer<FooModel> _sharpSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            var constraints = new Dictionary<string, Constraint>()
            {
                { "ListProperty", new Constraint(ConstraintType.NotNull) }
            };

            _sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
        }

        [TestMethod]
        public void TestList()
        {
            var fooModel = new FooModel()
            {
                ListProperty = null
            };

            _sharpSanitizer.Sanitize(fooModel);

            Assert.IsFalse(fooModel.ListProperty == null);
        }
    }
}
