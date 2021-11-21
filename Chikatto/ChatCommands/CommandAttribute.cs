using System;
using Chikatto.ChatCommands.Enums;
using Chikatto.Enums;

namespace Chikatto.ChatCommands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public Privileges Privileges;
        public CommandType Type;
        public string[] Triggers;
        public string Description;

        public CommandAttribute(string[] triggers, string description, Privileges privileges = Privileges.Normal, CommandType type = CommandType.Any)
        {
            Triggers = triggers;
            Description = description;
            Type = type;
            Privileges = privileges;
        }
    }
}