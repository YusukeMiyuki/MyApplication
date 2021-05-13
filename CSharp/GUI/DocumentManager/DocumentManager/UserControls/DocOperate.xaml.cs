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
using WpfCustomControls.MyTextBox;

namespace DocumentManager.UserControls
{
    /// <summary>
    /// DocOperate.xaml の相互作用ロジック
    /// </summary>
    public partial class DocOperate : UserControl
    {
        #region プライベートメンバ
        SortedDictionary<int, Document> mDocDic { get; } = new SortedDictionary<int, Document>();
        SortedDictionary<int, Directory> mAllDirDic { get; } = new SortedDictionary<int, Directory>();

        object mSelectedItem;
        #endregion

        #region パブリックプロパティ
        public ObservableCollection<Directory> Directories { get; private set; }
        /// <summary>
        /// 新しく作成するドキュメントの名前
        /// </summary>
        public string NewDocName { get; private set; }
        #endregion

        public DocOperate()
        {
            InitializeComponent();
        }

        public void Init()
        {
            Directories = new ObservableCollection<Directory>();
            tvDirDoc.ItemsSource = Directories;

            // Directoryの取得
            foreach (var dir in MainWindow.DocDB.GetDirectoryList())
            {
                if (dir.IsTop) Directories.Add(dir);
                if (mAllDirDic.ContainsKey(dir.DirID) == false) mAllDirDic.Add(dir.DirID, dir);
            }

            // 全ドキュメントの取得
            var docIdList = mAllDirDic.Values.SelectMany(x => x.DocIdList).Distinct().ToList();
            foreach (var doc in MainWindow.DocDB.GetDocumentList(docIdList))
            {
                if (mDocDic.ContainsKey(doc.DocID) == false) mDocDic.Add(doc.DocID, doc);
            }

            // デバッグ用
            //var dir0 = new Directory("f0");
            //dir0.SetInitDBInfo(0, 1, "0", "1,2");
            //mAllDirDic.Add(0, dir0);

            //var dir1 = new Directory("f1");
            //dir1.SetInitDBInfo(1, 0, "1,2", "");
            //mAllDirDic.Add(1, dir1);

            //var dir2 = new Directory("f2");
            //dir2.SetInitDBInfo(2, 0, "", "");
            //mAllDirDic.Add(2, dir2);

            //var doc0 = new Document("d0");
            //doc0.SetDBInfo(0, 0);
            //mDocDic.Add(0, doc0);

            //var doc1 = new Document("d1");
            //doc1.SetDBInfo(1, 1);
            //mDocDic.Add(1, doc1);

            //var doc2 = new Document("d2");
            //doc2.SetDBInfo(2, 2);
            //mDocDic.Add(2, doc2);

            //Directories.Add(dir0);
        }

        #region イベント

        #region 文字入力時
        private void txtEx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                // e.Handled を true にすると文字が入力されないようになる
                //e.Handled = txtNewDocName.CheckNGInput(e.Text);
            }
#if !DEBUG
            catch (Exception ee) { MessageBox.Show("予期せぬエラーが発生しました。"); }
#endif
            finally { }
        }
        #endregion

        #region 入力値変更時
        private void txtEx_TextChanged(object sender, TextChangedEventArgs e)
        {
            //try { txtNewDocName.CheckTextChanged(); }
            //catch { }
            //finally { }
        }
        #endregion

        #endregion

        #region 文書の取得
        public List<Document> GetDocumentList(List<int> docIdList)
        {
            var docList = new List<Document>();
            foreach (var id in docIdList)
            {
                if (mDocDic.TryGetValue(id, out var doc)) docList.Add(doc);
            }
            return docList;
        }
        #endregion

        #region ディレクトリの取得
        public List<Directory> GetDirectoryList(List<int> dirIdList)
        {
            var dirList = new List<Directory>();
            foreach (var id in dirIdList)
            {
                if (mAllDirDic.TryGetValue(id, out var dir)) dirList.Add(dir);
            }
            return dirList;
        }
        #endregion

        private void miNewDir_Click(object sender, RoutedEventArgs e)
        {
            if (mSelectedItem != null && !(mSelectedItem is Directory)) return;

            var nextId = mAllDirDic.Any() ? mAllDirDic.Keys.Last() + 1 : 0;
            var dir = new Directory($"");
            mAllDirDic.Add(nextId, dir);

            if (mSelectedItem != null)
            {
                (mSelectedItem as Directory).UpdateChildDirIdList(new List<int> { nextId });
                dir.SetInitDBInfo(nextId, 0, "", "");
            }
            else
            {
                Directories.Add(dir);
                dir.SetInitDBInfo(nextId, 1, "", "");
            }

            mSelectedItem = dir;
        }

        private void miNewDoc_Click(object sender, RoutedEventArgs e)
        {
            if (mSelectedItem == null || !(mSelectedItem is Directory)) return;

            var nextId = mDocDic.Any() ? mDocDic.Keys.Last() + 1 : 0;
            var doc = new Document($"d{nextId}");
            var chapterNextId = MainWindow.DocDB.GetNextChaptersId();
            doc.SetDBInfo(nextId, chapterNextId);
            mDocDic.Add(nextId, doc);

            (mSelectedItem as Directory).UpdateDocIdList(new List<int> { nextId });
            mSelectedItem = doc;
        }

        private void txtExDir_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            if (string.IsNullOrEmpty(txtEx.Text))
            {
                // 何かディレクトリ名があったのを空白にした場合
                if ((mSelectedItem as Directory).IsChangeDirName)
                {
                    MessageBox.Show("ディレクトリ名を空文字にはできません。");
                    txtEx.Focus();
                    return;
                }

                var del = txtEx.DataContext as Directory;
                Directories.Remove(del);
                mAllDirDic.Remove(del.DirID);
                return;
            }
            txtEx.ToThickZero();
            txtEx.Background = (txtEx.DataContext as Directory).IsChangeDirName ? Brushes.Yellow : Brushes.White;
        }

        private void txtExDoc_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            if (string.IsNullOrEmpty(txtEx.Text))
            {
                //var del = txtEx.DataContext as Directory;
                //Directories.Remove(del);
                return;
            }
            txtEx.ToThickZero();
            txtEx.Background = (txtEx.DataContext as Document).IsChangeDocName ? Brushes.Yellow : Brushes.White;
        }

        void txtEx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            txtEx.IsReadOnly = false;
            txtEx.Background = Brushes.White;
            if (txtEx.DataContext is Directory dir) mSelectedItem = dir;
            else mSelectedItem = txtEx.DataContext as Document;
        }

        void txtEx_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            txtEx.IsReadOnly = true;
            txtEx.Background = txtEx.SelectionBrush;
            txtEx.ToThickOne();
            if (txtEx.DataContext is Directory dir) mSelectedItem = dir;
            else mSelectedItem = txtEx.DataContext as Document;
        }

        void txtEx_Loaded(object sender, RoutedEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            txtEx.GotFocus -= txtEx_GotFocus; // 最初の追加時は入力可能にしたいので一瞬だけ外す
            if (mSelectedItem == txtEx.DataContext)
            {
                txtEx.ToThickOne();
                txtEx.Focus();
            }
            txtEx.GotFocus += txtEx_GotFocus;
        }

        private void txtEx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tvDirDoc.Focus(); // フォーカスを外したい
                mSelectedItem = null;
            }
        }

        private void tvDirDoc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tvDirDoc.Focus();
            mSelectedItem = null;
        }
    }
}
