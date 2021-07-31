using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FormsAnswerChecker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 回答者リスト(回答する必要のある人一覧)
        /// </summary>
        AnswerList mAnswerList;


        public MainWindow()
        {
            InitializeComponent();

            AddHandler(TextBox.DropEvent, new DragEventHandler(FileListBox_Drop), true);

            if (!ReadAnswerList())
            {
                SetErrorMessage("回答者のメールアドレス一覧を記載したAnswerList.txtファイルを準備してください");
            }
        }

        /// <summary>
        /// 対象となる回答者一覧を読み込む。
        /// </summary>
        private bool ReadAnswerList()
        {
            try
            {
                mAnswerList = new AnswerList();
            }
            catch (System.IO.FileNotFoundException e)
            {
                // ファイル無し
                return false;
            }
            return true;
        }

        private void FileListBox_Drop(object sender, DragEventArgs dragEvent)
        {
            if (dragEvent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])dragEvent.Data.GetData(DataFormats.FileDrop);
                // 複数ファイルがドロップされても、最初のファイルしか見ない。

                // 回答済みリストを取得
                //List<string> answeredList = GetAnsweredList(fileNames[0]);
                try
                {
                    List<string> answeredList = ExcelParser.GetAnsweredList(fileNames[0]);
                    List<string> unansweredList = mAnswerList.GetUnansweredList(answeredList);

                    ShowUnansweredList(unansweredList);

                }
                catch (System.IO.IOException e)
                {
                    SetErrorMessage("ファイルアクセスエラー：ファイルを開いていませんか？");
                }
            }
        }

        /// <summary>
        /// 未回答者一覧
        /// </summary>
        /// <param name="unansweredList"></param>
        private void ShowUnansweredList(List<string> unansweredList)
        {
            if (unansweredList.Count == 0)
            {
                MessageBox.Show("全員回答済み！");
            }
            else
            {
                string message = "未回答者は以下です。\nCtrl + cを押し、クリップボードに一覧をコピーして催促メール等にご活用ください\n----\n";
                foreach (string unanswer in unansweredList)
                {
                    message += unanswer + "\n";
                }
                MessageBox.Show(message);
            }
        }

        private void SetErrorMessage(string message)
        {
            textBox.Text = message;
            textBox.Background = Brushes.Red;
        }

        /// <summary>
        /// Forms回答ファイルを引数に、回答済みリストを取得する
        /// </summary>
        /// <param name="fileName">Forms回答ファイル</param>
        /// <returns></returns>
        private List<string> GetAnsweredList(string fileName)
        {
            try
            {
                return ExcelParser.GetAnsweredList(fileName);
            }
            catch (System.IO.IOException e)
            {

            }
            return null;
        }
    }
}
