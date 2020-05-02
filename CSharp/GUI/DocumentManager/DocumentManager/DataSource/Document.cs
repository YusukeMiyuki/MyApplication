using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.DataSource
{
    public class Document
    {
        List<Chapter> mChapters;

        public string DocName { get; private set; }
        public string HeadLine => DocName;
        public IReadOnlyList<Chapter> Chapters => mChapters;

        public Document(string docName)
        {
            DocName = docName;
            mChapters = new List<Chapter>();
        }

        public void AddChapter(string headline) => mChapters.Add(new Chapter(headline));
    }
}
