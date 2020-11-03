using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Infrastructure.Templating;
using Queo.Boards.Core.Infrastructure.Templating.DotLiquid;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public class DotLiquidRenderContextTest {
        /// <summary>
        ///     Testet das Ersetzen eines simplen Templates
        /// </summary>
        [TestMethod]
        public void TestParseAndRender() {
            /* Given: Ein einfacher String mit einem Platzhalter */
            const string TEMPLATE = "Das ist ein {{ value }} Template";
            const string PLACE_HOLDER_VALUE = "einfaches";
            string expectedString = TEMPLATE.Replace("{{ value }}", PLACE_HOLDER_VALUE);
            ModelMap model = new ModelMap();
            model.Add("value", PLACE_HOLDER_VALUE);
            /* When: Das Template mit dem Platzhalter gefüllt werden soll */
            string result = new DotLiquidRenderContext().ParseAndRender(TEMPLATE, model);

            /* Then: Muss der Platzhalter korrekt ersetzt werden. */
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void TestParseAndRenderTemplate() {
            /* Given: Ein einfacher String mit einem Platzhalter */
            const string TEMPLATE = "Das ist ein {{ foo.Description }} Template";
            const string PLACE_HOLDER_VALUE = "einfaches";
            Foo foo = new Foo(PLACE_HOLDER_VALUE);
            string expectedString = TEMPLATE.Replace("{{ foo.Description }}", PLACE_HOLDER_VALUE);
            ModelMap model = new ModelMap();
            model.Add("foo", foo);
            /* When: Das Template mit dem Platzhalter gefüllt werden soll */
            ITemplate template = new DotLiquidRenderContext().Parse(TEMPLATE);
            string result = template.Render(model);

            /* Then: Muss der Platzhalter korrekt ersetzt werden. */
            Assert.AreEqual(expectedString, result);
        }
    }

    public class Foo {
        private string _description;

        public Foo(string description) {
            _description = description;
        }

        public string Description {
            get { return _description; }
            set { _description = value; }
        }
    }
}