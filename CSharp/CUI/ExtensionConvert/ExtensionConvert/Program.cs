using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ExtensionConvert
{
    #region 列挙
    /// <summary>
    /// 変換先の列挙
    /// </summary>
    enum Extension
    {
        TSV,
        CSV
    }
    #endregion

    class Program
    {
        #region プライベートメンバ
        static string path;
        static Extension convertExt;
        const string c_VsCodePath = @"C:\Users\akada\AppData\Local\Programs\Microsoft VS Code\Code.exe";
        #endregion

        #region Main
        static void Main(string[] args)
        {
            commandParser(args);
            var newPath = getCopyPath();
            File.Copy(args[0], newPath, true);

            var psi = new ProcessStartInfo(c_VsCodePath)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = newPath
            };
            Process.Start(psi);
        }
        #endregion

        #region コマンドラインパーサー
        /// <summary>
        /// コマンドパーサー（これいる？）
        /// </summary>
        /// <param name="args">引数</param>
        static void commandParser(string[] args)
        {
            path = args[0];
            convertExt = args[1] switch
            {
                "-tsv" => Extension.TSV,
                "-csv" => Extension.CSV,
                _ => throw new InvalidOperationException()
            };
        }
        #endregion

        #region コピー先のファイルパス
        /// <summary>
        /// 新しいコピー先のパスを取得
        /// </summary>
        /// <returns>新しいコピー先のファイルパス</returns>
        static string getCopyPath()
        {
            var regNum = new Regex($@"{Path.GetFileNameWithoutExtension(path)}_copy[\d]*\.(csv|tsv)$");
            var name = Path.GetFileNameWithoutExtension(path);
            var retPath = convertExt switch
            {
                Extension.TSV => searchPath($"{Path.Combine(Path.GetDirectoryName(path), name)}_copy.tsv"),
                Extension.CSV => searchPath($"{Path.Combine(Path.GetDirectoryName(path), name)}_copy.csv"),
                _ => throw new InvalidOperationException()
            };
            return retPath;

            #region (ローカル関数) コピー可能なファイル名探索
            string searchPath(string path)
            {
                for (int i = 1; File.Exists(path); i++)
                {
                    path = regNum.Replace(path, $"{name}_copy{i}.tsv");
                }
                return path;
            }
            #endregion
        }
        #endregion
    }
}
