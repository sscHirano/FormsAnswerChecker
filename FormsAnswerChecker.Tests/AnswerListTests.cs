using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormsAnswerChecker;

namespace FormsAnswerChecker.Tests
{
    [TestClass]
    public class AnswerListTests
    {
        private string mTempFilePath;

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(mTempFilePath))
            {
                try
                {
                    File.Delete(mTempFilePath);
                }
                catch
                {
                    // テストクリーンアップ時の削除エラーは無視
                }
            }
        }

        private string CreateTempAnswerFile(IEnumerable<string> lines)
        {
            mTempFilePath = Path.Combine(Path.GetTempPath(), "AnswerListTest_" + Guid.NewGuid().ToString() + ".txt");
            File.WriteAllLines(mTempFilePath, lines, Encoding.UTF8);
            return mTempFilePath;
        }

        [TestMethod]
        public void コンストラクタ_空行や空白行を除外して正しく読み込めること()
        {
            // 準備: 空行や空白が含まれるリスト
            var lines = new[]
            {
                "user1@example.com",
                "",
                "   ",
                "user2@example.com",
                "user3@example.com"
            };
            string path = CreateTempAnswerFile(lines);

            // 実行
            var answerList = new AnswerList(path);
            var answered = new List<string>();

            // 検証: 全員未回答として取得し、読み込まれたメンバーを確認
            var unanswered = answerList.GetUnansweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user1@example.com", "user2@example.com", "user3@example.com" },
                unanswered
            );
        }

        [TestMethod]
        public void GetUnansweredList_未回答者が正しく抽出されること()
        {
            // 準備
            var lines = new[]
            {
                "user1@example.com",
                "user2@example.com",
                "user3@example.com"
            };
            string path = CreateTempAnswerFile(lines);
            var answerList = new AnswerList(path);

            // 実行: user1 だけ回答済み
            var answered = new List<string> { "user1@example.com" };
            var unanswered = answerList.GetUnansweredList(answered);

            // 検証: user2, user3 が未回答
            CollectionAssert.AreEqual(
                new List<string> { "user2@example.com", "user3@example.com" },
                unanswered
            );
        }

        [TestMethod]
        public void GetUnansweredList_全員回答済みの場合は空リストが返ること()
        {
            // 準備
            var lines = new[] { "user1@example.com", "user2@example.com" };
            string path = CreateTempAnswerFile(lines);
            var answerList = new AnswerList(path);

            // 実行: 全員回答済み
            var answered = new List<string> { "user1@example.com", "user2@example.com" };
            var unanswered = answerList.GetUnansweredList(answered);

            // 検証
            Assert.AreEqual(0, unanswered.Count);
        }

        [TestMethod]
        public void GetUnexpectedAnsweredList_想定外の回答者が正しく抽出されること()
        {
            // 準備
            var lines = new[] { "user1@example.com", "user2@example.com" };
            string path = CreateTempAnswerFile(lines);
            var answerList = new AnswerList(path);

            // 実行: 想定外の user99 が回答
            var answered = new List<string> { "user1@example.com", "user99@example.com" };
            var unexpected = answerList.GetUnexpectedAnsweredList(answered);

            // 検証
            CollectionAssert.AreEqual(
                new List<string> { "user99@example.com" },
                unexpected
            );
        }

        [TestMethod]
        public void GetUnexpectedAnsweredList_同一の想定外回答者が複数回答しても重複しないこと()
        {
            // 準備
            var lines = new[] { "user1@example.com" };
            string path = CreateTempAnswerFile(lines);
            var answerList = new AnswerList(path);

            // 実行: user99 が2回回答している
            var answered = new List<string> { "user99@example.com", "user99@example.com" };
            var unexpected = answerList.GetUnexpectedAnsweredList(answered);

            // 検証: 1件のみ取得されること
            CollectionAssert.AreEqual(
                new List<string> { "user99@example.com" },
                unexpected
            );
        }

        [TestMethod]
        public void コンストラクタ_実ファイルを使用して正しく読み込めること()
        {
            // 準備: テスト用回答者リストのパス
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAnswersList.txt");

            // 実行
            var answerList = new AnswerList(path);
            var answered = new List<string>();

            // 検証: 全員未回答として取得し、読み込まれたメンバーを確認
            var unanswered = answerList.GetUnansweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user1@example.com", "user2@example.com", "user3@example.com" },
                unanswered
            );
        }

        [TestMethod]
        public void GetUnansweredList_実ファイルを使用して未回答者が正しく抽出されること()
        {
            // 準備: 回答対象者リストのパス(user1～user11)
            string answerListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAnswersList02.txt");
            // 準備: 回答済みリストのエクセルファイルのパス(user1～user10)
            string answeredFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xlsx");

            // 実行
            var answerList = new AnswerList(answerListPath);
            var answered = ExcelParser.GetAnsweredList(answeredFilePath);

            // 検証: user11のみ未回答として取得されることを確認
            var unanswered = answerList.GetUnansweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user11@example.com" },
                unanswered
            );
        }

        [TestMethod]
        public void GetUnansweredList_実ファイルを使用して全員回答済みの場合は空リストが返ること()
        {
            // 準備: 回答対象者リストのパス(user1～user10)
            string answerListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAnswersList03.txt");
            // 準備: 回答済みリストのエクセルファイルのパス(user1～user10)
            string answeredFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xlsx");

            // 実行
            var answerList = new AnswerList(answerListPath);
            var answered = ExcelParser.GetAnsweredList(answeredFilePath);

            // 検証: 全員回答済みのため、未回答者リストの件数が0であることを確認
            var unanswered = answerList.GetUnansweredList(answered);
            Assert.AreEqual(0,unanswered.Count);
        }

        [TestMethod]
        public void GetUnexpectedAnsweredList_実ファイルを使用して想定外の回答者が正しく抽出されること()
        {
            // 準備: 回答対象者リストのパス(user1～user9)
            string answerListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAnswersList04.txt");
            // 準備: 回答済みリストのエクセルファイルのパス(user1～user10)
            string answeredFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xlsx");

            // 実行
            var answerList = new AnswerList(answerListPath);
            var answered = ExcelParser.GetAnsweredList(answeredFilePath);

            // 検証: 想定外の回答者(user10)が正しく抽出されるか
            var unexpected = answerList.GetUnexpectedAnsweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user10@example.com" },
                unexpected
            );
        }

        [TestMethod]
        public void 実ファイルを使用して想定外の回答者と未回答者の両方が正しく抽出されること()
        {
            // 準備: 回答対象者リストのパス(user1～user9, user11)
            string answerListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAnswersList05.txt");
            // 準備: 回答済みリストのエクセルファイルのパス(user1～user10)
            string answeredFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xlsx");

            // 実行
            var answerList = new AnswerList(answerListPath);
            var answered = ExcelParser.GetAnsweredList(answeredFilePath);

            // 検証: 未回答者(user11)が正しく抽出されること
            var unanswered = answerList.GetUnansweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user11@example.com" },
                unanswered
            );

            // 検証: 想定外の回答者(user10)が正しく抽出されること
            var unexpected = answerList.GetUnexpectedAnsweredList(answered);
            CollectionAssert.AreEqual(
                new List<string> { "user10@example.com" },
                unexpected
            );
        }
    }
}
