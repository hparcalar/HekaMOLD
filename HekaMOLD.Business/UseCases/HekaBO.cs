using Heka.DataAccess.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class HekaBO
    {
        public bool RunMigrations()
        {
            MigrationManager migrationManager = new MigrationManager();
            return migrationManager.RunMigrations();
        }
    }
}
