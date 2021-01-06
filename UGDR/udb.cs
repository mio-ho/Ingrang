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
        void save()
        {
            if (use == 0)
            {
                string connStr = @"Data Source=Ingrang.db";

                var con = new SQLiteConnection(connStr);
                con.Open();


                con.Close();
            }
        }
    }
}
