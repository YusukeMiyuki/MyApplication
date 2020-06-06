using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace OutDocChapter
{
    public partial class MyRibbon
    {

        private void myRibbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void btnOutChapter_Click(object sender, RibbonControlEventArgs e)
        {
            #region wordの章構成取得
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            var tplInfoList = new List<(string cNum, string cTitle, string cPage)>();

            for (int i = 1; i < doc.Paragraphs.Count; i++)
            {
                var para = doc.Paragraphs[i];

                if (!(para.get_Style() is Word.Style style))
                {
                    MessageBox.Show("なんかstyleが認識できない", $"{btnOutChapter.Label}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (style.NameLocal.Contains("見出し"))
                {
                    var range = para.Range;
                    var chapterNum = range.ListFormat.ListString;
                    var chapterTitle = range.Text.Replace(Environment.NewLine, "");
                    var page = range.Information[Word.WdInformation.wdActiveEndPageNumber] as object;
                    var info = (cNum: chapterNum, cTitle: chapterTitle, cPage: page?.ToString() ?? string.Empty);
                    tplInfoList.Add(info);
                }
            }
            if (tplInfoList.Count <= 0)
            {
                MessageBox.Show("章を確認できませんでした。", $"{btnOutChapter.Label}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion

            #region Excel出力
            var excelApp = new Excel.Application()
            {
                Visible = false
            };
            try
            {
                var thisAddinUriPath = Assembly.GetExecutingAssembly().CodeBase; // Assembly.GetExecutingAssemblyを何かの関数内で使用すると、このアドインのパスが取得できない。
                var uriPath = new UriBuilder(thisAddinUriPath);  // 取得されるパスは形式がUriなので、Pathで扱えるよう変換していく
                var path = Uri.UnescapeDataString(uriPath.Path);
                var thisAddinPath = Path.GetDirectoryName(Path.GetFullPath(path));
                var templatePath = Path.Combine(thisAddinPath, "Template", "OutChapterTemplate.xltx");
                Excel.Workbook wb = excelApp.Workbooks.Open(templatePath, ReadOnly: "False");
                try
                {
                    Excel.Worksheet sheet = wb.Sheets[1];
                    try
                    {
                        setCellValue(sheet, 1, 1, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        setCellValue(sheet, 2, 1, Path.GetFileName(doc.FullName));

                        // 章の数を最大列数として確保する。 「+1」は0になるのを防ぐため。
                        var colMax = tplInfoList.Max(x => x.cNum.Count(c => c == '.')) + 1;
                        // 行の「+」は念のため。
                        // 列の「+」も念のため。
                        var setValue = new object[tplInfoList.Count + 10, colMax * 2 + 10]; // 「*2」は章タイトルの行

                        for (int row = 0; row < tplInfoList.Count; row++)
                        {
                            var tplInfo = tplInfoList[row];
                            setValue[row, 0] = tplInfo.cPage;

                            var colIndent = tplInfo.cNum.Count(c => c == '.');
                            setValue[row, 1 + colIndent] = tplInfo.cNum.Last().ToString();
                            setValue[row, 1 + colMax + colIndent] = tplInfo.cTitle;
                        }

                        #region 列、及び行の挿入
                        if (colMax >= 2)
                        {
                            // 列の挿入
                            // 必ず章タイトル列から追加する。章番号から追加すると、章タイトル側の列追加位置がわかりにくくなるため。
                            // 1列追加 = 5 + 0 としなければならない。Excelは1行選んで挿入すれば1行追加されるため。また、テンプレートにはすでに1行分確保されていることに注意
                            var addChapterTitleCol = getRange(sheet, 1, 5, sheet.Rows.Count, 5 + colMax - 2);
                            try
                            {
                                addChapterTitleCol.Select();
                                addChapterTitleCol.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                            }
                            finally { Marshal.ReleaseComObject(addChapterTitleCol); }
                            var addChpaterNumCol = getRange(sheet, 1, 4, sheet.Rows.Count, 4 + colMax - 2);
                            try
                            {
                                addChpaterNumCol.Select();
                                addChpaterNumCol.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                            }
                            finally { Marshal.ReleaseComObject(addChpaterNumCol); }
                        }

                        // 行の挿入
                        var addRow = getRange(sheet, 6, 1, 6 + tplInfoList.Count - 2, sheet.Columns.Count);
                        try
                        {
                            addRow.Select();
                            addRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                        }
                        finally { Marshal.ReleaseComObject(addRow); }
                        #endregion

                        #region Excelに貼り付け
                        // Excelの貼り付け開始位置（Indentが1始まりなので注意）
                        var target = getRange(sheet, 6, 2, 6 + setValue.GetLength(0) - 1, 2 + setValue.GetLength(1) - 1);
                        try
                        {
                            target.Value = setValue;
                        }
                        finally { Marshal.ReleaseComObject(target); }
                        #endregion
                    }
                    finally { Marshal.ReleaseComObject(sheet); }
                }
                finally { Marshal.ReleaseComObject(wb); }
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                excelApp.Visible = true;
            }
            #endregion

            #region (ローカル関数) 指定セルに値代入
            void setCellValue(Excel.Worksheet sht, int row, int col, string value)
            {
                var targetRange = (Excel.Range)sht.Cells[row, col];
                try
                {
                    targetRange.Value = value;
                }
                finally { Marshal.ReleaseComObject(targetRange); }
            }
            #endregion

            #region (ローカル関数) 指定範囲のRengeオブジェクト取得
            Excel.Range getRange(Excel.Worksheet sht, int row1, int col1, int row2, int col2)
            {
                var startCell = (Excel.Range)sht.Cells[row1, col1];
                try
                {
                    var endCell = (Excel.Range)sht.Cells[row2, col2];
                    try
                    {
                        return sht.Range[startCell, endCell];
                    }
                    finally { Marshal.ReleaseComObject(endCell); }
                }
                finally { Marshal.ReleaseComObject(startCell); }
            }
            #endregion
        }
    }
}
