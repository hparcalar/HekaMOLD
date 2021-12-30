using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Heka.DataAccess.Context;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using Heka.DataAccess.Migrations;

namespace Heka.DataAccess.Migration
{
    public class MigrationManager
    {
        public void MigrateUp()
        {
            var configuration = new Configuration();
            configuration.TargetDatabase = new DbConnectionInfo("HekaEntities");

            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
