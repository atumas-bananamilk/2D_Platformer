using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player
{
    public int id { get; set; }
    public string name { get; set; }
    public GameObject obj { get; set; }
    public string room { get; set; }

    public Player(int id = -1, string name = "", GameObject obj = null, string room = "")
    {
        this.id = id;
        this.name = name;
        this.obj = obj;
        this.room = room;
    }
}

public class TCPPlayer {

    public static List<Player> players = new List<Player>();
    public static Player my_player = new Player(-1, "", null);

    public static IEnumerator InstantiateMine(int id, string name, string room_name, Vector2 position)
    {
        GameObject go = GameObject.Instantiate(LoadPrefabResource("main_player"), position, Quaternion.identity);
        my_player = new Player(id, name, go, room_name);
        players.Add(my_player);
        yield return null;
    }

    public static IEnumerator InstantiateOther(int id, string name, Vector2 position)
    {
        GameObject go = GameObject.Instantiate(LoadPrefabResource("main_player"), position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().gravityScale = 0;
        players.Add(new Player(id, name, go));
        yield return null;
    }

    private static GameObject LoadPrefabResource(string prefab){
        return (GameObject)Resources.Load(prefab, typeof(GameObject));
    }

    public static IEnumerator UpdateOther(int id, Vector2 position){
        GameObject other = GetPlayerGameObject(id);
        if (other){
            GetPlayerGameObject(id).transform.position = position;
        }
        else{
            Debug.Log("OTHER OBJ IS NULL");
        }
        yield return null;
    }

    public static void RemovePlayer(int id){
        for (int i = 0; i < players.Count; i++){
            if (players[i].id == id){
                GameObject.Destroy(players[i].obj);
                players.Remove(players[i]);
            }
        }
    }

    public static string GetNameByGameObj(GameObject obj){
        foreach (Player p in players)
        {
            if (p.obj == obj){
                return p.name;
            }
        }
        return "";
    }

    public static GameObject GetPlayerGameObject(int id){
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
        return my_player.id != -1;
    }

    public static bool IsMine(GameObject g)
    {
        return g == my_player.obj;
    }
}
