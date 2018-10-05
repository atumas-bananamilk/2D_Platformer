using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TCPPlayer {
    
    public static Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public static int my_tcp_id = -1;

    public static IEnumerator InstantiateMine(int id, Vector2 position)
    {
        my_tcp_id = id;
        GameObject prefabGo = (GameObject)Resources.Load("main_player", typeof(GameObject));
        GameObject go = GameObject.Instantiate(prefabGo, position, Quaternion.identity);
        players.Add(id, go);
        yield return null;
    }

    public static IEnumerator InstantiateOther(int id, Vector2 position)
    {
        GameObject prefabGo = (GameObject)Resources.Load("main_player", typeof(GameObject));
        GameObject go = GameObject.Instantiate(prefabGo, position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().gravityScale = 0;
        players.Add(id, go);
        yield return null;
    }

    public static IEnumerator UpdateOther(int id, Vector2 position){
        GameObject other_obj = GetPlayerById(id);
        if (other_obj){
            GetPlayerById(id).transform.position = position;
        }
        else{
            Debug.Log("OTHER OBJ IS NULL");
        }
        yield return null;
    }

    public static GameObject GetPlayerById(int id){
        foreach (KeyValuePair<int, GameObject> entry in players)
        {
            if (entry.Key == id){
                return entry.Value;
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
        return g == TCPPlayer.GetPlayerById(TCPPlayer.my_tcp_id);
    }
}
