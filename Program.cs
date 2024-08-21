using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading;

namespace Testing
{
    public class FileWatcher
    {
        static void Main(string[] args)
        {
            FileWatcher filewatcher = new FileWatcher();
            filewatcher.InitWatcher();

            Console.WriteLine("이메일 보내기 성공이 뜰때까지 대기");
            Console.ReadLine();
        }

        public void InitWatcher()
        {
            string filePath = $"C:\\VFlap\\potoHoli\\photo_image\\";

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = filePath;

            watcher.NotifyFilter = NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.Size |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite;

            watcher.Filter = "*.jpg";
            watcher.IncludeSubdirectories = true;

            watcher.Created += new FileSystemEventHandler(Created);

            watcher.EnableRaisingEvents = true;
        }

        private void Created(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(3000);
            Console.WriteLine("생성 완료!");

            string imgFile = "", userAddr = "";

            SQLitePCL.Batteries.Init();
            var connectionStringFile = @"Data Source=C:\VFlap\potoHoli\data\holilog.dat";
            using var connection = new SqliteConnection(connectionStringFile);
            connection.Open();
            using var readCmd = connection.CreateCommand();
            readCmd.CommandText =
            @"  SELECT *
                FROM TB_RECEIVE
                WHERE n_sendType = 1
                ORDER BY ROWID DESC LIMIT 1
            ";
            using var reader = readCmd.ExecuteReader();

            string[] names = e.FullPath.Split('\\');
            string path = names[names.Length - 2] + '/' + names[names.Length - 1];

            while (reader.Read())
            {
                userAddr = $"{reader["s_userAddr"]}";
                imgFile = $"{reader["s_imgFile"]}";
                Console.WriteLine($"DB 메일주소: {userAddr}");
                Console.WriteLine($"DB 파일이름: {imgFile}");
                Console.WriteLine($"파일생성 위치: {e.FullPath}");
                Console.WriteLine($"파일위치 필터링: {path}");
            }

            if (imgFile == path)
            {
                SendMail sendMail = new SendMail();
                sendMail.S_Mail(e.FullPath, userAddr);
            }
            else
            {
                Console.WriteLine("파일 감지부분 전송 실패");
                Console.WriteLine("로그 파일 userAddr : " + userAddr);
                Console.WriteLine("로그 파일 imgFile : " + imgFile);
                Console.WriteLine("실제 사진 path : " + path);
            }
        }
    }
}
