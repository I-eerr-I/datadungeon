using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    const string MAZE_NAME  = "Maze.db";

    string        connectionString;
    IDbConnection dbConnection;

    PlayerController playerController;

    string[] tables = {"Rooms", "Keys", "Monsters", "Inventory", "Status", "MyKeys"};

    void Start()
    {
        player           = Instantiate(player, player.transform.position, Quaternion.identity);
        playerController = player.GetComponent<PlayerController>();
        GenerateMaze();
    }

    void Update()
    {
        
    }

    void GenerateMaze() 
    {
        connectionString = "URI=file:" + Application.persistentDataPath + "/" + MAZE_NAME;
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        IDbCommand command  = dbConnection.CreateCommand();
        foreach(string table in tables)
        {
            command.CommandText = "DROP TABLE IF EXISTS " + table;
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table;
            if(table.Equals("Rooms"))
            {
                command.CommandText += "(id INTEGER PRIMARY KEY AUTOINCREMENT, ";
                command.CommandText += "name VARCHAR(45) UNIQUE, ";
                command.CommandText += "blocked TINYINT NOT NULL DEFAULT 0)";
            }
            else if(table.Equals("Keys"))
            {
                command.CommandText += "(id INTEGER PRIMARY KEY AUTOINCREMENT, ";
                command.CommandText +=   "user VARCHAR(45) NOT NULL UNIQUE, ";
                command.CommandText +=   "password VARCHAR(45) NOT NULL UNIQUE, ";
                command.CommandText +=   "room_id INTEGER, ";
                command.CommandText +=   "FOREIGN KEY(room_id) REFERENCES rooms (id))";
            }
            else if(table.Equals("Monsters"))
            {
                command.CommandText += "(id INTEGER PRIMARY KEY AUTOINCREMENT, ";
                command.CommandText += "max_secrete INTEGER NOT NULL, ";
                command.CommandText += "min_secrete INTEGER NOT NULL)";
            }
            else if(table.Equals("Inventory"))
            {
                command.CommandText += "(trojan INTEGER DEFAULT 0, ";
                command.CommandText +=   "worm INTEGER DEFAULT 0, ";
                command.CommandText +=   "bug INTEGER DEFAULT 0)";
            }
            else if(table.Equals("Status"))
            {
                command.CommandText += "(hp INTEGER NOT NULL DEFAULT " + playerController.GetMinDepthOfVision().ToString() + ", ";
                command.CommandText +=   "deapth_of_vision INTEGER NOT NULL DEFAULT " + playerController.GetMinDepthOfVision().ToString() + ", ";
                command.CommandText +=   "max_hp INTEGER NOT NULL DEFAULT " + playerController.GetMinMaxHealth().ToString() + ", ";
                command.CommandText +=   "luck REAL NOT NULL DEFAULT " + playerController.GetMinLuck().ToString() + ")";
            }   
            else if(table.Equals("MyKeys"))
            {
                command.CommandText += "(key_id INTEGER, ";
                command.CommandText +=   "FOREIGN KEY(key_id) REFERENCES door_keys(id))";
            }
            command.ExecuteNonQuery();
        }
    }
        //DB Work Example
        // IDbCommand dbCommand;
        // IDataReader reader;
        // dbCommand = dbConnection.CreateCommand();
        // string qCreateTable = "CREATE TABLE IF NOT EXISTS test (id INTEGER PRIMARY KEY, val INTEGER)";
        // dbCommand.CommandText = qCreateTable;
        // reader = dbCommand.ExecuteReader();
        // IDbCommand command = dbConnection.CreateCommand();
        // command.CommandText = "INSERT INTO test (id, val) VALUES (0,5)";
        // command.ExecuteNonQuery();
        // IDbCommand commandRead = dbConnection.CreateCommand();
        // string query = "SELECT * FROM test";
        // commandRead.CommandText = query;
        // reader = commandRead.ExecuteReader();
        // while(reader.Read())
        // {
        //     Debug.Log("id: " + reader[0].ToString());
        //     Debug.Log("val: " + reader[1].ToString());
        // }
}
