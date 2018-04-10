using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ProtoBuf.Meta;

namespace AgGateway.ADAPT.TestUtilities.ProtobufContract
{
    [TestFixture]
    public class ProtobufContractGeneratorTest
    {
        private string _tempXmlFileCorrect;
        private string _tempXmlFileIncorrect;
        private string _tempProtoFile;
        private string _tempDirectory;

        private readonly TestClassA _testClassA = new TestClassA { AString1 = "ABC", AString2 = "XYZ" };

        [SetUp]
        public void Setup()
        {
            var resourceDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ProtobufTestFiles");

            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            _tempDirectory = tempDirectory;

            _tempXmlFileCorrect = Path.Combine(tempDirectory, "text2.xml");
            File.WriteAllText(_tempXmlFileCorrect, File.ReadAllText(Path.Combine(resourceDirectory, "ProtobufMappingTest2.xml")));

            _tempXmlFileIncorrect = Path.Combine(tempDirectory, "text3.xml");
            File.WriteAllText(_tempXmlFileIncorrect, File.ReadAllText(Path.Combine(resourceDirectory, "ProtobufMappingTest3.xml")));

            _tempProtoFile = Path.Combine(tempDirectory, "test.proto");
            File.WriteAllBytes(_tempProtoFile, File.ReadAllBytes(Path.Combine(resourceDirectory, "test.proto")));
        }

        
        [Test]
        [Ignore("Run this test to generate the protobuf model code")]
        public void GenerateCodeThatCreateProtobufModel()
        {
            var generator = new ProtobufContractGenerator(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ProtobufMapping.xml"));

            // BEGINNING OF GENERATED CODE:
            // This code is generated by running the GenerateCodeThatCreateProtobufModel() method in ProtobufContractGeneratorTest
            // (but you have to manually copy it)
            
            Debug.WriteLine(@"// BEGINNING OF GENERATED CODE");
            Debug.WriteLine(@"// This code is generated by running the GenerateCodeThatCreateProtobufModel() method in ProtobufContractGeneratorTest");
            Debug.WriteLine(@"// (but you have to manually copy it here)");
            
            generator.GenerateContractCode("AgGateway.ADAPT.ApplicationDataModel.dll");

            // These are manually added.  They are used for reference layer protobuf
            Debug.WriteLine(@"model[typeof(RasterData<EnumeratedRepresentation, EnumerationMember>)].Add(463, ""Representation"");");
            Debug.WriteLine(@"model[typeof(RasterData<StringRepresentation, string>)].Add(464, ""Representation"");");
            Debug.WriteLine(@"model[typeof(RasterData<NumericRepresentation, NumericValue>)].Add(465, ""Representation"");");

            Debug.WriteLine(@"model[typeof(SerializableRasterData<string>)].Add(466, ""values"");");
            Debug.WriteLine(@"model[typeof(SerializableRasterData<EnumerationMember>)].Add(467, ""values"");");
            Debug.WriteLine(@"model[typeof(SerializableRasterData<NumericValue>)].Add(468, ""values"");");
            Debug.WriteLine(@"model[typeof(SerializableRasterData<string>)].Add(469, ""Representation"");");
            Debug.WriteLine(@"model[typeof(SerializableRasterData<EnumerationMember>)].Add(470, ""Representation"");");
            Debug.WriteLine(@"model[typeof(SerializableRasterData<NumericValue>)].Add(471, ""Representation"");");

            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(472, ""RasterReferenceLayer"");");
            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(473, ""StringValues"");");
            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(474, ""EnumerationMemberValues"");");
            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(475, ""NumericValueValues"");");

            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(849, ""ShapeReferenceLayer"");");
            Debug.WriteLine(@"model[typeof(SerializableReferenceLayer)].Add(850, ""ShapeLookupValues"");");
            Debug.WriteLine(@"model[typeof(SerializableShapeData)].Add(851, ""shapeLookups"");");

            Debug.WriteLine(@"// END OF GENERATED CODE:");
            Debug.WriteLine(@"//");
            Debug.WriteLine(@"//");
        }

        [Test]
        public void WhenExportThenFileIsWritten()
        {
            var generator = new ProtobufContractGenerator(_tempXmlFileCorrect);
            var model = generator.GenerateContractCode("AgGateway.ADAPT.TestUtilities.dll");
            
            var filename = Path.Combine(_tempDirectory, "test.proto");

            var testClassA = new TestClassA { AString1 = "ABC", AString2 = "XYZ" };

            Write(filename, testClassA, model);

            File.Exists(filename);
        }

        [Test]
        public void WhenExportAndImportTestClassesThenWorks()
        {
            var generator = new ProtobufContractGenerator(_tempXmlFileCorrect);
            var model = generator.GenerateContractCode("AgGateway.ADAPT.TestUtilities.dll");

            var filename = Path.Combine(_tempDirectory, "test.proto");

            var testClassA = new TestClassA { AString1 = "ABC", AString2 = "XYZ"};

            Write(filename, testClassA, model);

            var readTestClassA = Read<TestClassA>(filename, model);

            Assert.AreEqual(testClassA.AString1, readTestClassA.AString1);
            Assert.AreEqual(testClassA.AString2, readTestClassA.AString2);
        }

        [Test]
        public void WhenImportWithWrongContractThenDoesNotWork()
        {
            var generator = new ProtobufContractGenerator(_tempXmlFileIncorrect);
            var model = generator.GenerateContractCode("AgGateway.ADAPT.TestUtilities.dll");

            var readTestClassA = Read<TestClassA>(_tempProtoFile, model);

            Assert.AreNotEqual(_testClassA.AString1, readTestClassA.AString1);
            Assert.AreNotEqual(_testClassA.AString2, readTestClassA.AString2);
        }

        [Test]
        public void WhenImportWithCorrectContractThenWorks()
        {
            var generator = new ProtobufContractGenerator(_tempXmlFileCorrect);
            var model = generator.GenerateContractCode("AgGateway.ADAPT.TestUtilities.dll");

            var readTestClassA = Read<TestClassA>(_tempProtoFile, model);

            Assert.AreEqual(_testClassA.AString1, readTestClassA.AString1);
            Assert.AreEqual(_testClassA.AString2, readTestClassA.AString2);
        }

        [Test]
        public void GivenBadAssemblyWhenGenerateThenModelIsNull()
        {
            var generator = new ProtobufContractGenerator(_tempXmlFileCorrect);
            var model = generator.GenerateContractCode("ThisDoesntExist.dll");

            Assert.IsNull(model);
        }

        [Test]
        public void GivenBadXmlFileWhenGenerateThenModelIsNull()
        {
            var generator = new ProtobufContractGenerator(@"..\..\ProtobufTestFiles\HelloThere.xml");
            var model = generator.GenerateContractCode("TestUtilities.dll");

            Assert.IsNull(model);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tempDirectory, true);
        }

        private void Write<T>(string path, T content, RuntimeTypeModel model)
        {
            using (var fileStream = File.Open(path, FileMode.Create))
            {
                model.Serialize(fileStream, content);
            }
        }

        private T Read<T>(string path, RuntimeTypeModel model)
        {
            using (var fileStream = File.OpenRead(path))
            {
                return (T)model.Deserialize(fileStream, null, typeof(T));
            }
        }
    }
}
