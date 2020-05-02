using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentManager.DataSource;
using DocumentManager.OtherWindows;

namespace DocumentManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        void menuNewCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCreateDoc = new DocCreateWindow();
                newCreateDoc.ShowDialog();
                if (newCreateDoc.DialogResult.Value)
                {
                    var doc = new Document(newCreateDoc.NewDocName);

                }
            }
#if DEBUG
            catch { }
#endif
            finally { }
        }
    }
}
