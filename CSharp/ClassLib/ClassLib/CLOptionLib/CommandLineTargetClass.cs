using System;

namespace CLOptionLib
{
    /// <summary>
    /// this attribute is used by developers to check Command Line Attributes for properties or fields in Class..
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandLineTargetClass : Attribute
    {
        static bool? _inherited;

        /// <summary>
        /// Is Inherited Property of AttributeUsage
        /// </summary>
        public static bool Inherited
        {
            get
            {
                if (_inherited != null) return _inherited.Value;
                var attr = typeof(CommandLineAttr).GetCustomAttributes(typeof(AttributeUsageAttribute), true)[0] as AttributeUsageAttribute;
                _inherited = attr.Inherited;
                return _inherited.Value;
            }
        }
    }
}
