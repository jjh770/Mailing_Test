using System.IO;
using Microsoft.Data.Sqlite;

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
        public bool createdFinish = false;

        public void InitWatcher()
        {
            // ------------------------------------------넣어야할부분-------------------------------------------
            // string filePath = $"사진이 저장되는 주소";
            // ------------------------------------------넣어야할부분-------------------------------------------
            string filePath = $"D:\\WatcherTesting\\SQLTesting\\photo_image\\";

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
            if (createdFinish)
                return;
            Console.WriteLine("생성 완료!");

            string imgFile = "", userAddr = "";

            SQLitePCL.Batteries.Init();
            // ------------------------------------------넣어야할부분-------------------------------------------
            // var connectionStringFile = @"바뀌는 로그파일 주소";
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
                Console.WriteLine($"s_userAddr: {userAddr}");
                Console.WriteLine($"s_imgFile: {imgFile}");
                Console.WriteLine($"{path}");
            }

            if (imgFile == path)
            {
                SendMail sendMail = new SendMail();
                sendMail.S_Mail(e.FullPath, "whdgur1068@naver.com");
                //sendMail.S_Mail(e.FullPath, userAddr);
            }
            else
            {
                Console.WriteLine("전송 실패");
                Console.WriteLine("로그 파일 userAddr : " + userAddr);
                Console.WriteLine("로그 파일 imgFile : " + imgFile);
                Console.WriteLine("실제 사진 path : " + path);
            }
            createdFinish = true;
        }
    }
}
