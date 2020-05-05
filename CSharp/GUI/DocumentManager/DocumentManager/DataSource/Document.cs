using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.DataSource
{
    /// <summary>
    /// 文書の情報を扱う
    /// </summary>
    public class Document
    {
        #region DB情報
        /// <summary>
        /// DB上のドキュメントID
        /// </summary>
        public int DocID { get; private set; }
        /// <summary>
        /// DB上の本ドキュメントに紐づく章のID
        /// </summary>
        public int ChaptersID { get; private set; }
        #endregion

        #region プライベートメンバ
        string mInitDocName;
        List<Chapter> mChapters;
        #endregion

        #region パブリックプロパティ
        /// <summary>
        /// ドキュメント名
        /// </summary>
        public string DocName { get; set; }
        /// <summary>
        /// 章
        /// </summary>
        public IReadOnlyList<Chapter> Chapters => mChapters;

        public bool IsChangeDirName => DocName != mInitDocName;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="docName">ドキュメント名</param>
        public Document(string docName)
        {
            DocName = docName;
            mInitDocName = docName;
            mChapters = new List<Chapter>();
        }
        #endregion

        #region DB情報セット
        /// <summary>
        /// DB情報をセットする
        /// </summary>
        public void SetDBInfo(int docId, int chaptersId)
        {
            DocID = docId;
            ChaptersID = chaptersId;
        }
        #endregion

        #region 章の追加
        /// <summary>
        /// 章を追加する
        /// </summary>
        /// <param name="headline">章タイトル</param>
        public void AddChapter(string headline) => mChapters.Add(new Chapter(headline));
        #endregion
    }
}
