using System;
using System.IO;
using Queo.Boards.Core.Domain;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Inspections;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace DbSchemaCreator {
    class Program {
        private const string FILENAME = "..\\..\\..\\Queo.Boards.Core\\Database\\db_ddl.sql";

        private static void BuildSchema(Configuration obj) {
            TextWriter textWriter = new StringWriter();
            new SchemaExport(obj).SetOutputFile(FILENAME).Execute(x => Console.WriteLine(textWriter.ToString()), false, false, textWriter);
        }

        private static void Main(string[] args) {
            try
            {
                Fluently.Configure()
                            .Database(MsSqlConfiguration.MsSql2012)
                            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>()
                                    .Conventions.Add(Table.Is(x => "tbl" + x.EntityType.Name))
                                    .Conventions.Add(DefaultAccess.CamelCaseField(CamelCasePrefix.Underscore))
                                    .Conventions.Add(ForeignKey.EndsWith("_Id")))
                            .ExposeConfiguration(BuildSchema).BuildConfiguration();
            }
            catch (Exception ex)
            {

                throw;
            }

            Console.WriteLine("enter for exit");
            Console.ReadLine();
        }
    }
}