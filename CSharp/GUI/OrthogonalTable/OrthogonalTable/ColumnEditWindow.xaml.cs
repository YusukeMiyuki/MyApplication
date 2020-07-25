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
using System.Windows.Shapes;

namespace OrthogonalTable
{
    /// <summary>
    /// ColumnEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ColumnEditWindow : Window
    {
        public ObservableCollection<HeaderName> HeaderNames = new ObservableCollection<HeaderName>();

        public class HeaderName
        {
            public string Header;
        }


        public ColumnEditWindow()
        {
            InitializeComponent();
            dgColumn.ItemsSource = HeaderNames;
            HeaderNames.Add(new HeaderName());
        }

        private void btnHanei_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtColumnNum.Text, out var colNum) == false) return;

            var now = dgColumn.Columns.Count;

            for (int i = now; i < colNum; i++)
            {
                var textcol = new DataGridTextColumn();
                textcol.Header = $"{i}";
                dgColumn.Columns.Add(textcol);
            }
        }
    }
}
