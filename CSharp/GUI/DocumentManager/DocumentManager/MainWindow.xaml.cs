using System.Windows;
using System.Windows.Controls;
using DocumentManager.DataSource;
using DocumentManager.OtherWindows;

namespace DocumentManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region パブリックメンバ
        /// <summary>
        /// データベースの操作用メンバ
        /// </summary>
        static public DocumentDB DocDB { get; private set; }

        static public DocOperateWindow DocWindow { get; private set; }
        #endregion

        #region コンストラクタ
        public MainWindow()
        {
            InitializeComponent();

            DocDB = new DocumentDB();

            DocWindow = new DocOperateWindow();
            DocWindow.Show();
        }
        #endregion

        #region イベント
        #endregion
    }
}
