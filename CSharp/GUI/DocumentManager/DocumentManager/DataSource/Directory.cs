using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing.Printing;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.DataSource
{
    /// <summary>
    /// フォルダを扱うクラス
    /// </summary>
    public class Directory : INotifyPropertyChanged
    {
        #region DB情報
        public int DirID { get; private set; }
        public int IsTopDir { get; private set; }
        public string DocID { get; private set; }
        public string ChildDirID { get; private set; }
        #endregion

        #region プライベートメンバ
        string mInitDirName;
        List<int> mDocIDList;
        List<int> mChildDirIdList;
        #endregion

        #region パブリックメンバ
        public event PropertyChangedEventHandler PropertyChanged;
        public string DirName { get; set; }
        public bool IsTop => IsTopDir == 1;
        public IReadOnlyList<int> DocIdList => mDocIDList;
        public IReadOnlyList<Document> DocList => MainWindow.DocWindow.GetDocumentList(mDocIDList);
        public IReadOnlyList<int> ChildDirIdList => mChildDirIdList;
        public IReadOnlyList<Directory> ChildDirList => MainWindow.DocWindow.GetDirectoryList(mChildDirIdList);

        public bool IsChangeDirName => DirName != mInitDirName;

        public ObservableCollection<object> Items
        {
            get
            {
                var childNodes = new ObservableCollection<object>();
                foreach (var doc in ChildDirList)
                {
                    childNodes.Add(doc);
                }
                foreach (var dir in DocList)
                {
                    childNodes.Add(dir);
                }
                return childNodes;
            }
        }
        #endregion

        public Directory(string dirName)
        {
            DirName = dirName;
            mInitDirName = dirName;
            mDocIDList = new List<int>();
            mChildDirIdList = new List<int>();
        }

        public void SetInitDBInfo(int dirID, int isTopDir, string docId, string childDirId)
        {
            DirID = dirID;
            IsTopDir = isTopDir;
            DocID = docId;
            mDocIDList = string.IsNullOrEmpty(docId) ? new List<int>() : docId.Split(',').Select(x => int.Parse(x)).ToList();
            ChildDirID = childDirId;
            mChildDirIdList = string.IsNullOrEmpty(childDirId) ? new List<int>() : childDirId.Split(',').Select(x => int.Parse(x)).ToList();
        }

        public void UpdateChildDirIdList(List<int> idList)
        {
            mChildDirIdList.AddRange(idList.Where(x => mChildDirIdList.Contains(x) == false));
            OnPropertyChanged(nameof(Items));
        }

        public void UpdateDocIdList(List<int> idList)
        {
            mDocIDList.AddRange(idList.Where(x => mDocIDList.Contains(x) == false));
            OnPropertyChanged(nameof(Items));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル", Justification = "protectedのため、スタイルの適用禁止")]
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
