using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Tests.Builders.Services;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public class ServiceBaseTest : PersistenceBaseTest {
        
        protected CreateService CreateService { get; private set; }

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
                list.Add("assembly://Queo.Boards.Core/Queo.Boards.Core.Config/Spring.Services.xml");
                list.Add("assembly://Queo.Boards.Core/Queo.Boards.Core.Config/Spring.Validators.xml");
                return list.ToArray();
            }
        }

        [TestInitialize]
        public override void InitializeTest() {
            base.InitializeTest();
            CreateService = new CreateService(base.applicationContext);
        }
    }
}