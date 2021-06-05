﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CLOptionLib
{
    /// <summary>
    /// command line parser
    /// </summary>
    public sealed class CommandLineParser
    {
        #region private member
        Type _targetType;
        const string c_Prefix = "-";
        readonly string _pleaseHelp = $"\n\nPlease reference help that you specify \" {c_Prefix}h or {c_Prefix}help\"";
        Dictionary<string, string> _optValueDic;
        List<CommandLineAttr> _commandLineAttrList;
        #endregion

        #region property
        /// <summary>
        /// Arguments
        /// </summary>
        public IEnumerable<string> Args { get; }
        /// <summary>
        /// Did Command Line Error occur.
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        /// error message
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// exist help command
        /// </summary>
        public bool IsHelp
        {
            get
            {
                if (Args == null || Args.Any() == false) return false;
                return Args.Any(x => x == $"{c_Prefix}h" || x == $"{c_Prefix}help");
            }
        }
        /// <summary>
        /// help message
        /// </summary>
        public string HelpMessage
        {
            get
            {
                var sb = new StringBuilder();

                sb.AppendLine(getAssemblyInfo());

                sb.Append($"usage: {getAssemblyName()} ");
                var allAttrs = CommandLineAttrList;
                var reqOpts = allAttrs.Where(x => x.Required).Select(x => $"{c_Prefix}{x.CLOptionName}").ToList();
                if (reqOpts.Count != 0)
                {
                    foreach (var reqOpt in reqOpts)
                    {
                        sb.Append($"{reqOpt} {{ param }} ");
                    }
                }
                sb.AppendLine($"[ options ... ]");

                sb.AppendLine();

                foreach (var attr in allAttrs.OrderBy(x => x.CLOptionName.Length))
                {
                    sb.AppendLine($"{c_Prefix}{attr.CLOptionName}\t: {attr.HelpMessage}");
                }

                sb.AppendLine();

                sb.AppendLine("Display this message.");
                sb.AppendLine($"{c_Prefix}h");
                sb.AppendLine($"{c_Prefix}help");

                return sb.ToString();
            }
        }
        /// <summary>
        /// command line attr of target type
        /// </summary>
        List<CommandLineAttr> CommandLineAttrList
        {
            get
            {
                if (_commandLineAttrList != null) return _commandLineAttrList;
                _commandLineAttrList = getCLAttrs().ToList();
                return _commandLineAttrList;
            }
        }
        /// <summary>
        /// Key: option
        /// <para>Value: argument value</para>
        /// </summary>
        Dictionary<string, string> OptValueDic
        {
            get
            {
                if (_optValueDic != null) return _optValueDic;
                _optValueDic = new Dictionary<string, string>();
                var opts = CommandLineAttrList.Select(x => $"{c_Prefix}{x.CLOptionName}").ToList();
                var beforeOpt = string.Empty;
                foreach (var arg in Args)
                {
                    if (opts.Contains(arg))
                    {
                        // if options are duplicated, Exception occur.
                        _optValueDic.Add(arg, string.Empty);
                        beforeOpt = arg;
                    }
                    else
                    {
                        _optValueDic[beforeOpt] = arg;
                    }
                }
                return _optValueDic;
            }
        }
        #endregion

        #region Get assembly name
        /// <summary>
        /// Get assembly name
        /// </summary>
        /// <returns>assembly name of exe</returns>
        string getAssemblyName()
        {
            var assembly = Assembly.GetAssembly(_targetType);
            if (assembly == null) return string.Empty;
            return assembly.GetName().Name;
        }
        #endregion

        #region Get assembly info
        /// <summary>
        /// Get assembly info.
        /// <para>Tool Title</para>
        /// <para>Tool Version</para>
        /// <para>CompanyName</para>
        /// <para>LecalCopyright</para>
        /// <para>Comments(tool description)</para>
        /// </summary>
        /// <returns>string : {title version company legalCopyright \n comments}</returns>
        string getAssemblyInfo()
        {
            var assembly = Assembly.GetAssembly(_targetType);
            if (assembly == null) return string.Empty;

            var sb = new StringBuilder();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            // FileDescription is Title.
            sb.AppendLine($"{fvi.FileDescription} {fvi.FileVersion} {fvi.CompanyName} {fvi.LegalCopyright}");
            sb.AppendLine(fvi.Comments);

            return sb.ToString();
        }
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="targetType">Type with specified CommandLineTargetClass Attribute</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="Exception"/>
        public CommandLineParser(Type targetType)
        {
            _targetType = targetType;
            if (_targetType == null) throw new ArgumentNullException();
            if (isDefineCommandTargetAttr() == false) throw new Exception($"Type[{_targetType.Name}] does not have {nameof(CommandLineTargetClass)}.");

            Args = Environment.GetCommandLineArgs()[1..]; // First arg is EXE file name.

            IsError = chkCommadLine() == false;

            // implement checking args method after days ...

            setValueToPropAndField();
        }
        #endregion

        #region Check having CommandLineTargetClass Attribute
        /// <summary>
        /// check having CommandLineTargetClass Attribute.
        /// </summary>
        /// <returns>true: has CommandLineTargetClass, false: does not have CommandLineTargetClass</returns>
        bool isDefineCommandTargetAttr()
        {
            return _targetType?.GetCustomAttributes(typeof(CommandLineTargetClass), CommandLineTargetClass.Inherited).Length != 0;
        }
        #endregion

        #region Check Command Line Options
        /// <summary>
        /// Check Command Line Options.
        /// </summary>
        /// <returns>true: OK, false: NG</returns>
        bool chkCommadLine()
        {
            // check required option 
            if (chkRequired(CommandLineAttrList) == false) return false;

            return true;
        }
        #endregion

        #region Get Command Line Attrs
        /// <summary>
        /// Get Command Line Attrs of MemberInfo
        /// </summary>
        /// <returns>Command Line Attr Objects</returns>
        IEnumerable<CommandLineAttr> getCLAttrs()
        {
            foreach (var mem in getAllCommandLineMember())
            {
                yield return mem.GetCustomAttribute(typeof(CommandLineAttr), CommandLineAttr.Inherited) as CommandLineAttr;
            }
        }
        #endregion

        #region Get Command Line Member
        /// <summary>
        /// Get Command Line Member.
        /// </summary>
        /// <returns></returns>
        IEnumerable<MemberInfo> getAllCommandLineMember()
        {
            var memlist = new List<MemberInfo>();
            memlist.AddRange(_targetType.GetRuntimeFields());
            memlist.AddRange(_targetType.GetRuntimeProperties());

            foreach (var member in memlist)
            {
                if (member.GetCustomAttribute(typeof(CommandLineAttr), CommandLineAttr.Inherited) == null) continue;
                yield return member;
            }
        }
        #endregion

        #region check required option
        /// <summary>
        /// Check required option
        /// </summary>
        /// <param name="allCommandLineAttrs">All Command Line Attrs</param>
        /// <returns>true: OK, false: NG</returns>
        bool chkRequired(IEnumerable<CommandLineAttr> allCommandLineAttrs)
        {
            foreach (var optName in allCommandLineAttrs.Where(x => x.Required).Select(x => $"{c_Prefix}{x.CLOptionName}"))
            {
                if (Args.Contains(optName) == false)
                {
                    ErrorMessage = $"Error: Option {optName} is not specified.{_pleaseHelp}";
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region set value to property and field
        /// <summary>
        /// set value to property and field
        /// </summary>
        void setValueToPropAndField()
        {
            foreach (var mem in getAllCommandLineMember())
            {
                var option = $"{c_Prefix}{(mem.GetCustomAttribute(typeof(CommandLineAttr)) as CommandLineAttr).CLOptionName}";

                Type memType;
                if (mem is PropertyInfo pi)
                {
                    memType = pi.PropertyType;
                    if (tryGetOptionValue(memType, option, out var newVal) == false) continue;
                    pi.SetValue(null, newVal);
                }
                else if (mem is FieldInfo fi)
                {
                    memType = fi.FieldType;
                    if (tryGetOptionValue(memType, option, out var newVal) == false) continue;
                    fi.SetValue(null, newVal);
                }
            }
        }
        #endregion

        #region option value after cast type
        /// <summary>
        /// option value after cast type.
        /// </summary>
        /// <param name="memberType">member(property or field) type</param>
        /// <param name="option">option name(Key of OptValueDic)</param>
        /// <param name="val">out value</param>
        /// <returns>true: success getting, false: fail getting</returns>
        bool tryGetOptionValue(Type memberType, string option, out object val)
        {
            val = null;
            // implement other type after days ...
            if (memberType == typeof(int))
            {
                if (OptValueDic.TryGetValue(option, out var value) == false) return false;
                val = int.Parse(value);
            }
            else if (memberType == typeof(double))
            {
                if (OptValueDic.TryGetValue(option, out var value) == false) return false;
                val = double.Parse(value);
            }
            else if (memberType == typeof(string))
            {
                if (OptValueDic.TryGetValue(option, out var value) == false) return false;
                val = value;
            }
            else if (memberType == typeof(bool)) val = OptValueDic.TryGetValue(option, out _);
            else return false;

            return true;
        }
        #endregion
    }
}
