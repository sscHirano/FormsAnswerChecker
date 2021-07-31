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
        private static readonly int MAIL_ADDRESS_INDEX = 4;

        /// <summary>
        /// 回答済みメンバー(メールアドレス)一覧を取得する
        /// </summary>
        /// <param name="fileName">Forms回答結果エクセル</param>
        /// <returns></returns>
        internal static List<string> GetAnsweredList(string fileName)
        {
            XLWorkbook workbook = new XLWorkbook(fileName);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            int lastRow = worksheet.LastRowUsed().RowNumber();
            // 回答済みメンバー(メールアドレス)一覧を取得
            List<string> answeredLisd = new List<string>(lastRow);
            for (int i = 1; i <= lastRow; i++)
            {
                IXLCell cell = worksheet.Cell(i, MAIL_ADDRESS_INDEX);
                Console.WriteLine(cell.Value);
                answeredLisd.Add(cell.Value.ToString());
            }
            return answeredLisd;
        }
    }
}
