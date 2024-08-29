using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Testing
{
    public class FileWatcher
    {
        private FileSystemWatcher watcher;

        static void Main(string[] args)
        {
            FileWatcher filewatcher = new FileWatcher();
            filewatcher.InitWatcher();

            Console.WriteLine("이메일 보내기 성공이 뜰때까지 대기");
            Console.ReadLine();
        }

        private void InitWatcher()
        {
            try
            {
                newWatcher();
                watcher.Created += new FileSystemEventHandler(Created);
                watcher.Error += new ErrorEventHandler(OnError);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            int iMaxAttempts = 120;
            int iTimeOut = 30000;
            int i = 0;
            while (watcher.EnableRaisingEvents == false && i < iMaxAttempts)
            {
                i += 1;
                try
                {
                    watcher.EnableRaisingEvents = true;
                }
                catch
                {
                    watcher.EnableRaisingEvents = false;
                    System.Threading.Thread.Sleep(iTimeOut);
                }
            }
            InitWatcher();
        }

        private void newWatcher()
        {
            //string filePath = $"C:\\VFlap\\potoHoli\\photo_image\\";
            string filePath = @"D:\WatcherTesting\SQLTesting\photo_image\";

            watcher = new FileSystemWatcher();

            watcher.Path = filePath;

            watcher.NotifyFilter = NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.Size |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite;

            watcher.Filter = "*.jpg";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void Created(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Created 시작");
            Console.WriteLine("생성 완료!");

            string imgFile = "", userAddr = "";

            SQLitePCL.Batteries.Init();
            // ------------------------------------------넣어야할부분-------------------------------------------
            //var connectionStringFile = @"Data Source=C:db 데이터 파일 경로 넣기";
            var connectionStringFile = @"Data Source=D:\판교박물관 포토홀리\potoHoli\data\holilog.dat";
            // ------------------------------------------넣어야할부분-------------------------------------------
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
