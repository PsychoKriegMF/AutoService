using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace AutoService
{
    public class DBWork
    {
        //static private string dbname = "Автосервис.db";
        static private string path = $"Data Source=Автосервис.db;";

        //CREATE TABLE IF NOT EXISTS Mechanics(
        //    id INTEGER PRIMARY KEY AUTOINCREMENT,
        //    number INTEGER,
        //    name VARCHAR);

        //INSERT INTO Mechanics(number , name)
        //    VALUES
        //    (1, 'Иванов'),
        //    (2, 'Петров'),
        //    (3, 'Сидоров'),
        //    (4, 'Кузнецов');

        //CREATE TABLE IF NOT EXISTS Jobs(
        //    jobs_id INTEGER PRIMARY KEY AUTOINCREMENT,
        //    number INTEGER,
        //    name VARCHAR,
        //    standardHours REAL,
        //    cost DECIMAL,
        //    client VARCHAR,
        //    Mechanics_id INTEGER,
        //    FOREIGN KEY (Mechanics_id) REFERENCES Mechanics (id));

        //INSERT INTO Jobs(number, name, standardHours, cost, client, Mechanics_id)
        //    VALUES
        //    ( 1,'Прокачка тормозной системы', 1.5, 3000, 'x001xx196', 1 ),
        //    ( 2,'Капиталка двигателя', 10, 6000, 'x002xx196', 2 ),
        //    ( 3,'Замена масла', 1, 1500, 'x003xx196', 3 ),
        //    ( 4,'Шиномонтаж', 2, 2000, 'x004xx196', 4 );

        static public bool MakeDB(string _dbname = "Автосервис.db")
        {
            bool result = false;
            string path = $"Data Source={_dbname};";
            string create_table_Mechanics = "CREATE TABLE IF NOT EXISTS " +
                "Mechanics " +
                "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "number INTEGER," +
                "name VARCHAR," +
                "Avatar BLOB);";
            string init_data_Mechanics = "INSERT INTO Mechanics (number, name) " +
                "VALUES " +
                "(1, 'Иванов')," +
                "(2, 'Петров')," +
                "(3, 'Сидоров')," +
                "(4, 'Кузнецов');";

            string create_table_Jobs = "CREATE TABLE IF NOT EXISTS " +
                "Jobs " +
                "(jobs_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "number INTEGER," +
                "name VARCHAR," +
                "standardHours REAL," +
                "cost DECIMAL," +
                "client VARCHAR," +
                "Mechanics_id INTEGER," +
                "FOREIGN KEY (Mechanics_id) REFERENCES Mechanics (id));";            

            string init_data_Jobs = "INSERT INTO Jobs " +
                "(number, name, standardHours, cost, client, Mechanics_id)" +
                "VALUES " +
                "(1,'Прокачка тормозной системы', 1.5, 3000, 'x001xx196', 1)," +
                "(2,'Капиталка двигателя', 10, 6000, 'x002xx196', 2)," +
                "(3, 'Замена масла', 1, 1500, 'x003xx196', 3)," +
                "(4,'Шиномонтаж', 2, 2000, 'x004xx196', 4);";



            //string show_all_data = "SELECT * FROM Category;";
            SQLiteConnection conn = new SQLiteConnection(path);
            SQLiteCommand cmd01 = conn.CreateCommand();
            SQLiteCommand cmd02 = conn.CreateCommand();
            SQLiteCommand cmd03 = conn.CreateCommand();
            SQLiteCommand cmd04 = conn.CreateCommand();

            cmd01.CommandText = create_table_Mechanics;
            cmd02.CommandText = init_data_Mechanics;
            cmd03.CommandText = create_table_Jobs;
            cmd04.CommandText = init_data_Jobs;

            conn.Open();
            cmd01.ExecuteNonQuery();
            cmd02.ExecuteNonQuery();
            cmd03.ExecuteNonQuery();
            cmd04.ExecuteNonQuery();
            conn.Close();
            result = true;
            return result;
        }

        static public List<string> GetMechanics()
        {
            List<string> result = new List<string>();
            string get_Mechanics = "SELECT name FROM Mechanics;";
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = get_Mechanics;
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }            
            return result;
        }

        static public void AddAvatar(string _name, byte[] _image)
        {
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                SQLiteCommand command = new SQLiteCommand(conn);
                command.CommandText = @"UPDATE Mechanics SET Avatar=@Avatar " +
                    $"WHERE Name LIKE '{_name}%';";
                    command.Parameters.Add(new SQLiteParameter("@Avatar", _image));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        static public MemoryStream GetAvatar(string _name)
        {
            MemoryStream result = null;
            byte[] _image = null;
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                string get_image = $"SELECT Avatar FROM Mechanics WHERE Name LIKE '{_name}%';";
                cmd.CommandText = get_image;
                conn.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if(!reader.IsDBNull(0))
                        {
                            _image = (byte[])reader.GetValue(0);
                        }
                    }
                }
            }
            if(_image != null)
            {
                result = new MemoryStream(_image);
            }
            return result;
        }

        static public void AddData(string _newCategoryInsert, string _dbname = "Автосервис.db")
        {
            string path = $"Data Source={_dbname};";
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = _newCategoryInsert;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        static public DataSet Refresh(string _dbname = "Автосервис.db")
        {
            DataSet result = new DataSet();
            string path = $"Data Source={_dbname};";
            string show_all_data = "SELECT * FROM Category;";
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(show_all_data, conn);
                adapter.Fill(result);
            }
            return result;
        }

        static public void Save(DataTable dt, out string _query, string _dbname = "Автосервис.db")
        {
            string path = $"Data Source={_dbname};";
            string show_all_data = "SELECT * FROM Category;";
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(show_all_data, conn);
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter);
                adapter.Update(dt);
                _query = commandBuilder.GetUpdateCommand().CommandText;
            }

        }
    }
}
