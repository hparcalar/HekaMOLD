﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Heka.DataAccess.Migration
{
    public class MigrationManager
    {
        public bool RunMigrations()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Migration"].ConnectionString))
                {
                    con.Open();

                    DataTable dtVersions = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM DbMigration", con);
                    adapter.Fill(dtVersions);
                    adapter.Dispose();

                    string lastMigratedVersion = "";
                    if (dtVersions.Rows.Count > 0)
                        lastMigratedVersion = dtVersions.Rows[dtVersions.Rows.Count - 1]["Version"].ToString();

                    var assembly = Assembly.GetAssembly(typeof(MigrationManager));
                    var versionScripts = assembly.GetManifestResourceNames()
                        .Where(d => d.EndsWith(".sql") && string.Compare(d.Replace("Heka.DataAccess.Migration.Versions.",""), (lastMigratedVersion + ".sql")) == 1)
                        .OrderBy(d => d);

                    foreach (var script in versionScripts)
                    {
                        StreamReader sReader = new StreamReader(assembly.GetManifestResourceStream(script));
                        string[] sqlSections = Regex.Split(sReader.ReadToEnd(), "GO\\n");

                        // EXECUTE SECTIONS
                        foreach (var sqlScript in sqlSections)
                        {
                            SqlCommand cmd = new SqlCommand(sqlScript, con);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        // WRITE LOG TO DbMigration
                        SqlCommand vCmd = new SqlCommand("INSERT INTO DbMigration(Version, UpdateDate) VALUES('"+
                            script.Replace("Heka.DataAccess.Migration.Versions.","").Replace(".sql","") +"', GETDATE())", con);
                        vCmd.ExecuteNonQuery();
                    }

                    con.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}