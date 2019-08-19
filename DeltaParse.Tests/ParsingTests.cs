using System;
using DeltaParse.Processors;
using NUnit.Framework;

namespace DeltaParse.Tests
{
    [TestFixture]
    public class ParsingTests
    {
        [Test]
        public void Given_SimpleText_ExpectValues()
        {
            string text = "The quick brown fox jumps over the lazy dog";
            string template = "The quick {{colour}} fox jumps over the lazy {{animal}}";

            var parser = new Parser(template, new StandardProcessor());

            var results = parser.Parse(text);

            Assert.AreEqual("brown", results.ParsedValues["colour"][0]);
            Assert.AreEqual("dog", results.ParsedValues["animal"][0]);
        }

        [Test]
        public void Given_SimpleText_MultipleTokens_ExpectValues()
        {
            string text = "The quick brown fox jumps over the lazy dog";
            string template = "The {{adjective}} {{adjective}} fox {{verb}} over the {{adjective}} dog";

            var parser = new Parser(template, new StandardProcessor());

            var results = parser.Parse(text);

            Assert.AreEqual("quick", results.ParsedValues["adjective"][0]);
            Assert.AreEqual("brown", results.ParsedValues["adjective"][1]);
            Assert.AreEqual("lazy", results.ParsedValues["adjective"][2]);
            Assert.AreEqual("jumps", results.ParsedValues["verb"][0]);
        }

        [Test]
        public void Given_Template_MungeProcessor_ExpectValues()
        {
            string text = "The {{adjective}} {{adjective}} fox {{verb}} over the {{adjective}} dog";
            string template = "The {{adjective}} {{adjective}} fox {{verb}} over the {{adjective}} dog";

            var parser = new Parser(template, new MungeProcessor());

            var results = parser.Parse(text);

            Assert.AreEqual("{{adjective}}", results.ParsedValues["adjective"][0]);
            Assert.AreEqual("{{adjective}}", results.ParsedValues["adjective"][1]);
            Assert.AreEqual("{{adjective}}", results.ParsedValues["adjective"][2]);
            Assert.AreEqual("{{verb}}", results.ParsedValues["verb"][0]);
        }

        [Test]
        public void Given_SimpleHtml_ExpectValues()
        {
            string text = @"
<html>
    <header><title>Page Title</title></header>
    <body>
        I said, Hello world
    </body>
</html>";
            string template = @"
<html>
    <header><title>{{Title}}</title></header>
    <body>
        I said, {{Message}}
    </body>
</html>";

            var parser = new Parser(template, new StandardProcessor());

            var results = parser.Parse(text);

            Assert.AreEqual("Page Title", results.ParsedValues["Title"][0]);
            Assert.AreEqual("Hello world", results.ParsedValues["Message"][0]);
        }

        [Test]
        public void Given_ComplexHtml_ExpectValues()
        {
            string text = @"
<html>
    <header><title>Page Title</title></header>
    <body>
        <p>
        Pulvinar luctus senectus tempus purus porttitor litora! Eu quisque et ultricies ligula a venenatis sed odio in. Per auctor mollis sem sociis libero ut facilisi nam? Curabitur neque donec aliquam phasellus et neque sit lacus turpis eleifend sociis. Sociis dictum at auctor? Id penatibus urna sollicitudin egestas posuere posuere duis accumsan ligula elit porttitor. Nulla mi consectetur accumsan a orci dictum non tempus tellus ligula. Aliquet montes faucibus?
        </p>
        <p>
        Mi quis, adipiscing eros congue nullam libero curabitur pretium inceptos eleifend sollicitudin? Mus tincidunt ac convallis per interdum imperdiet risus fringilla porta potenti lectus hendrerit. Auctor pellentesque est mollis neque elit sem nullam nisi? Pellentesque tempor vehicula tincidunt in egestas cursus! Non habitasse laoreet leo nulla torquent et sit lacus urna per suscipit semper. Non tincidunt ultrices augue varius cum nam. Molestie ut convallis natoque, suspendisse duis lacinia justo quisque varius porta habitant pulvinar.
        </p>
        <p>
        In facilisi per aptent, mauris neque dictumst ut. Tincidunt tempus gravida varius sed tempor morbi per mauris quisque inceptos. Praesent feugiat sollicitudin habitant donec accumsan ultrices lacus! Ipsum nulla lacinia amet hac habitasse habitant dui sociis himenaeos quis. Eros velit, feugiat at risus massa nulla sollicitudin. Tristique litora in risus duis vitae nunc cursus. Ullamcorper aenean metus arcu vehicula. Nulla fusce, ante montes turpis lacinia gravida sollicitudin nascetur dolor. Ad at pharetra magnis eros, class dapibus porta natoque tortor feugiat cum. Turpis himenaeos cras aptent odio feugiat lorem sodales aliquet pellentesque ligula. Enim tristique nullam lorem aliquet montes.
        </p>
        <p>
        Aliquet felis congue curabitur? Sit fusce vehicula neque cursus phasellus adipiscing. Interdum hendrerit maecenas sem laoreet magnis imperdiet luctus convallis ultrices amet. Nisl fringilla parturient platea massa est senectus morbi magna montes magnis! Semper aenean congue risus erat proin venenatis donec. Ullamcorper dictumst fames condimentum tempor ante nostra platea per leo elementum aliquet. Tortor commodo rhoncus nam condimentum in mattis justo vulputate molestie cum! Risus habitasse adipiscing id facilisis id ad quam nunc, vitae dictumst. Vestibulum duis cubilia curabitur? Dictum hendrerit facilisi, malesuada fusce.
        </p>
        <p>
        Risus bibendum montes imperdiet tempor lacinia mauris lacinia quis natoque velit hac! Pulvinar amet dolor, nostra elit suspendisse non blandit potenti. Magnis iaculis integer nisi. Ornare vulputate inceptos tortor malesuada felis ullamcorper. Elit ad nostra integer congue porta eros lectus mauris justo, pretium fermentum vehicula. Enim aliquet pretium nascetur sociis praesent lorem lacus cubilia pretium lobortis. Sit tempor cursus hac eu eget integer nec sagittis suscipit. Aenean tellus proin quam pulvinar nulla sed neque, laoreet ultricies. Mattis viverra placerat conubia non. Primis per curabitur dis congue nullam nunc velit ipsum porttitor!
        </p>
    </body>
</html>";
            string template = @"
<html>
    <header><title>Page Title</title></header>
    <body>
        <p>
        {{line1}}
        </p>
        <p>
        Mi quis, adipiscing eros congue nullam libero curabitur pretium inceptos eleifend sollicitudin? Mus tincidunt ac convallis per interdum imperdiet risus fringilla porta potenti lectus hendrerit. Auctor pellentesque est mollis neque elit sem nullam nisi? Pellentesque tempor vehicula tincidunt in egestas cursus! Non habitasse laoreet leo nulla torquent et sit lacus urna per suscipit semper. Non tincidunt ultrices augue varius cum nam. Molestie ut convallis natoque, suspendisse duis lacinia justo quisque varius porta habitant pulvinar.
        </p>
        <p>
        In {{line2}}, mauris neque dictumst ut{{line2}} sed tempor morbi per mauris quisque inceptos. Praesent feugiat sollicitudin habitant donec accumsan ultrices lacus! Ipsum nulla lacinia amet hac habitasse habitant dui sociis himenaeos quis. Eros velit, feugiat at risus massa nulla sollicitudin. Tristique litora in risus duis vitae nunc cursus. Ullamcorper aenean metus arcu vehicula. Nulla fusce, ante montes turpis lacinia gravida sollicitudin nascetur dolor. Ad at pharetra magnis eros, class dapibus porta natoque tortor feugiat cum. Turpis himenaeos cras aptent odio feugiat lorem sodales aliquet pellentesque ligula. {{line2}}lorem aliquet montes.
        </p>
        <p>
        Aliquet felis congue curabitur? Sit fusce vehicula neque cursus phasellus adipiscing. Interdum hendrerit maecenas sem laoreet magnis imperdiet luctus convallis ultrices amet. Nisl fringilla parturient platea massa est senectus morbi magna montes magnis! Semper aenean congue risus erat proin venenatis donec. Ullamcorper dictumst fames condimentum tempor ante nostra platea per leo elementum aliquet. Tortor commodo rhoncus nam condimentum in mattis justo vulputate molestie cum! Risus habitasse adipiscing id facilisis id ad quam nunc, vitae dictumst. Vestibulum duis cubilia curabitur? Dictum hendrerit facilisi, malesuada fusce.
        </p>
        <p>
        Risus {{line4}} suspendisse non blandit potenti. Magnis iaculis integer nisi. Ornare vulputate inceptos tortor malesuada felis ullamcorper. Elit ad nostra integer congue porta eros lectus mauris justo, pretium fermentum {{line4}}.{{line4}}pretium nascetur sociis praesent lorem lacus cubilia pretium lobortis. Sit tempor cursus hac eu eget integer nec sagittis suscipit. Aenean tellus proin quam pulvinar nulla sed neque, laoreet ultricies. Mattis viverra placerat conubia non. Primis per curabitur dis congue nullam nunc velit ipsum porttitor!
        </p>
    </body>
</html>";

            var parser = new Parser(template, new StandardProcessor());

            var results = parser.Parse(text);

            Assert.AreEqual("Pulvinar luctus senectus tempus purus porttitor litora! Eu quisque et ultricies ligula a venenatis sed odio in. Per auctor mollis sem sociis libero ut facilisi nam? Curabitur neque donec aliquam phasellus et neque sit lacus turpis eleifend sociis. Sociis dictum at auctor? Id penatibus urna sollicitudin egestas posuere posuere duis accumsan ligula elit porttitor. Nulla mi consectetur accumsan a orci dictum non tempus tellus ligula. Aliquet montes faucibus?", results.ParsedValues["line1"][0]);
            Assert.AreEqual("facilisi per aptent", results.ParsedValues["line2"][0]);
            Assert.AreEqual(". Tincidunt tempus gravida varius", results.ParsedValues["line2"][1]);
            Assert.AreEqual("Enim tristique nullam ", results.ParsedValues["line2"][2]);
            Assert.AreEqual("bibendum montes imperdiet tempor lacinia mauris lacinia quis natoque velit hac! Pulvinar amet dolor, nostra elit", results.ParsedValues["line4"][0]);
            Assert.AreEqual("vehicula", results.ParsedValues["line4"][1]);
            Assert.AreEqual(" Enim aliquet ", results.ParsedValues["line4"][2]);
        }
    }
}
