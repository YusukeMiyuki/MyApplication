using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentManager.OtherWindows
{
    /// <summary>
    /// DocCreateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DocCreateWindow : Window
    {
        #region パブリックプロパティ
        /// <summary>
        /// 新しく作成するドキュメントの名前
        /// </summary>
        public string NewDocName { get; private set; }
        #endregion

        #region コンストラクタ
        public DocCreateWindow()
        {
            InitializeComponent();

            // テキストボックスの種類を設定
            txtNewDocName.SetInputTextKind(WpfCustomControls.MyTextBox.TextBoxEx.TextKind.FileOrFolderName);
            // フォーカスをテキストボックスにあてておく
            txtNewDocName.Focus();
        }
        #endregion

        #region イベント

        #region OKボタン
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewDocName.Text))
                {
                    // エラー
                    MessageBox.Show("ドキュメント名が入力されていません。");
                    txtNewDocName.Focus();
                    return;
                }
                NewDocName = txtNewDocName.Text;
                DialogResult = true;
            }
#if !DEBUG
            catch (Exception ee) { MessageBox.Show("予期せぬエラーが発生しました。"); }
#endif
            finally { }
        }
        #endregion

        #region キャンセルボタン
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try { Close(); }
#if !DEBUG
            catch (Exception ee) { MessageBox.Show("予期せぬエラーが発生しました。"); }
#endif
            finally { }
        }
        #endregion

        #region 文字入力時
        private void txtNewDocName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                // e.Handled を true にすると文字が入力されないようになる
                e.Handled = txtNewDocName.CheckNGInput(e.Text);
            }
#if !DEBUG
            catch (Exception ee) { MessageBox.Show("予期せぬエラーが発生しました。"); }
#endif
            finally { }
        }
        #endregion

        #region 入力値変更時
        private void txtNewDocName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { txtNewDocName.CheckTextChanged(); }
            catch { }
            finally { }
        }
        #endregion

        #endregion
    }
}
