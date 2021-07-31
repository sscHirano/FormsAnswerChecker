using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsAnswerChecker
{
    public class AnswerList
    {
        private readonly string ANSWER_LIST_FILE_NAME = "AnswerList.txt";

        private List<string> mAnswerList = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// ファイルを読み込み、リストを作成する
        /// exeと同じ位置にAnswerList.txtを準備しておくこと
        /// </summary>
        public AnswerList()
        {
            using (StreamReader streamReader = new StreamReader(ANSWER_LIST_FILE_NAME, Encoding.UTF8))
            {
                while (streamReader.EndOfStream == false)
                {
                    mAnswerList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// 実際の回答者を引数に受け取り、未回答者を返却する
        /// </summary>
        /// <param name="answeredList">回答済みリスト</param>
        /// <returns>未回答者</returns>
        public List<string> GetUnansweredList(List<string> answeredList)
        {
            throw new NotImplementedException();
        }

    }
}
