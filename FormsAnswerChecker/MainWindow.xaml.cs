using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const string ANSWER_LISTS_DIR = "AnswerLists";

        /// <summary>
        /// 回答者リスト(回答する必要のある人一覧)
        /// </summary>
        private AnswerList mAnswerList;

        public MainWindow()
        {
            InitializeComponent();

            AddHandler(TextBox.DropEvent, new DragEventHandler(FileListBox_Drop), true);
            AddHandler(TextBox.PreviewDragOverEvent, new DragEventHandler(Window_PreviewDragOver), true);

            InitCategoryList();
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        /// <summary>
        /// AnswerListsフォルダ内のテキストファイル一覧をComboBoxにセットする
        /// </summary>
        private void InitCategoryList()
        {
            string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ANSWER_LISTS_DIR);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var txtFiles = Directory.GetFiles(dirPath, "*.txt");

            if (txtFiles.Length == 0)
            {
                SetErrorMessage(string.Format("exeと同じ位置の {0} フォルダ内に、回答者一覧テキストファイル(.txt)を配置してください。", ANSWER_LISTS_DIR));
                return;
            }

            foreach (var file in txtFiles)
            {
                categoryComboBox.Items.Add(Path.GetFileName(file));
            }

            categoryComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// カテゴリ選択変更時イベント
        /// </summary>
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem == null)
            {
                return;
            }

            string fileName = categoryComboBox.SelectedItem.ToString();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ANSWER_LISTS_DIR, fileName);

            if (!ReadAnswerList(filePath))
            {
                SetErrorMessage(string.Format("ファイルの読み込みに失敗しました: {0}", fileName));
            }
        }

        /// <summary>
        /// 対象となる回答者一覧を読み込む。
        /// </summary>
        private bool ReadAnswerList(string filePath)
        {
            try
            {
                mAnswerList = new AnswerList(filePath);
            }
            catch (Exception)
            {
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
