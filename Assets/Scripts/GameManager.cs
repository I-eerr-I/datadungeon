using System.IO;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections;
using Random=UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject         player;
    public UIManager          uiManager;
    public TerminalController terminal;
    
    [Header("Tutorial")]
    public GameObject         tutorial;
    public bool               isTutorial;

    public IDbConnection      dbConnection;

    string           connection_string;
    IDbCommand       command;
    PlayerController player_controller;

    public enum UPGRADABLE {DOV, MH, LUCK};

    // Database constants
    const string   ROOMS_TABLENAME     = "rooms";
    const string   KEYS_TABLENAME      = "keys";
    const string   MONSTERS_TABLENAME  = "monsters";
    const string   ROOM_TABLENAME      = "room";
    string[]       MAIN_TABLES         = {ROOMS_TABLENAME,    
                                          KEYS_TABLENAME, 
                                          MONSTERS_TABLENAME
                                          };
    const string   MAZE_NAME           = "Maze.db";
    const string   ROOMS_TABLE         = @"
    (id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(45) UNIQUE,
    locked TINYINT NOT NULL DEFAULT 0)"; 
    const string   KEYS_TABLE          = @"
    (id INTEGER PRIMARY KEY,
    room_name VARCHAR(45))";
    const string   MONSTERS_TABLE      = @"
    (id INTEGER PRIMARY KEY,
    max_secrete INTEGER NOT NULL,
    min_secrete INTEGER NOT NULL)"; 
    const string   ROOM_TABLE          = @"
    (id INTEGER PRIMARY KEY,
    name VARCHAR(45) NOT NULL,
    type VARCHAR(45) NOT NULL,
    monster_id INTEGER DEFAULT NULL,
    key_id     INTEGER DEFAULT NULL)";

    // Rooms parameter
    int               key_id           = 0;
    int               rooms_amount;
    string[]          rooms;
    const int         MIN_ROOMS_AMOUNT = 2;
    const int         MAX_ROOMS_AMOUNT = 4;
    const string      DOOR_TYPENAME    = "door";
    const string      KEY_TYPENAME     = "key";
    const string      EXITDOOR_NAME    = "MAZE EXIT";

    // Monsters parameters
    int          monster_id                  = 0;
    const int    MIN_MONSTERS_AMOUNT         = 1;
    const int    MAX_MONSTERS_AMOUNT         = 2;
    const int    MIDDLE_MONSTER_LEVEL        = 2;
    const int    HARD_MONSTER_LEVEL          = 4;
    const int    MAX_SECRETE_NUMBER          = 100;
    const int    EASY_MIN_SECRETE_DISTANCE   = 40; 
    const int    EASY_MAX_SECRETE_DISTANCE   = 50;
    const int    MIDDLE_MIN_SECRETE_DISTANCE = 30;
    const int    MIDDLE_MAX_SECRETE_DISTANCE = 40;
    const int    HARD_MIN_SECRETE_DISTANCE   = 20;
    const int    HARD_MAX_SECRETE_DISTANCE   = 30; 
    const float  EASY_MONSTER_CHANCE         = 1f;
    const float  MIDDLE_MONSTER_CHANCE       = 0.5f;
    const float  HADR_MONSTER_CHANCE         = 0.25f;
    const string EASY_MONSTER_NAME           = "Easy";
    const string MIDDLE_MONSTER_NAME         = "Middle";
    const string HARD_MONSTER_NAME           = "Hard";
    const string MONSTER_TYPENAME            = "monster";

    // Items parameters
    const int      MIN_ITEMS_AMOUNT = 0;
    const int      MAX_ITEMS_AMOUNT = 2;
    const string   ITEM_TYPENAME    = "item";
    const string   WORM_TYPENAME    = "worm";
    const string   TROJAN_TYPENAME  = "trojan";
    const string   BUG_TYPENAME     = "bug";
    string[]       ITEMS            = {WORM_TYPENAME, TROJAN_TYPENAME, BUG_TYPENAME};

    int monster_killed   = 0;
    int max_level_points = 0;
    bool game_over       = false;
    bool ended_game      = false;
    bool wait_for_enter  = false;

    void Awake()
    {
        player            = Instantiate(player, player.transform.position, Quaternion.identity);
        player_controller = player.GetComponent<PlayerController>();
        uiManager.SetPlayerController(player_controller);
        terminal.SetPlayerController(player_controller);
        OpenConnection();
        command = dbConnection.CreateCommand();
        if(!SaveAndLoad())
        {
            isTutorial = true;
            tutorial.SetActive(true);
        }
        GenerateMaze();
    }

    void Update()
    {
        if(game_over)
        {
            uiManager.EndGameAnimation(monster_killed);
            ended_game = true;
            game_over  = false;
            wait_for_enter = true;
            terminal.ShowNewTextLoading("Press Enter To Continue...");
        }

        if(wait_for_enter && Input.GetButtonDown("Enter"))
        {
            Reload();
        }
    }

    void Reload()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void OpenConnection()
    {
        connection_string = "URI=file:" + Application.persistentDataPath + "/" + MAZE_NAME;
        dbConnection      = new SqliteConnection(connection_string);
        dbConnection.Open();
    }

    void OnApplicationQuit()
    {
        FullQuit();
    }

    bool SaveAndLoad()
    {
        SaveData data = SaveSystem.Load();
        if(data == null) 
        {
            player_controller.SetStartCharacteristics();
            return false;
        }
        player_controller.SetCharacteristics(data);
        return true;
    }

    void GenerateMaze() 
    {
        ReinitAllTables();
        rooms_amount = Random.Range(MIN_ROOMS_AMOUNT + player_controller.level, MAX_ROOMS_AMOUNT + player_controller.level);
        rooms = new string[rooms_amount];
        AddRooms();
        AddExitToRandomRoom();
        BlockRooms();
    }

    void ReinitAllTables()
    {
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name <> 'sqlite_sequence'";
        IDataReader reader  = command.ExecuteReader();
        ArrayList tables = new ArrayList();
        while(reader.Read())
        {
            tables.Add(reader.GetString(0));
        }
        reader.Close();
        foreach(string table in tables) 
        {
            command.CommandText = "DROP TABLE IF EXISTS " + table;
            command.ExecuteNonQuery();
        }
        foreach(string table in MAIN_TABLES)
        {
            command.CommandText = "DROP TABLE IF EXISTS " + table;
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table;
            if(table.Equals(ROOMS_TABLENAME))
            {
                command.CommandText += ROOMS_TABLE;
            }
            else if(table.Equals(KEYS_TABLENAME))
            {
                command.CommandText += KEYS_TABLE;
            }
            else if(table.Equals(MONSTERS_TABLENAME))
            {
                command.CommandText += MONSTERS_TABLE;
            }
            command.ExecuteNonQuery();
        }
    } 

    void AddMonstersToRoom(string room_name)
    {
        int monsters_amount = Random.Range(MIN_MONSTERS_AMOUNT + player_controller.level, MAX_MONSTERS_AMOUNT + player_controller.level);
        max_level_points += monsters_amount*player_controller.level*10;
        for(int m = 0; m < monsters_amount; m++)
        {
            command.CommandText  = "INSERT INTO "+MONSTERS_TABLENAME+"(id, min_secrete, max_secrete) VALUES (" + monster_id.ToString() + ", ";
            float monster_chance = Random.Range(0f, 1f)*(1-player_controller.luck);
            int[] secrete_range  = {};
            string monster_name  = "";
            if(monster_chance <= EASY_MONSTER_CHANCE && monster_chance > MIDDLE_MONSTER_CHANCE)
            {
                secrete_range = GetSecreteRange(EASY_MIN_SECRETE_DISTANCE, EASY_MAX_SECRETE_DISTANCE);
                monster_name  = GetHashedName(EASY_MONSTER_NAME, monster_id);
            }
            else if(monster_chance < MIDDLE_MONSTER_CHANCE && monster_chance >= HADR_MONSTER_CHANCE && MIDDLE_MONSTER_LEVEL <= player_controller.level)
            {
                secrete_range = GetSecreteRange(MIDDLE_MIN_SECRETE_DISTANCE, MIDDLE_MAX_SECRETE_DISTANCE);
                monster_name  = GetHashedName(MIDDLE_MONSTER_NAME, monster_id);
            }
            else if(monster_chance < HADR_MONSTER_CHANCE && HARD_MONSTER_LEVEL <= player_controller.level)
            {
                secrete_range = GetSecreteRange(HARD_MIN_SECRETE_DISTANCE, HARD_MAX_SECRETE_DISTANCE);
                monster_name  = GetHashedName(HARD_MONSTER_NAME, monster_id);
            }
            if(monster_name == "") 
            {
                monster_name  = GetHashedName(EASY_MONSTER_NAME, monster_id);
                secrete_range = GetSecreteRange(EASY_MIN_SECRETE_DISTANCE, EASY_MAX_SECRETE_DISTANCE);
            }
            command.CommandText += secrete_range[0].ToString() + ", " + secrete_range[1].ToString() + ")";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + room_name + "(name, type, monster_id) VALUES('" + monster_name + "', '"+MONSTER_TYPENAME+"', " + monster_id.ToString() + ")";
            command.ExecuteNonQuery();
            monster_id++;
        }
    }

    void AddItemsToRoom(string room_name)
    {
        int items_amount      = Random.Range(MIN_ITEMS_AMOUNT, MAX_ITEMS_AMOUNT);
        float add_item_chance = Random.Range(0f, 1f);
        if(add_item_chance < player_controller.luck) items_amount++;
        for (int i = 0; i < items_amount; i++)
        {
            int item_type = Random.Range(0, ITEMS.Length-1);
            if(ITEMS[item_type] == WORM_TYPENAME) InsertItemIntoRoom(room_name, WORM_TYPENAME);
            else if(ITEMS[item_type] == TROJAN_TYPENAME) InsertItemIntoRoom(room_name, TROJAN_TYPENAME);
            else if(ITEMS[item_type] == BUG_TYPENAME) InsertItemIntoRoom(room_name, BUG_TYPENAME);
        }
    }

    void AddRooms()
    {
        max_level_points += rooms_amount * player_controller.level*10;
        for (int i = 0; i < rooms_amount; i++)
        {
            string room_name    = "room" + i.ToString();
            InitRoom(room_name);
            AddMonstersToRoom(room_name);
            AddItemsToRoom(room_name);
            rooms[i] = room_name;
        }
    }

    void AddExitToRandomRoom()
    {
        int room_with_exit  = Random.Range(0, rooms_amount-1);
        command.CommandText = "INSERT INTO " + rooms[room_with_exit] + "(name, type) VALUES('" + EXITDOOR_NAME + "', '" + DOOR_TYPENAME + "')"; 
        command.ExecuteNonQuery();
    }

    void BlockRooms()
    {
        int blocked_min_amount = 0+player_controller.level;
        int blocked_max_amount = rooms_amount-1;
        if(blocked_min_amount > blocked_max_amount) blocked_min_amount = blocked_max_amount-1;
        int blocked_amount  = Random.Range(blocked_min_amount, blocked_max_amount);
        ArrayList blocked_rooms = new ArrayList();
        for(int i = 0; i < blocked_amount; i++)
        {
            bool found_not_blocked = false;
            int blocked_room = 0;
            while(!found_not_blocked)
            {
                blocked_room = Random.Range(1, rooms_amount-1);
                if(!blocked_rooms.Contains(blocked_room)) found_not_blocked = true;
            }
            command.CommandText = "INSERT INTO " + KEYS_TABLENAME + "(id, room_name) VALUES("+ key_id.ToString() + ", '" + rooms[blocked_room] +"')";
            command.ExecuteNonQuery();
            command.CommandText = "UPDATE " + ROOMS_TABLENAME + " SET locked = 1 WHERE name = '" + rooms[blocked_room] + "'";
            command.ExecuteNonQuery();
            string key_keeper = rooms[Random.Range(0, blocked_room-2)];  
            string key_name   = "key for " + rooms[blocked_room];
            command.CommandText = "INSERT INTO " + key_keeper + "(name, type, key_id) VALUES('" + key_name + "', '" + KEY_TYPENAME + "', " + key_id.ToString() + ")";   
            command.ExecuteNonQuery();
            key_id++;
            blocked_rooms.Add(blocked_room);
        }
    }

    void InsertItemIntoRoom(string room_name, string item)
    {
        command.CommandText = "INSERT INTO " + room_name + "(name, type) VALUES('" + item + "', '" + ITEM_TYPENAME + "')";
        command.ExecuteNonQuery(); 
    }

    void InitRoom(string room_name)
    {
        command.CommandText = "CREATE TABLE " + room_name + ROOM_TABLE;
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO " + ROOMS_TABLENAME + "(name) VALUES('" + room_name + "')";
        command.ExecuteNonQuery();
    }

    string GetHashedName(string name, int id)
    {
        return name+"_"+Mathf.Abs((name+id.ToString()).GetHashCode()).ToString();
    }

    int[] GetSecreteRange(int min_secrete_distance, int max_secrete_distance)
    {
        int secrete_distance = Random.Range(min_secrete_distance, max_secrete_distance);
        int min_secrete      = Random.Range(0, MAX_SECRETE_NUMBER-secrete_distance);
        int max_secrete      = min_secrete+secrete_distance;
        int[] result = {min_secrete, max_secrete};
        return result;
    }

    public long CountMonstersInRoom(string roomName)
    {
        command.CommandText = "SELECT Count(type) FROM " + roomName + " WHERE type = '" + MONSTER_TYPENAME + "'";
        IDataReader reader  = command.ExecuteReader();
        reader.Read();
        long result = reader.GetInt64(0);
        reader.Close();
        return result;
    }

    public bool IsTheRoomLocked(string roomName)
    {
        command.CommandText = "SELECT locked FROM rooms WHERE name = '"+roomName+"'";
        IDataReader reader  = command.ExecuteReader();
        int blocked = 0;
        if(reader.Read())
        {
            blocked = reader.GetInt32(0);
        }
        reader.Close();
        return (blocked == 1)? true : false; 
    }

    public bool PunchMonster(string monster_id, string room_name, int secrete, out bool is_lower)
    {
        player_controller.AttackAnimation();
        command.CommandText = "SELECT min_secrete, max_secrete FROM monsters WHERE id = "+monster_id;
        IDataReader reader  = command.ExecuteReader();
        bool is_punched = false;
        is_lower        = false;
        while(reader.Read())
        {
            long minimum = reader.GetInt64(0);
            long maximum = reader.GetInt64(1);
            if(secrete < minimum)
            {
                is_lower = false;
            }
            else if(secrete > maximum)
            {
                is_lower = true;
            }
            if(secrete >= minimum && secrete <= maximum)
            {
                is_punched = true;
            }
        }
        reader.Close();
        if(is_punched)
        {
            DeleteMonster(monster_id, room_name);
        }
        else
        {
            if(!player_controller.TakeMonsterAttack())
            {
                uiManager.MonsterMissed();
            }
        }
        return is_punched;
    }

    public string GetMonsterID(string room_monster_id, string room_name)
    {
        command.CommandText = "SELECT monster_id FROM "+room_name+" WHERE id = "+room_monster_id;
        IDataReader reader  = command.ExecuteReader();
        if(!reader.Read()) return "";
        string monster_id = reader.GetInt64(0).ToString();
        reader.Close();
        return monster_id;
    }

    public bool ExpandMonsterSecrete(string monster_id)
    {
        command.CommandText = "SELECT min_secrete, max_secrete FROM monsters WHERE id = "+monster_id;
        IDataReader reader  = command.ExecuteReader();
        if(!reader.Read()) return false;
        long min_secrete = reader.GetInt64(0);
        long max_secrete = reader.GetInt64(1);
        reader.Close();
        min_secrete = (long)((float)min_secrete - 0.25f*(float)min_secrete);
        max_secrete = (long)((float)max_secrete + 0.25*(float)max_secrete);
        if(min_secrete < 0) min_secrete = 0;
        if(max_secrete > MAX_SECRETE_NUMBER) max_secrete = MAX_SECRETE_NUMBER;
        command.CommandText = "UPDATE monsters SET min_secrete = "+min_secrete+", max_secrete = "+max_secrete+" WHERE id = "+monster_id;
        command.ExecuteNonQuery();
        return true;
    }

    public bool DeleteMonster(string monster_id, string room_name)
    {
        monster_killed++;
        player_controller.IncrementLevelPoints();
        command.CommandText = "DELETE FROM monsters WHERE id = "+monster_id;
        command.ExecuteNonQuery();
        command.CommandText = "DELETE FROM " + room_name + " WHERE monster_id = " + monster_id;
        command.ExecuteNonQuery();
        player_controller.IncrementVisionForRoom(room_name);
        return true;
    }

    public void SkipTutorial()
    {
        isTutorial = false;
        tutorial.SetActive(false);
    }
    
    public void Quit()
    {
        dbConnection.Close();
        SceneManager.LoadScene("Menu");
    }

    public void FullQuit()
    {
        dbConnection.Close();
        Application.Quit();
    }

    public void EndGame()
    {
        if(!ended_game)
        {
            game_over = true;
        }
    }

    public void UnlockTheRoom(string room_name)
    {
        command.CommandText = "UPDATE rooms SET locked = 0 WHERE name = '"+room_name+"'";
        command.ExecuteNonQuery();
    }

    public void DeleteItem(string item_id, string room_name)
    {
        command.CommandText = "DELETE FROM " + room_name + " WHERE id = " + item_id;
        command.ExecuteNonQuery();
    }

    public void Upgrade(UPGRADABLE upgrade_type)
    {
        player_controller.Upgrade(upgrade_type);
        SaveSystem.Save(player_controller);
        Reload();
    }

    public int GetKilledMonsters()
    {
        return monster_killed;
    }

    public int GetMaxLevelPoints()
    {
        return max_level_points;
    }

    public void ActivateWaitForEnterToEnd()
    {
        wait_for_enter = true;
    }
}