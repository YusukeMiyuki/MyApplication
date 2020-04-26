using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DocumentManager.DataSource
{
    public class Chapter
    {
        List<Chapter> mChapters = new List<Chapter>();

        public string HeadLine { get; private set; }
        public string Text { get; private set; }

        public IReadOnlyList<Chapter> Chapters => mChapters;

        public Chapter(string headline)
        {
            HeadLine = headline;
        }

        public void AddChapter(string headline) => mChapters.Add(new Chapter(headline));
    }
}
