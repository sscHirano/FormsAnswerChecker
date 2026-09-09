using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormsAnswerChecker;

namespace FormsAnswerChecker.Tests
{
    [TestClass]
    public class ExcelParserTests
    {
        private string mTempExcelPath;

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(mTempExcelPath))
            {
                try
                {
                    File.Delete(mTempExcelPath);
                }
                catch
                {
                    // テストクリーンアップ時の削除エラーは無視
                }
            }
        }

        private string CreateTempExcelFile(Action<IXLWorksheet> populateSheet)
        {
            mTempExcelPath = Path.Combine(Path.GetTempPath(), "ExcelParserTest_" + Guid.NewGuid().ToString() + ".xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                populateSheet(worksheet);
                workbook.SaveAs(mTempExcelPath);
            }
            return mTempExcelPath;
        }

        [TestMethod]
        public void GetAnsweredList_正常なExcelからD列のメールアドレスを抽出できること()
        {
            // 準備: ヘッダー行 + 2行のデータ
            string path = CreateTempExcelFile(ws =>
            {
                // 1行目: ヘッダー
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "開始時刻";
                ws.Cell(1, 3).Value = "完了時刻";
                ws.Cell(1, 4).Value = "メール";
                ws.Cell(1, 5).Value = "氏名";

                // 2行目: 回答者1
                ws.Cell(2, 4).Value = "alice@example.com";
                // 3行目: 回答者2
                ws.Cell(3, 4).Value = "bob@example.com";
            });

            // 実行
            List<string> answeredList = ExcelParser.GetAnsweredList(path);

            // 検証: 1行目はスキップされ、2行目以降のD列のみ取得されること
            CollectionAssert.AreEqual(
                new List<string> { "alice@example.com", "bob@example.com" },
                answeredList
            );
        }

        [TestMethod]
        public void GetAnsweredList_ヘッダー行のみの場合は空リストが返ること()
        {
            // 準備: 1行目のみ
            string path = CreateTempExcelFile(ws =>
            {
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 4).Value = "メール";
            });

            // 実行
            List<string> answeredList = ExcelParser.GetAnsweredList(path);

            // 検証
            Assert.AreEqual(0, answeredList.Count);
        }

        [TestMethod]
        public void GetAnsweredList_セルが完全に空のシートの場合は空リストが返ること()
        {
            // 準備: データなしのシート
            string path = CreateTempExcelFile(ws =>
            {
                // 何も書き込まない
            });

            // 実行
            List<string> answeredList = ExcelParser.GetAnsweredList(path);

            // 検証
            Assert.IsNotNull(answeredList);
            Assert.AreEqual(0, answeredList.Count);
        }
    }
}
