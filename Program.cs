using System.IO;
using Microsoft.Data.Sqlite;

namespace Testing
{

    public class FileWatcher
    {
        static void Main(string[] args)
        {
            int pressNum = 0;
            FileWatcher filewatcher = new FileWatcher();
            filewatcher.InitWatcher();

            SQLitePCL.Batteries.Init();

            var connectrionStringMemory = "Data Source=:memory:";
            //var connectionStringFile = @"Data Source=E:\판교박물관 포토홀리\potoHoli\data\holilog.dat";
            var connectionStringFile = @"Data Source=D:\WatcherTesting\SQLTesting\mydatabase.dat";

            using var connection = new SqliteConnection(connectionStringFile);
            connection.Open();

            while (true)
            {
                Console.WriteLine("Press Number");

                switch (Console.ReadLine())
                {
                    case "0":
                        {
                            // 테이블 만들기
                            using var cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS MyTable " +
                            "(ID INTEGER PRIMARY KEY, s_userAddr TEXT);", connection);
                            cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("테이블 생성 완료");
                        break;

                    case "1":
                        {
                            // 테이블에 이메일 넣기
                            Console.WriteLine("Press E-mail");
                            string e_mail = Console.ReadLine();
                            using var paramCmd = new SqliteCommand("INSERT INTO MyTable (s_userAddr)" +
                                "VALUES (@name);", connection);
                            paramCmd.Parameters.AddWithValue("@name", $"{e_mail}");
                            paramCmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("이메일 기입 완료");
                        break;
                    case "2":
                        {
                            // 테이블에 마지막 행 삭제
                            //using var deleteCmd = new SqliteCommand("DELETE FROM MyTable");
                            //int result = deleteCmd.ExecuteNonQuery();
                            using var transaction = connection.BeginTransaction();
                            try
                            {
                                using var deleteCmd = new SqliteCommand("DELETE FROM MyTable");
                                deleteCmd.ExecuteNonQuery();
                                transaction.Commit();
                                Console.WriteLine("삭제 성공");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("삭제 실패");
                                transaction.Rollback();
                            }
                        }
                        break;
                    case "3":
                        {
                            //using var readCmd = new SqliteCommand("SELECT * FROM TB_RECEIVE ORDER BY ROWID DESC LIMIT 1;", connection);
                            using var readCmd = new SqliteCommand("SELECT * FROM MyTable;", connection);
                            using var reader = readCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["ID"]}, s_userAddr: {reader["s_userAddr"]}");
                            }
                            //Console.WriteLine($"ID: {reader["ID"]}, s_userAddr: {reader["s_userAddr"]}");
                        }
                        break;
                    default:
                        Console.WriteLine("끝");
                        return;
                }



            }


            //using var transaction = connection.BeginTransaction();
            //try
            //{
            //    using var cmd1 = new SqliteCommand("INSERT INTO MyTable (Name)" +
            //    "VALUES (@name);", connection, transaction);
            //    cmd1.Parameters.AddWithValue("@name", "Daniel");
            //    cmd1.ExecuteNonQuery();

            //    using var cmd2 = new SqliteCommand("INSERT INTO MyTable (Name)" +
            //    "VALUES (@name);", connection, transaction);
            //    cmd2.Parameters.AddWithValue("@name", "Daniel");
            //    cmd2.ExecuteNonQuery();

            //    transaction.Commit();
            //}
            //catch (Exception)
            //{
            //    transaction.Rollback();
            //}


            //using var readCmd = new SqliteCommand("SELECT * FROM TB_RECEIVE ORDER BY ROWID DESC LIMIT 1;", connection);
            //using var readCmd = new SqliteCommand("SELECT * FROM MyTable;", connection);
            //using var reader = readCmd.ExecuteReader();
            //while(reader.Read())
            //{
            //    //if (reader["ID"].ToString() == "20")
            //    Console.WriteLine($"ID: {reader["ID"]}, Name: {reader["Name"]}");
            //}
            //Console.WriteLine($"ID: {reader["ID"]}, s_userAddr: {reader["s_userAddr"]}");
        }

        public void InitWatcher()
        {
            string filePath = $"D:\\WatcherTesting\\SQLTesting\\";

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = filePath;

            watcher.NotifyFilter = NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.Size |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite;

            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;

            watcher.Created += new FileSystemEventHandler(Changed);
            watcher.Changed += new FileSystemEventHandler(Changed);
            watcher.Renamed += new RenamedEventHandler(Renamed);

            watcher.EnableRaisingEvents = true;
        }

        private void Changed(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("감지완료!");

            SQLitePCL.Batteries.Init();

            var connectrionStringMemory = "Data Source=:memory:";
            var connectionStringFile = @"Data Source=D:\WatcherTesting\SQLTesting\mydatabase.dat";

            using var connection = new SqliteConnection(connectionStringFile);
            connection.Open();

            using var readCmd = new SqliteCommand("SELECT * FROM MyTable ORDER BY ROWID DESC LIMIT 1;", connection);
            using var reader = readCmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"크아악 : {reader["s_userAddr"]}");
                SendMail sendMail = new SendMail();
                sendMail.S_Mail(@"D:\WatcherTesting\SQLTesting\Testing.docx", $"{reader["s_userAddr"]}");
            }
        }

        private void Renamed(object source, FileSystemEventArgs e)
        {
            Console.Write(e.FullPath);
        }
    }


}
