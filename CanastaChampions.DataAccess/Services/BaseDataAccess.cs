using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace CanastaChampions.DataAccess.Services
{
    public abstract class BaseDataAccess
    {
        public static string DB_FILE = @"D:\Development\CanastaChampions\CanastaChampions.db";
        protected static string CONNECTION_STRING = @"DataSource=" + DB_FILE + ";Version=3";
        protected static SQLiteConnection _conn = null;
    }
}
