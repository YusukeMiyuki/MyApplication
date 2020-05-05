using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DocumentManager.DataSource
{
    /// <summary>
    /// 章を扱うクラス
    /// </summary>
    public class Chapter
    {
        #region DB情報
        /// <summary>
        /// DB上で識別するためのID
        /// </summary>
        public int ChapterID { get; private set; }
        /// <summary>
        /// ドキュメントからの紐づけ用ID
        /// </summary>
        public int ChpatersID { get; private set; }
        #endregion

        #region プライベートメンバ
        /// <summary>
        /// 子の章
        /// </summary>
        List<Chapter> mChapters = new List<Chapter>();
        #endregion

        #region パブリックメンバ
        /// <summary>
        /// 章タイトル
        /// </summary>
        public string HeadLine { get; private set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 章番号
        /// </summary>
        public double ChapterNumber { get; private set; }
        /// <summary>
        /// 子の章
        /// </summary>
        public IReadOnlyList<Chapter> Chapters => mChapters;
        #endregion

        public Chapter(string headline)
        {
            HeadLine = headline;
        }

        public void AddChapter(string headline) => mChapters.Add(new Chapter(headline));
    }
}
