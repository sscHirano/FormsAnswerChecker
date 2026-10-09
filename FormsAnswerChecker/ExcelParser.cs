using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsAnswerChecker
{
    class ExcelParser
    {
        /// <summary>
        /// メールアドレスの列(D列)Index
        /// </summary>
        private const int MAIL_ADDRESS_INDEX = 4;

        /// <summary>
        /// 回答済みメンバー(メールアドレス)一覧を取得する
        /// </summary>
        /// <param name="fileName">Forms回答結果エクセル</param>
        /// <returns></returns>
        internal static List<string> GetAnsweredList(string fileName)
        {
            using (XLWorkbook workbook = new XLWorkbook(fileName))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                var lastRowUsed = worksheet.LastRowUsed();
                if (lastRowUsed == null)
                {
                    return new List<string>();
                }
                int lastRow = lastRowUsed.RowNumber();
                List<string> answeredLisd = new List<string>(lastRow);
                // 回答済みメンバー(メールアドレス)一覧を取得 (1行目はヘッダー行のため2行目から開始。Excelライブラリではindexが1始まり)
                for (int i = 2; i <= lastRow; i++)
                {
                    IXLCell cell = worksheet.Cell(i, MAIL_ADDRESS_INDEX);
                    answeredLisd.Add(cell.Value.ToString());
                }
                return answeredLisd;
            }
        }
    }
}
