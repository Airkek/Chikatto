using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Chikatto.Controllers
{
    public class RegistrationController : Controller
    {
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-[ ]";

        private static readonly Regex EmailRegex = new( //http://www.regular-expressions.info/email.html
            @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            RegexOptions.Compiled);
        
        [HttpPost]
        [Route("/users")]
        public async Task<IActionResult> Register()
        {
            if (!Global.Config.AllowRegistrations)
            {
                return UnprocessableEntity(
                    "{'form_error': {'user':{'username':['In-game registrations is not allowed. Please, try again later.']}}}");
            }

            var errors = new Dictionary<string, List<string>>
            {
                ["username"] = new(),
                ["user_email"] = new(),
                ["password"] = new()
            };
            
            var username = (string) Request.Form["user[username]"];
            var email = Request.Form["user[user_email]"].ToString().ToLower(); // email should be lowercase, i guess
            var password = (string) Request.Form["user[password]"];

            var safe = Auth.GetSafeName(username);
            
            if(username.StartsWith(" ") || username.EndsWith(" "))
                errors["username"].Add("Username can't start or end with spaces!");
            
            if(username.Length < 3)
                errors["username"].Add("The requested username is too short.");
            else if(username.Length > 15)
                errors["username"].Add("The requested username is too long.");

            if(username.Contains("_") && username.Contains(" "))
                errors["username"].Add("Please use either underscores or spaces, not both!");
            
            if(username.Any(x => !AllowedChars.Contains(x)))
                errors["username"].Add("The requested username contains invalid characters.");
            
            if(username.Contains("[]"))
                errors["username"].Add("This username choice is not allowed."); // peppy why? https://i.imgur.com/IyCO4AT.png
            
            if(password.Length < 8)
                errors["password"].Add("Password is too short.");
            else if(password.Length > 32)
                errors["password"].Add("Password is too long.");
            
            if(password.Distinct().Count() < 3)
                errors["password"].Add("Password must containt at least 3 unique characters.");
            
            if(!EmailRegex.IsMatch(email))
                errors["user_email"].Add("Please, provide valid email address.");

            if (errors.All(x => x.Value.Count == 0))
            {
                var exists = await Db.FetchOne<User>("SELECT username_safe, email FROM users WHERE username_safe = @safe OR email = @email",
                    new {safe, email});

                if (exists is not null)
                {
                    if (exists.SafeName == safe)
                        errors["username"].Add("This username is already in use.");
                    
                    if (exists.Email == email)
                        errors["user_email"].Add("This email is already in use.");
                }
            }
            
            if (errors.Any(x => x.Value.Count > 0))
                return UnprocessableEntity("{'form_error': {'user':" + JsonConvert.SerializeObject(errors) + "}}");

            if (Request.Form["check"] == "0")
            {
                using var md5 = MD5.Create();
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                var md5Pw = BitConverter.ToString(data).ToLower().Replace("-", "");
                var hashPw = BCrypt.Net.BCrypt.HashPassword(md5Pw);

                Global.BCryptCache[hashPw] = md5Pw;

                await Db.Execute(@"INSERT INTO users (privileges, salt, username, username_safe, email, password_md5, password_version, register_datetime, latest_activity)
                             VALUES (@privs, '', @username, @safe, @email, @hashPw, 2, UNIX_TIMESTAMP(), UNIX_TIMESTAMP())",
                    new{ privs = Privileges.PendingVerification, username, safe, email, hashPw });

                var id = await Db.FetchOne<int>("SELECT id FROM users WHERE username_safe = @safe", new {safe});

                await Db.Execute("INSERT INTO users_stats (id, username) VALUES (@id, @username)", new { id, username });
                await Db.Execute("INSERT INTO users_stats_relax (id, username) VALUES (@id, @username)", new { id, username });
                
                if (id == 1000)
                {
                    //give all permissions to first registered user
                    await Db.Execute("UPDATE users SET privileges = @privs WHERE id = 1000",
                        new {privs = Privileges.Public | Privileges.Normal | Privileges.Donor | Privileges.Owner | Privileges.PendingVerification});
                }
            
                XConsole.Log($"<{username} ({id})> has registered!", back: ConsoleColor.Green);
            }

            return Ok("ok");
        }
    }
}