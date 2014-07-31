using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StencilBox.Tests
{
    [TestClass]
    public class ProcessTests
    {
        public class TestModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public bool IsTrue { get; set; }
        }

        [TestMethod]
        public void Stencil_handles_normal_brackets_correctly()
        {
            const string template = "[~Title][][~Description]][[~IsTrue]";
            const string expectedOutput = "title[]desc][True";
            var model = new TestModel { Title="title", Description = "desc", IsTrue = true};

            var output = Stencil.Apply(template, model);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
