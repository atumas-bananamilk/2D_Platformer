using System;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;

public class TCPNetwork : MonoBehaviour
{

    //private static readonly string IP = "35.180.106.179";
    private static readonly string IP = "127.0.0.1";
    private static readonly Int32 PORT = 9999;
    private static long ping = 0;
    public static Boolean Connected = false;

    private static TcpClient client;
    private static NetworkStream stream;
    private static Byte[] comm_data;
    private static System.Diagnostics.Stopwatch watch;
    private static Thread thread;

    private static string l_response = String.Empty;
    private static string[] l_response_split = { };

    private static List<string> l_cmds = new List<string>();
    private static List<string> l_data = new List<string>();

    private static Regex regex = new Regex(@"\(.*?\)");
    private static MatchCollection matches;

    public void Connect()
    {
        if (!Connected)
        {
            try
            {
                client = new TcpClient(IP, PORT);
                stream = client.GetStream();
                Connected = true;
                Debug.Log("TCP: CONNECTED");
                AsyncListen();
            }
            catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
            catch (SocketException e) { Debug.Log("SocketException: " + e); }
        }
    }

    public void Instantiate(string prefab, string room_name, Vector2 position, Quaternion rotation)
    {
        if (Connected){
            float velocity = 0;
            AsyncSend(TCPMessageManager.AssignPlayerId(ref prefab, ref room_name, position, velocity));
        }
    }

    public void FindMatch(){
        AsyncSend(TCPMessageManager.FindMatch());
    }

    public void LeaveMatch(){
        AsyncSend(TCPMessageManager.LeaveMatch());
    }

    //public static void AssignPlayerId(ref string prefab, ref string room_name, Vector2 position, float velocity)
    //{
    //    AsyncSend("(assign:" + TCPPlayer.my_player.name + "," + room_name + "," + position.x + "," + position.y + "," + velocity + ")");
    //}

    public static void SendMovementInfo(Vector2 position, ref float velocity)
    {
        if (TCPPlayer.IdIsSet())
        {
            AsyncSend(TCPMessageManager.SendMovementInfo(position, ref velocity));
        }
        else
        {
            Debug.Log("NO PLAYER ID");
        }
    }

    public IEnumerator DestroyPlayer(int id)
    {
        TCPPlayer.RemovePlayer(id);
        yield return null;
    }

    private void AsyncListen()
    {
        thread = new Thread(() => Listen());
        thread.Start();
    }

    private static void AsyncSend(string msg)
    {
        thread = new Thread(() => Send(msg));
        thread.Start();
    }

    public void SetupPlayers(int id)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerMove>().SetupPlayers();
    }

    public void SetCorrectCanvas(int id)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerHealthBar>().SetCorrectCanvas();
    }

    public IEnumerator UpdateMatchIndicators(int match_players_count)
    {
        gameObject.GetComponent<MatchManager>().IndicateReadyPlayers(match_players_count);
        yield return null;
    }

    public IEnumerator LeaveMatchQueue()
    {
        gameObject.GetComponent<MatchManager>().ClearIndicators();
        yield return null;
    }

    public IEnumerator StartGame(string room_name, int spawn_location_id)
    {
        gameObject.GetComponent<MatchManager>().GoToRoom(room_name, spawn_location_id);
        yield return null;
    }

    public IEnumerator InstantiatePlayer(int id, string player_name, string room_name, Vector2 position)
    {
        TCPPlayer.InstantiatePlayer(id, player_name, room_name, position);
        SetupPlayers(id);
        SetCorrectCanvas(id);
        yield return null;
    }

    private void Listen()
    {
        while (true)
        {
            try
            {
                comm_data = new Byte[256];
                l_response = Encoding.ASCII.GetString(comm_data, 0, stream.Read(comm_data, 0, comm_data.Length));
                if (l_response.Length > 0)
                {
                    Debug.Log("RECEIVED: " + l_response);
                    ProcessReceivedData(l_response);
                }
            }
            catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
            catch (SocketException e) { Debug.Log("SocketException: " + e); }
        }
    }

    private void ProcessReceivedData(string data)
    {
        //string str = "(a)(b)(c)()";
        matches = regex.Matches(data);
        for (int i = 0; i < matches.Count; i++)
        {
            string v = matches[i].ToString().Split('(', ')')[1];
            if (v.Length > 0)
            {
                string[] entries = v.Split(':');
                if (entries.Length < 2){
                    ProcessMessages(entries[0]);
                }
                else{
                    ProcessMessages(entries[0], entries[1]);
                }
            }
        }
    }

    private void ProcessMessages(string cmd = "", string data_str = "")
    {
        string[] data = data_str.Split(',');
        int id;
        Int32.TryParse(data[0], out id);

        if (cmd == "find")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                UpdateMatchIndicators(Int32.Parse(data[0]))
            );
        }
        else if (cmd == "leave"){
            // someone else left
            if (data[0].Length > 0){
                int players_waiting = Int32.Parse(data[0]);

                UnityMainThreadDispatcher.Instance().Enqueue(
                    UpdateMatchIndicators(players_waiting)
                );
            }
            // this user left
            else{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    LeaveMatchQueue()
                );
            }
        }
        else if (cmd == "start")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                StartGame(data[0], Int32.Parse(data[1]))
            );
        }
        else if (cmd == "assign")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                InstantiatePlayer(id, data[1], data[2], new Vector2(float.Parse(data[3]), float.Parse(data[4])))
            );
        }
        else if (cmd == "init")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                InstantiatePlayer(id, data[1], "", new Vector2(float.Parse(data[3]), float.Parse(data[4])))
            );
        }
        else if (cmd == "pos")
        {
            if (id != TCPPlayer.my_player.id)
            {
                //Debug.Log("UPDATING OTHER AT: (" + Int32.Parse(data[0]) + "," + float.Parse(data[1]) + "," + float.Parse(data[2]) + ")");
                UnityMainThreadDispatcher.Instance().Enqueue(
                    TCPPlayer.UpdateOther(id, new Vector2(float.Parse(data[1]), float.Parse(data[2])))
                );
            }
        }
        else if (cmd == "disconnect")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                DestroyPlayer(id)
            );
        }
        else
        {

        }
    }

    private static void Send(string msg)
    {
        watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            comm_data = new Byte[256];
            comm_data = Encoding.ASCII.GetBytes(msg);
            stream.Write(comm_data, 0, comm_data.Length);
            //Debug.Log("SENT: " + msg);
        }
        catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
        catch (SocketException e) { Debug.Log("SocketException: " + e); }

        watch.Stop();
        ping = watch.ElapsedMilliseconds;
    }

    public static void Disconnect()
    {
        try
        {
            AsyncSend(TCPMessageManager.Disconnect());
            Debug.Log("DISCONNECTING");
            stream.Close();
            client.Close();
            Connected = false;
        }
        catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
        catch (SocketException e) { Debug.Log("SocketException: " + e); }
    }

    public static long GetPing()
    {
        return ping;
    }
}