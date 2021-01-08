using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UGDR
{
    class udb
    {
        static int use = 0;
        public static void save(string sql)
        {
            if (use == 0)
            {
                use = 1;
                string connStr = @"Data Source=Ingrang.db";

                var con = new SQLiteConnection(connStr);
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                use = 0;
            }
        }
    }
}
