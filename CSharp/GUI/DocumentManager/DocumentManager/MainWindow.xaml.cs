using System.Windows;
using DocumentManager.DataSource;
using DocumentManager.OtherWindows;

namespace DocumentManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region プライベートメンバ
        /// <summary>
        /// データベースの操作用メンバ
        /// </summary>
        DocumentDB mDocDB;
        #endregion

        #region コンストラクタ
        public MainWindow()
        {
            InitializeComponent();

            mDocDB = new DocumentDB();
        }
        #endregion

        #region イベント

        #region メニュー → 新規作成クリック
        /// <summary>
        /// 新規ドキュメントの作成を行う。
        /// </summary>
        void menuNewCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCreateDoc = new DocCreateWindow();
                newCreateDoc.ShowDialog();
                if (newCreateDoc.DialogResult.Value)
                {
                    var doc = new Document(newCreateDoc.NewDocName);
                    mDocDB.AddNewDocument(doc);
                }
            }
#if !DEBUG
            catch { }
#endif
            finally { }
        }
        #endregion

        #endregion
    }
}
