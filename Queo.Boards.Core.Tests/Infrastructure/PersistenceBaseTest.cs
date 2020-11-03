using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Tests.Builders;
using Spring.Data.NHibernate;
using Spring.Testing.Microsoft;
using Spring.Transaction;
using Spring.Transaction.Support;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public class PersistenceBaseTest : AbstractDependencyInjectionSpringContextTests {
        private HibernateTransactionManager _hibernateTransactionManager;
        private IDictionary<string, object> _objectsOfType;
        private ITransactionStatus _transactionStatus;
        
        protected Create Create { get; private set; }

        /// <summary>
        ///     Subclasses must implement this property to return the locations of their
        ///     config files. A plain path will be treated as a file system location.
        /// </summary>
        /// <value>
        ///     An array of config locations
        /// </value>
        protected override string[] ConfigLocations {
            get {
                return new[] {
                    "assembly://Queo.Boards.Core.Tests/Queo.Boards.Core.Tests.Config/Spring.Database.xml",
                    "assembly://Queo.Boards.Core.Tests/Queo.Boards.Core.Tests.Config/Spring.Test.xml",
                    "assembly://Queo.Boards.Core/Queo.Boards.Core.Config/Spring.Infrastructure.xml",
                    "assembly://Queo.Boards.Core/Queo.Boards.Core.Config/Spring.Persistence.xml",
                };
            }
        }

        [TestCleanup]
        public virtual void CleanUp() {
            _hibernateTransactionManager.Rollback(_transactionStatus);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            //base.TestInitialize();
            _objectsOfType = applicationContext.GetObjectsOfType(typeof(FluentSessionFactory), true, true);
            _hibernateTransactionManager =
                    (HibernateTransactionManager)applicationContext.GetObject("applicationTransactionManager");
            DefaultTransactionDefinition def = new DefaultTransactionDefinition();

            _transactionStatus = _hibernateTransactionManager.GetTransaction(def);
            FluentSessionFactory factoryObject = (FluentSessionFactory)_objectsOfType["&applicationSessionFactory"];
            // ReSharper disable once CSharpWarnings::CS0618   korrekte Verwendung da zur Erzeugung des Schemas verwendet.
            //FluentSessionFactory factoryObject = FluentSessionFactory.FactoryObject;
            if (factoryObject == null) {
                throw new ApplicationException(
                        "TestLocalSessionFactoryObject nicht vorhanden. Fehler in der Konfiguration der SessionFactory?");
            }
            
            Create = new Create(base.applicationContext);
            CreateTransient = new Create(null);

            // factoryObject.CreateDatabaseSchema();
        }

        public Create CreateTransient { get; private set; }
    }
}