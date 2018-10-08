using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Player
{
    public int id { get; set; }
    public string name { get; set; }
    public GameObject obj { get; set; }
    public string room { get; set; }

    public Player(int id, string name, GameObject obj, string room = "")
    {
        this.id = id;
        this.name = name;
        this.obj = obj;
        this.room = room;
    }
}

public class TCPPlayer {

    public static List<Player> players = new List<Player>();
    public static string my_name = "";
    public static int my_tcp_id = -1;
    public static string my_room_name = "";

    public static IEnumerator InstantiateMine(int id, string name, string room_name, Vector2 position)
    {
        my_name = name;
        my_tcp_id = id;
        my_room_name = room_name;
        GameObject go = GameObject.Instantiate((GameObject)Resources.Load("main_player", typeof(GameObject)), position, Quaternion.identity);
        players.Add(new Player(id, name, go, room_name));
        yield return null;
    }

    public static IEnumerator InstantiateOther(int id, string name, Vector2 position)
    {
        GameObject go = GameObject.Instantiate((GameObject)Resources.Load("main_player", typeof(GameObject)), position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().gravityScale = 0;
        players.Add(new Player(id, name, go));
        yield return null;
    }

    public static IEnumerator UpdateOther(int id, Vector2 position){
        GameObject other = GetPlayerById(id);
        if (other){
            GetPlayerById(id).transform.position = position;
        }
        else{
            Debug.Log("OTHER OBJ IS NULL");
        }
        yield return null;
    }

    public static void RemovePlayer(int id){
        foreach (Player p in players){
            if (p.id == id){
                GameObject.Destroy(p.obj);
                players.Remove(p);
            }
        }
    }

    public static string GetNameByObj(GameObject obj){
        foreach (Player p in players)
        {
            if (p.obj == obj){
                return p.name;
            }
        }
        return "";
    }

    public static GameObject GetPlayerById(int id){
        foreach (Player p in players)
        {
            if (p.id == id)
            {
                return p.obj;
            }
        }
        return null;
    }

    public static bool IdIsSet()
    {
        return my_tcp_id != -1;
    }

    public static bool IsMine(GameObject g)
    {
        return g == GetPlayerById(my_tcp_id);
    }
}
