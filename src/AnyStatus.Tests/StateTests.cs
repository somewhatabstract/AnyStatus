using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AnyStatus.Tests
{
    [TestClass]
    public class StateTests
    {
        [Ignore]
        [TestMethod]
        public void Serialization()
        {
            var xml = string.Empty;
            var stringWriter = new StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(State));
            var settings = new XmlWriterSettings { Indent = true };
            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<State>
  <Value>4</Value>
  <Priority>4</Priority>
  <DisplayName>Ok</DisplayName>
  <Color>Green</Color>
  <IconName>StatusOK_grey_16x.png</IconName>
</State>";

            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                writer.WriteStartDocument();
                xmlSerializer.Serialize(writer, State.Ok);
                writer.WriteEndDocument();
                xml = stringWriter.ToString();
            }

            Assert.IsFalse(string.IsNullOrEmpty(xml));
            Assert.AreEqual(expected, xml);
        }

        [Ignore]
        [TestMethod]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(State));
            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<State>
  <Value>4</Value>
  <Priority>4</Priority>
  <DisplayName>Ok</DisplayName>
  <Color>Green</Color>
  <IconName>StatusOK_grey_16x.png</IconName>
</State>";

            object result;

            using (var reader = new StringReader(expected))
            {
                result = serializer.Deserialize(reader);
            }

            Assert.IsNotNull(result);
        }
    }
}
