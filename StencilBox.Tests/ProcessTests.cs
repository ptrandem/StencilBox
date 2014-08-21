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
            public DateTime TestDate { get; set; }
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

        [TestMethod]
        public void Stencil_handles_null_properties_correctly()
        {
            const string template = "[~Title][~Description]";
            const string expectedOutput = "";
            var model = new TestModel();

            var output = Stencil.Apply(template, model, null, ProcessFlags.CleanUnprocessed);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Stencil_handles_dates_correctly()
        {
            const string template = "[~TestDate]";
            const string expectedOutput = "5/2/1979 12:00:00 AM";
            var model = new TestModel { TestDate = DateTime.Parse("05/02/1979") };

            var output = Stencil.Apply(template, model);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Stencil_handles_format_strings_correctly()
        {
            const string template = "[~TestDate:MM/dd/yyyy]";
            const string expectedOutput = "05/02/1979";
            var model = new TestModel { TestDate = DateTime.Parse("05/02/1979") };

            var output = Stencil.Apply(template, model);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Stencil_handles_mindates_correctly()
        {
            const string template = "[~TestDate:MM/dd/yyyy]";
            const string expectedOutput = "";
            var model = new TestModel();

            var output = Stencil.Apply(template, model);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
