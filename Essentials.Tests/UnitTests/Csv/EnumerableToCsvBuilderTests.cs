using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Csv;
using AldursLab.Testing.Extensions;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.UnitTests.Csv
{
    [TestFixture]
    public class EnumerableToCsvBuilderTests : AssertionHelper
    {
        [Test]
        public void Given_sample_collection_Builds_expected_output()
        {
            var sampledata = new[]
            {
                new SampleData() {Id = 1, Data = "D|1"},
                new SampleData() {Id = 2, Data = "D\"2\""}
            }.AsEnumerable();

            var builder =
                EnumerableToCsvBuilder.Create(sampledata)
                                      .AddMapping("Id", data => data.Id.ToString())
                                      .AddMapping("Data", data => data.Data);
            var csv = builder.BuildCsv().NormalizeLineEndings();

            Expect(csv, EqualTo(CsvResources.CsvOutput1.NormalizeLineEndings()));

        }

        class SampleData
        {
            public int Id { get; set; }
            public string Data { get; set; }
        }
    }
}
