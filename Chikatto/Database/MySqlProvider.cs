using System;
using Chikatto.Objects;
using MySql.Data.MySqlClient;

namespace Chikatto.Database
{
    public class MySqlProvider
    {
        public static MySqlConnection GetDbConnection() => new(Global.DbConnectionString); 
    }
}