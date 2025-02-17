using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PropertyObjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NoSortAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CallPriorityAttribute : Attribute
    {
        public CallPriorityAttribute(int priority) => Priority = priority;

        public int Priority { get; set; }
    }

    public class CallPriorityComparer : IComparer<MethodInfo>
    {
        public int Compare(MethodInfo x, MethodInfo y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            var xPriority = GetPriority(x);
            var yPriority = GetPriority(y);

            if (xPriority > yPriority)
            {
                return 1;
            }

            if (xPriority < yPriority)
            {
                return -1;
            }

            return 0;
        }

        private int GetPriority(MethodInfo mi)
        {
            var objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);

            if (objs.Length == 0)
            {
                return 50;
            }

            if (!(objs[0] is CallPriorityAttribute attr))
            {
                return 50;
            }

            return attr.Priority;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TypeAliasAttribute : Attribute
    {
        public TypeAliasAttribute(params string[] aliases) => Aliases = aliases;

        public string[] Aliases { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ParsableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class CustomEnumAttribute : Attribute
    {
        public CustomEnumAttribute(string[] names) => Names = names;

        public string[] Names { get; }
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructibleAttribute : Attribute
    {
        public ConstructibleAttribute() :
            this(AccessLevel.Player) // Lowest accesslevel for current functionality (Level determined by access to [add)
        {
        }

        public ConstructibleAttribute(AccessLevel accessLevel) => AccessLevel = accessLevel;

        public AccessLevel AccessLevel { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommandPropertyAttribute : Attribute
    {
        public CommandPropertyAttribute(
            AccessLevel level,
            bool readOnly = false,
            bool canModify = false
        ) : this(level, level)
        {
            ReadOnly = readOnly;
            CanModify = canModify;
        }

        public CommandPropertyAttribute(AccessLevel readLevel, AccessLevel writeLevel)
        {
            ReadLevel = readLevel;
            WriteLevel = writeLevel;
        }

        public AccessLevel ReadLevel { get; }
        public AccessLevel WriteLevel { get; }
        public bool ReadOnly { get; }
        public bool CanModify { get; }
    }
}
