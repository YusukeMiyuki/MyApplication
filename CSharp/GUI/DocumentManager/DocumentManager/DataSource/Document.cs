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
        List<Chapter> mChapters;
        #endregion

        #region パブリックプロパティ
        /// <summary>
        /// ドキュメント名
        /// </summary>
        public string DocName { get; private set; }
        /// <summary>
        /// 章
        /// </summary>
        public IReadOnlyList<Chapter> Chapters => mChapters;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="docName">ドキュメント名</param>
        public Document(string docName)
        {
            DocName = docName;
            mChapters = new List<Chapter>();
        }
        #endregion

        #region DB情報セット

        #region ドキュメントIDをセット
        /// <summary>
        /// ドキュメントIDをセットする
        /// </summary>
        /// <param name="id">DBから取得、または作成したID</param>
        public void SetDocID(int id) => DocID = id;
        #endregion

        #region 章IDのセット
        /// <summary>
        /// 本ドキュメントに紐づく章のIDをセットする
        /// </summary>
        /// <param name="id">DBから取得、または作成したID</param>
        public void SetChaptersID(int id) => ChaptersID = id;
        #endregion

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
