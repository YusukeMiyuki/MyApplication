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

        public Document()
        {
            DocName = "TestDoc";
            mChapters = new List<Chapter>();

            AddChapter("TestChapter");
            mChapters.First().AddChapter("TestChapter2");
        }

        public void AddChapter(string headline) => mChapters.Add(new Chapter(headline));
    }
}
