using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsAnswerChecker
{
    class AnswerRequestList
    {
        private readonly List<string> mAnswerRequestList = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// 指定されたファイルを読み込み、リストを作成する
        /// </summary>
        /// <param name="filePath">回答対象者ファイルのパス</param>
        internal AnswerRequestList(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (streamReader.EndOfStream == false)
                {
                    string member = streamReader.ReadLine();
                    if (String.IsNullOrWhiteSpace(member))
                    {
                        continue;
                    }
                    mAnswerRequestList.Add(member);
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
            foreach (string answer in mAnswerRequestList)
            {
                if (!answeredList.Contains(answer))
                {
                    unansweredList.Add(answer);
                }
            }
            return unansweredList;
        }

        /// <summary>
        /// 実際の回答者を引数に受け取り、対象リストに存在しない回答者（想定外の回答者）を返却する
        /// </summary>
        /// <param name="answeredList">回答済みリスト</param>
        /// <returns>対象外の回答者一覧</returns>
        internal List<string> GetUnexpectedAnsweredList(List<string> answeredList)
        {
            List<string> unexpectedList = new List<string>();
            foreach (string answer in answeredList)
            {
                if (!mAnswerRequestList.Contains(answer) && !unexpectedList.Contains(answer))
                {
                    unexpectedList.Add(answer);
                }
            }
            return unexpectedList;
        }

    }
}
