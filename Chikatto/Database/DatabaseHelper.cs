﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using User = Chikatto.Database.Models.User;

namespace Chikatto.Database
{
    public static class DatabaseHelper
    {
        private static string ConnectionString => $"server={Global.Config.DatabaseHost};database={Global.Config.DatabaseName};" +
                                                 $"user={Global.Config.DatabaseUser};password={Global.Config.DatabasePassword};";

        public static void Init()
        {
            Map(typeof(User));
            Map(typeof(Score));
            Map(typeof(DbChannel));
            Map(typeof(Beatmap));
        }

        private static void Map(Type type)
        {
            var map = new CustomPropertyTypeMap(typeof(User), 
                (t, columnName) => 
                    t.GetProperties().FirstOrDefault(prop => 
                        GetColumnFromAttribute(prop) == columnName));
            
            SqlMapper.SetTypeMap(typeof(User), map);
        }
        
        public static async Task<IEnumerable<T>> FetchAll<T>(string query, object param = null)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            return await connection.QueryAsync<T>(query, param);
        }

        public static async Task<T> FetchOne<T>(string query, object param = null)
        {
            var all = (await FetchAll<T>(query, param)).ToList();
            
            return all.Count == 0 ? default : all[0];
        }

        public static async Task Execute(string command, object param = null)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.ExecuteAsync(command, param);
        }
        
        private static string GetColumnFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute), false);
            return attrib?.Name;
        }
    }
}