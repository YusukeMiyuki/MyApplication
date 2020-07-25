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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrthogonalTable
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        int mCount = 0;
        public ObservableCollection<Test> Tests = new ObservableCollection<Test>();

        public class Test
        {
            public string Header;
        }

        public UserControl1()
        {
            InitializeComponent();
            //dgFactorTable.ItemsSource = Tests;
        }

        private void btnColumnChange_Click(object sender, RoutedEventArgs e)
        {
            var win = new ColumnEditWindow();
            win.Show();
            //mCount++;
            //var header = $"Header{mCount}";
            //var ss = new Test
            //{
            //    Header = header
            //};
            ////for (int i = 0; i < mCount; i++)
            ////{
            ////    ss.Rows.Add($"Test{i}");
            ////}

            ////var maxRowCou = mHeaderFactorDic.Max(x => x.Value.Count);
            ////foreach (var rows in mHeaderFactorDic)
            ////{
            ////    if (rows.Value.Count == maxRowCou) continue;
            ////    for (int i = 0; i < maxRowCou - rows.Value.Count; i++)
            ////    {
            ////        rows.Value.Add(string.Empty);
            ////    }
            ////}

            //var textcol = new DataGridTextColumn();
            //var b = new Binding("Header");
            //textcol.Header = header;
            //textcol.Binding = b;
            //dgFactorTable.Columns.Add(textcol);
            //Tests.Add(ss);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            var ssssss = dgFactorTable.Columns;
        }
    }
}
