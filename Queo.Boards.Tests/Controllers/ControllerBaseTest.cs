using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class ControllerBaseTest : ServiceBaseTest {
        /// <summary>
        ///     Subclasses must implement this property to return the locations of their
        ///     config files. A plain path will be treated as a file system location.
        /// </summary>
        /// <value>
        ///     An array of config locations
        /// </value>
        protected override string[] ConfigLocations {
            get {
                List<string> list = base.ConfigLocations.ToList();
                list.Add("assembly://Queo.Boards/Queo.Boards.Config/Spring.Controller.xml");
                list.Add("assembly://Queo.Boards/Queo.Boards.Config/Spring.Security.xml");
                return list.ToArray();
            }
        }
    }
}