using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DocumentManager.DataSource;


namespace DocumentManager
{
    /// <summary>
    /// データベースを操作するためのクラス
    /// </summary>
    public class DocumentDB
    {
        #region 内部クラス

        #region ドキュメントのDB情報クラス
        /// <summary>
        /// ドキュメントのテーブル名やカラム名を定数で扱うクラス
        /// </summary>
        class DocDBInfo
        {
            #region 定数
            /// <summary>
            /// ドキュメントテーブル名
            /// </summary>
            public const string C_DocumentTable = "document";

            #region カラム名
            public const string C_DocID = "DocID";
            public const string C_DocName = "DocName";
            public const string C_ChaptersID = "ChaptersID";
            #endregion

            #endregion
        }
        #endregion

        #region 章のDB情報クラス
        /// <summary>
        /// 章のテーブル名やカラム名を定数で扱うクラス
        /// </summary>
        class ChapterDBInfo
        {
            #region 定数
            /// <summary>
            /// 章テーブル
            /// </summary>
            public const string C_ChapterTable = "chapter";

            #region カラム名
            public const string C_ChapterID = "ChapterID";
            public const string C_ChaptersID = "ChaptersID";
            public const string C_ChapterNumber = "ChapterNumber";
            public const string C_ChapterName = "ChapterName";
            public const string C_Content = "Content";
            public const string C_AttachmentData = "AttachmentData";
            public const string C_TestItemID = "TestItemID";
            #endregion

            #endregion
        }
        #endregion

        #region テストアイテムのDB情報クラス
        /// <summary>
        /// テストアイテムのテーブル名やカラム名を定数で扱うクラス
        /// </summary>
        class TestItemDBInfo
        {
            #region 定数
            /// <summary>
            /// テストアイテムテーブル
            /// </summary>
            public const string C_TestItemTable = "testitem";

            #region カラム名
            public const string C_TestItemID = "TestItemID";
            public const string C_TestItemNumber = "TestItemNumber";
            public const string C_TestContent = "TestContent";
            public const string C_ExpectedValue = "ExpectedValue";
            public const string C_AttachmentData = "AttachmentData";
            #endregion

            #endregion
        }
        #endregion

        #endregion

        #region プライベートメンバ
        /// <summary>
        /// データベースまでのパス
        /// </summary>
        static string mDbPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0])), "db", "DocumentDb.sqlite");
        /// <summary>
        /// データベースへの接続用文字列
        /// </summary>
        string mDocumentDB;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ（内部で必要なテーブルを作成する）
        /// </summary>
        public DocumentDB()
        {
            if (File.Exists(mDbPath) == false)
            {
                var fs = File.Create(mDbPath);
                fs.Close(); // ここで閉じておかないと Open で例外出る
            }

            mDocumentDB = $"Data Source={mDbPath}";

            using (var db = new SQLiteConnection(mDocumentDB))
            {
                db.Open();
                using (var command = db.CreateCommand())
                {
                    // document Table 作成
                    var sb = new StringBuilder();
                    sb.Append($"CREATE TABLE IF NOT EXISTS {DocDBInfo.C_DocumentTable} (");
                    sb.Append($"{DocDBInfo.C_DocID} INTEGER NOT NULL PRIMARY KEY, ");  // 文書のID、本 Table の主キー
                    sb.Append($"{DocDBInfo.C_DocName} TEXT UNIQUE NOT NULL, ");        // 文書名
                    sb.Append($"{DocDBInfo.C_ChaptersID} INTEGER NOT NULL);");         // この文書に紐づく章のID（chapter Table の主キーじゃないので注意！）

                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();

                    // chapter Table 作成
                    // このテーブルでかくならない？
                    sb.Clear();
                    sb.Append($"CREATE TABLE IF NOT EXISTS {ChapterDBInfo.C_ChapterTable} (");
                    sb.Append($"{ChapterDBInfo.C_ChapterID} INTEGER NOT NULL PRIMARY KEY, "); // 章本体の固有なID (主キー)
                    sb.Append($"{ChapterDBInfo.C_ChaptersID} INTEGER NOT NULL, ");            // 文書からの紐づけ用ID
                    sb.Append($"{ChapterDBInfo.C_ChapterNumber} REAL NOT NULL, ");            // 章番号（これが並び替えの基本になる）
                    sb.Append($"{ChapterDBInfo.C_ChapterName} TEXT, ");                       // 章タイトル
                    sb.Append($"{ChapterDBInfo.C_Content} TEXT, ");                           // 内容
                    sb.Append($"{ChapterDBInfo.C_AttachmentData} BLOB, ");                    // 参考資料とかを直接添付する列
                    sb.Append($"{ChapterDBInfo.C_TestItemID} INTEGER);");                     // この章に紐づくテストのID

                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();

                    // testitem Table 作成
                    // こいつもでかくならない？
                    sb.Clear();
                    sb.Append($"CREATE TABLE IF NOT EXISTS {TestItemDBInfo.C_TestItemTable} (");
                    sb.Append($"{TestItemDBInfo.C_TestItemID} INTEGER NOT NULL PRIMARY KEY, "); // テストデータのID
                    sb.Append($"{TestItemDBInfo.C_TestItemNumber} REAL NOT NULL, ");            // 章番号がそのままくる（これが並び替えの基本になる）
                    sb.Append($"{TestItemDBInfo.C_TestContent} TEXT, ");                        // 試験内容
                    sb.Append($"{TestItemDBInfo.C_ExpectedValue} TEXT, ");                      // 期待値
                    sb.Append($"{TestItemDBInfo.C_AttachmentData} BLOB);");                     // テストデータとかを直接添付する列

                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region 新しいドキュメントをテーブルに追加する
        /// <summary>
        /// 新しいドキュメントをテーブルに追加する
        /// </summary>
        /// <param name="doc">ドキュメントオブジェクト</param>
        public void AddNewDocument(Document doc)
        {
            using (var db = new SQLiteConnection(mDocumentDB))
            {
                db.Open();
                using (var command = db.CreateCommand())
                {
                    int newDocID = getNextID(DocDBInfo.C_DocumentTable, DocDBInfo.C_DocID);

                    // ChapterDBInfo.C_ChaptersIDは複数ヒットするが、SQLiteのmax関数は最初にヒットしたものしか返さない、よって一つしか返ってこない
                    int newChaptersID = getNextID(ChapterDBInfo.C_ChaptersID, ChapterDBInfo.C_ChapterTable);

                    command.CommandText = $"insert into {DocDBInfo.C_DocumentTable} ({DocDBInfo.C_DocID}, {DocDBInfo.C_DocName}, {DocDBInfo.C_ChaptersID}) values ({newDocID}, '{doc.DocName}', {newChaptersID});";
                    try { command.ExecuteNonQuery(); }
                    catch (SQLiteException se) { MessageBox.Show(se.Message); }

                    doc.SetDocID(newDocID);
                    doc.SetChaptersID(newChaptersID);

                    #region (ローカル関数) 連番になるよう、次のID値を取得
                    int getNextID(string table, string column)
                    {
                        command.CommandText = $"select max({column}) from {table}";
                        using (var sdr = command.ExecuteReader())
                        {
                            // max関数を使ったsqlなので一つしか返ってこない. 新しくドキュメントを追加するので、++する
                            // maxが複数ヒットする場合、最初の一つしか返さない（SQLite独特らしい）
                            return (sdr.Read() && int.TryParse(sdr[$"max({column})"].ToString(), out var maxId)) ? ++maxId : 0;
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion
    }
}
