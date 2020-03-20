using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClassLib.EnvironmentInfo
{
    /// <summary>
    /// 環境変数やマシーン、ログイン者などの環境全般を扱うクラス
    /// </summary>
    public static class EnvironmentInfo
    {
        #region CPUの最大スレッド数の取得
        static int? mMaxThread;
        /// <summary>
        /// 最大スレッド数を取得する
        /// </summary>
        public static int MaxThread
        {
            get
            {
                if (mMaxThread.HasValue) return mMaxThread.Value;
                ThreadPool.GetMinThreads(out var maxThread, out _);
                mMaxThread = maxThread;
                return mMaxThread.Value;
            }
        }
        #endregion

        #region 環境変数の値を取得する
        /// <summary>
        /// 指定した環境変数の値を取得する
        /// </summary>
        /// <param name="envVarName">環境変数名</param>
        /// <param name="value">環境変数の値</param>
        /// <returns>取得に成功したかどうか</returns>
        public static bool GetEnvironmentVariable(string envVarName, out string value)
        {
            try
            {
                value = Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.User);
                if (value == null) value = Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.Machine); // 権限がないと例外発生
                if (value == null)
                {
                    value = string.Empty;
                    return false;
                }
            }
            catch 
            { 
                value = string.Empty;
                return false;
            }
            return true;
        }
        #endregion
    }
}
