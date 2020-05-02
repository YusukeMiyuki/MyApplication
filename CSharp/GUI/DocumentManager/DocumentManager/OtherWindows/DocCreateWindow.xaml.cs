using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocumentManager.OtherWindows
{
    /// <summary>
    /// DocCreateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DocCreateWindow : Window
    {
        public string NewDocName;

        public DocCreateWindow()
        {
            InitializeComponent();

            txtNewDocName.SetInputTextKind(WpfCustomControls.MyTextBox.TextBoxEx.TextKind.FileOrFolderName);

            txtNewDocName.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewDocName.Text))
                {
                    // エラー
                    MessageBox.Show("ドキュメント名が入力されていません。");
                    return;
                }
                NewDocName = txtNewDocName.Text;
                DialogResult = true;
                Close();
            }
            catch { }
            finally { }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try { Close(); }
            catch { }
            finally { }
        }

        private void txtNewDocName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                // e.Handled を true にすると文字が入力されないようになる
                e.Handled = txtNewDocName.CheckNGInput(e.Text);
            }
            catch { }
            finally { }
        }

        private void txtNewDocName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtNewDocName.CheckTextChanged();
            }
            catch { }
            finally { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NewDocName)) e.Cancel = true;
            }
            catch { }
            finally { }
        }
    }
}
