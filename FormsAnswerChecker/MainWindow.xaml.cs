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
        private AnswerList mAnswerList;

        public MainWindow()
        {
            InitializeComponent();

            AddHandler(TextBox.DropEvent, new DragEventHandler(FileListBox_Drop), true);

            if (!ReadAnswerList())
            {
                string message = "exeファイルと同じ位置に回答者のメールアドレス一覧を記載したAnswerList.txtファイルを準備してください";
                SetErrorMessage(message);
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
            catch (System.IO.FileNotFoundException)
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
                try
                {
                    List<string> answeredList = ExcelParser.GetAnsweredList(fileNames[0]);
                    List<string> unansweredList = mAnswerList.GetUnansweredList(answeredList);

                    ShowUnansweredList(unansweredList);

                }
                catch (System.IO.IOException)
                {
                    SetErrorMessage("ファイルアクセスエラー：ファイルを開いていませんか？");
                }
            }
        }

        /// <summary>
        /// 未回答者一覧を表示する
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

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        /// <param name="message"></param>
        private void SetErrorMessage(string message)
        {
            textBox.Text = message;
            textBox.Background = Brushes.Red;
        }

    }
}
