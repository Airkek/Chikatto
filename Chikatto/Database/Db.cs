using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Dapper;
using MySql.Data.MySqlClient;

namespace Chikatto.Database
{
    public static class Db
    {
        private static string ConnectionString => $"server={Global.Config.Database.Host};database={Global.Config.Database.Database};" +
                                                  $"user={Global.Config.Database.User};password={Global.Config.Database.Password};";

        public static void Init()
        {
            Assembly.GetEntryAssembly().GetTypes()
                .Where(t => t.GetProperties()
                    .Any(x => x.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0)
                ).ToList().ForEach(Map);
        }

        private static void Map(Type type)
        {
            var map = new CustomPropertyTypeMap(type, 
                (t, columnName) => 
                    t.GetProperties().FirstOrDefault(prop => 
                        GetColumnFromAttribute(prop) == columnName));
            
            SqlMapper.SetTypeMap(type, map);
        }
        
        public static async Task<IEnumerable<T>> FetchAll<T>(string query, object param = null)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            return await connection.QueryAsync<T>(query, param);
        }

        public static async Task<T> FetchOne<T>(string query, object param = null)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(query, param);
        }

        public static async Task<int> Execute(string command, object param = null)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            return await connection.ExecuteAsync(command, param);
        }
        
        private static string GetColumnFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute), false);
            return attrib?.Name;
        }
    }
}