using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoTaskManageer.DataSourceCS
{
    /// <summary>
    /// ToDoタスク自身のクラス
    /// </summary>
    class ToDoTask
    {
        #region プロパティ
        /// <summary>
        /// タスク名
        /// </summary>
        public string TaskName { get; private set; }
        /// <summary>
        /// 優先度
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 期日
        /// </summary>
        public DateTime DeadLine { get; private set; }
        /// <summary>
        /// 実行が完了したか
        /// </summary>
        public bool IsDone { get; private set; }
        /// <summary>
        /// 担当者
        /// </summary>
        public string PersonInCharge { get; private set; }
        #endregion

        #region コンストラクタ
        public ToDoTask(string taskName, int priority, DateTime deadLine, bool isDone = false, string person = null)
        {
            TaskName = taskName;
            Priority = priority;
            DeadLine = deadLine;
            SetIsDone(isDone);
            SetPersonInCharge(person);
        }
        #endregion

        #region タスク名変更
        /// <summary>
        /// タスク名の変更
        /// </summary>
        /// <param name="taskName">タスク名</param>
        /// <returns>変更できたかどうか</returns>
        public bool ChangeTaskName(string taskName)
        {
            if (string.IsNullOrEmpty(taskName))
            {

                return false;
            }
            TaskName = taskName;
            return true;
        }
        #endregion

        #region 優先度の変更
        /// <summary>
        /// 優先度変更
        /// </summary>
        /// <param name="priority">優先度</param>
        public void ChangePriority(int priority)
        {
            if (Priority == priority) return;
            Priority = priority;
        }
        #endregion

        #region 完了したかどうか
        /// <summary>
        /// 完了したかどうか
        /// </summary>
        /// <param name="isDone">true; 完了、false; 未完了</param>
        public void SetIsDone(bool isDone) => IsDone = isDone;
        #endregion

        #region 担当者の変更
        /// <summary>
        /// 担当者の変更
        /// </summary>
        /// <param name="person">担当者名</param>
        public void SetPersonInCharge(string person)
        {
            if (string.IsNullOrEmpty(person)) PersonInCharge = Environment.UserName; // 既定は実行ユーザ
            else PersonInCharge = person;
        }
        #endregion
    }
}
