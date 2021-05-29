using System;

namespace CLOptionLib
{
    /// <summary>
    /// deal Command line option Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CommandLineAttr : Attribute
    {
        static bool? _inherited;

        /// <summary>
        /// CLOption Name
        /// </summary>
        public string CLOptionName { get; }
        /// <summary>
        /// Is Option Required
        /// </summary>
        public bool Required { get; }
        /// <summary>
        /// Help Message
        /// </summary>
        public string HelpMessage { get; }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clOptionName">Command line option name</param>
        /// <param name="required"> Is option required </param>
        /// <param name="helpMessage">Help message</param>
        public CommandLineAttr(string clOptionName, bool required, string helpMessage = null)
        {
            if (string.IsNullOrEmpty(clOptionName) || string.IsNullOrWhiteSpace(clOptionName))
                throw new ArgumentException("Can not set null, empty and whitespace for OptionName.");
            CLOptionName = clOptionName;
            Required = required;
            HelpMessage = helpMessage;
        }
    }
}
