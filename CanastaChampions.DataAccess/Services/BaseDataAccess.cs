using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace CanastaChampions.DataAccess.Services
{
    public abstract class BaseDataAccess
    {
        protected static string CONNECTION_STRING = @"DataSource=D:\Development\CanastaChampions\CanastaChampions.db;Version=3";
        protected static SQLiteConnection _conn = null;
    }
}
