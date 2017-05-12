using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System;
using System.Collections.Generic;

namespace SKUD
{
    public class SQLite
    {
        public DataTable t;

        public SQLite()
        {
            if (!File.Exists("base.sqlite"))
            {
                try
                {
                    string baseName = "base.sqlite";
                    SQLiteConnection.CreateFile(baseName);

                    //
                    //SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");

                    //SQLiteConnection conn = new SQLiteConnection()
                    //using (SQLiteConnection conn = (SQLiteConnection)factory.CreateConnection())
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source = " + baseName))
                    {
                        //conn.ConnectionString = "Data Source = " + baseName;
                        conn.Open();

                        using (SQLiteCommand command = new SQLiteCommand(conn))
                        {
                            command.CommandText = @"CREATE TABLE [users] (
                            [_id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [username] char(20),
                            [password] char(20)
                            );";
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE [employees] (
                            [_id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [numb] char(6),
                            [name] char(20),
                            [family] char(20),
                            [patr] char(20),
                            [dolj] char(20),
                            [otdel] char(20)
                            );";
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE [graf] (
                            [_id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [numb] char(6),
                            [in] char(20),
                            [out] char(20)
                            );";
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE [journal] (
                            [_id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [numb] char(6),
                            [in] char(20),
                            [out] char(20),
                            [dot] char(20)
                            );";
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();

                            command.CommandText = @"INSERT INTO [users](username, password) values('admin','1111')";
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }

                        conn.Close();
                        //
                    }
                }
                catch { }
            }

        }

        public List<List<string>> RQuery(string SQL)
        {
            List<List<string>> Result = new List<List<string>>();
            

            try
            {
                string baseName = "base.sqlite";
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = " + baseName))
                {
                    //conn.ConnectionString = "Data Source = " + baseName;
                    conn.Open();

                    SQLiteCommand command = new SQLiteCommand(conn);

                    command.CommandText = @SQL;
                    command.CommandType = CommandType.Text;

                    SQLiteDataReader reader = command.ExecuteReader();

                    using (SQLiteDataAdapter a = new SQLiteDataAdapter(SQL, conn))
                    {
                        t = new DataTable();
                        a.Fill(t);
                    }

                    int i = 0;
                    while (reader.Read())
                    {
                        Result.Add(new List<string>());

                        int j = 0;
                        while (j != reader.FieldCount)
                        {
                            Result[i].Add(reader.GetValue(j).ToString());
                            j += 1;
                        }

                        i += 1;
                    }

                    reader.Close();

                    conn.Close();
                }
                return Result;
            }
            catch
            {
                return Result;
            }         
        }

        public bool WQuery(string SQL)
        {
            try
            {
                string baseName = "base.sqlite";
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = " + baseName))
                {
                    //conn.ConnectionString = "Data Source = " + baseName;
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        command.CommandText = @SQL;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    conn.Close();
                    //
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
