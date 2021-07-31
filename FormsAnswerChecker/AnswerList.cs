using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsAnswerChecker
{
    class AnswerList
    {
        private readonly string ANSWER_LIST_FILE_NAME = "AnswerList.txt";

        private List<string> mAnswerList = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// ファイルを読み込み、リストを作成する
        /// exeと同じ位置にAnswerList.txtを準備しておくこと
        /// </summary>
        internal AnswerList()
        {
            using (StreamReader streamReader = new StreamReader(ANSWER_LIST_FILE_NAME, Encoding.UTF8))
            {
                while (streamReader.EndOfStream == false)
                {
                    string member = streamReader.ReadLine();
                    if (String.IsNullOrWhiteSpace(member))
                    {
                        continue;
                    }
                    mAnswerList.Add(member);
                }
            }
        }

        /// <summary>
        /// 実際の回答者を引数に受け取り、未回答者を返却する
        /// </summary>
        /// <param name="answeredList">回答済みリスト</param>
        /// <returns>未回答者</returns>
        internal List<string> GetUnansweredList(List<string> answeredList)
        {
            List<string> unansweredList = new List<string>();
            foreach (string answer in mAnswerList)
            {
                if (!answeredList.Contains(answer))
                {
                    unansweredList.Add(answer);
                }
            }
            return unansweredList;
        }

    }
}
