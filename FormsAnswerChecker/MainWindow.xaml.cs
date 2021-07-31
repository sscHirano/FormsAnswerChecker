using System;
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
        public MainWindow()
        {
            InitializeComponent();

            AddHandler(TextBox.DropEvent, new DragEventHandler(FileListBox_Drop), true);

            if (!ReadAnswerList())
            {
                textBox.Text = "回答者のメールアドレス一覧を記載したAnswerList.txtファイルを準備してください";
                textBox.Background = Brushes.Red;
            }
        }

        /// <summary>
        /// 対象となる回答者一覧を読み込む。
        /// </summary>
        private bool ReadAnswerList()
        {
            try
            {
                AnswerList answer = new AnswerList();
            }
            catch (System.IO.FileNotFoundException e)
            {
                // ファイル無し
                return false;
            }
            return true;
        }

        private void FileListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                
            }
        }

    }
}
