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
        public string newDocName;

        public DocCreateWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewDocName.Text))
                {
                    // エラー
                    return;
                }

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

        }
    }
}
