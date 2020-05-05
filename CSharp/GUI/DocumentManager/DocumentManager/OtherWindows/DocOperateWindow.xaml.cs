using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentManager.DataSource;
using WpfCustomControls.MyTextBox;

namespace DocumentManager.OtherWindows
{
    /// <summary>
    /// DocOperateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DocOperateWindow : Window
    {
        #region プライベートメンバ
        SortedDictionary<int, Document> mAllDocDic { get; } = new SortedDictionary<int, Document>();
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

        public DocOperateWindow()
        {
            InitializeComponent();

            Directories = new ObservableCollection<Directory>();
            tvDirDoc.ItemsSource = Directories;

            foreach (var dir in MainWindow.DocDB.GetDirectoryList().OrderBy(x => x.DirID))
            {
                Directories.Add(dir);
                if (mAllDirDic.ContainsKey(dir.DirID) == false) mAllDirDic.Add(dir.DirID, dir);
            }

            makeDocDictionary();

            // デバッグ用
            var dir0 = new Directory("f0");
            dir0.SetInitDBInfo(0, "0", "1,2");
            mAllDirDic.Add(0, dir0);

            var dir1 = new Directory("f1");
            dir1.SetInitDBInfo(1, "1,2", "");
            mAllDirDic.Add(1, dir1);

            var dir2 = new Directory("f2");
            dir2.SetInitDBInfo(2, "", "");
            mAllDirDic.Add(2, dir2);

            var doc0 = new Document("d0");
            doc0.SetDBInfo(0, 0);
            mAllDocDic.Add(0, doc0);

            var doc1 = new Document("d1");
            doc1.SetDBInfo(1, 1);
            mAllDocDic.Add(1, doc1);

            var doc2 = new Document("d2");
            doc2.SetDBInfo(2, 2);
            mAllDocDic.Add(2, doc2);

            Directories.Add(dir0);
        }

        #region イベント

        #region OKボタン
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtNewDocName.Text))
                //{
                //    // エラー
                //    MessageBox.Show("ドキュメント名が入力されていません。");
                //    txtNewDocName.Focus();
                //    return;
                //}
                //NewDocName = txtNewDocName.Text;
                //DialogResult = true;
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
                //e.Handled = txtNewDocName.CheckNGInput(e.Text);
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
            //try { txtNewDocName.CheckTextChanged(); }
            //catch { }
            //finally { }
        }
        #endregion

        #endregion

        #region 文書検索用のDictionary作成
        void makeDocDictionary()
        {
            var docIdList = Directories.SelectMany(x => x.DocIdList).Distinct().ToList();
            foreach (var doc in MainWindow.DocDB.GetDocumentList(docIdList))
            {
                if (mAllDocDic.ContainsKey(doc.DocID) == false) mAllDocDic.Add(doc.DocID, doc);
            }
        }
        #endregion

        #region 文書の取得
        public List<Document> GetDocumentList(List<int> docIdList)
        {
            var docList = new List<Document>();
            foreach (var id in docIdList)
            {
                if (mAllDocDic.TryGetValue(id, out var doc)) docList.Add(doc);
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
            var dir = new Directory($"f{nextId}");
            dir.SetInitDBInfo(nextId, "", "");
            mAllDirDic.Add(nextId, dir);

            if (mSelectedItem != null) (mSelectedItem as Directory).UpdateChildDirIdList(new List<int> { nextId });
            else Directories.Add(dir);

            mSelectedItem = dir;
        }

        private void miNewDoc_Click(object sender, RoutedEventArgs e)
        {
            if (mSelectedItem == null || !(mSelectedItem is Directory)) return;

            var nextId = mAllDocDic.Any() ? mAllDocDic.Keys.Last() + 1 : 0;
            var doc = new Document($"d{nextId}");
            var chapterNextId = mAllDocDic.Any() ? mAllDocDic.Values.Max(x => x.ChaptersID) + 1 : 0;
            doc.SetDBInfo(nextId, chapterNextId);
            mAllDocDic.Add(nextId, doc);

            (mSelectedItem as Directory).UpdatDocIdList(new List<int> { nextId });
            mSelectedItem = doc;
        }

        private void txtExDir_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            if (string.IsNullOrEmpty(txtEx.Text))
            {
                var del = txtEx.DataContext as Directory;
                Directories.Remove(del);
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
            txtEx.Background = (txtEx.DataContext as Document).IsChangeDirName ? Brushes.Yellow : Brushes.White;
        }

        void txtEx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var txtEx = sender as TextBoxEx;
            txtEx.IsReadOnly = false;
            txtEx.Background = Brushes.White;
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
    }
}
