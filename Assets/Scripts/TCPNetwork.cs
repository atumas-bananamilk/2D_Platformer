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

    Regex regex = new Regex(@"\(.*?\)");
    MatchCollection matches;

    public bool Connect()
    {
        if (!Connected)
        {
            try
            {
                client = new TcpClient(IP, PORT);
                stream = client.GetStream();
                Connected = true;
                Debug.Log("TCP: CONNECTED");
                return true;
            }
            catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
            catch (SocketException e) { Debug.Log("SocketException: " + e); }
        }
        return false;
    }

    public void Instantiate(string prefab, string room_name, Vector2 position, Quaternion rotation)
    {
        if (Connected){
            float velocity = 0;
            AssignPlayerId(ref prefab, ref room_name, position, velocity);
            AsyncListen();
        }
    }

    public static void AssignPlayerId(ref string prefab, ref string room_name, Vector2 position, float velocity)
    {
        AsyncSend("(assign:" + TCPPlayer.my_name + "," + room_name + "," + position.x + "," + position.y + "," + velocity + ")");
    }

    public static void SendMovementInfo(Vector2 position, ref float velocity)
    {
        if (TCPPlayer.IdIsSet())
        {
            AsyncSend("(pos:" + TCPPlayer.my_tcp_id + "," + position.x.ToString("0.##") + "," + position.y.ToString("0.##") + "," + velocity.ToString("0.##") + ")");
        }
        else
        {
            Debug.Log("NO PLAYER ID");
        }
    }

    public IEnumerator DestroyPlayer(int id)
    {
        TCPPlayer.RemovePlayer(id);
        //Destroy(TCPPlayer.GetPlayerById(id));
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

    public IEnumerator SetupPlayers(int id)
    {
        TCPPlayer.GetPlayerById(id).GetComponent<playerMove>().SetupPlayers();
        yield return null;
    }

    public IEnumerator SetCorrectCanvas(int id)
    {
        TCPPlayer.GetPlayerById(id).GetComponent<playerHealthBar>().SetCorrectCanvas();
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
                ProcessMessages(entries[0], entries[1]);
            }
        }
    }

    private void ProcessMessages(string cmd, string data_str)
    {
        string[] data = data_str.Split(',');
        int id = Int32.Parse(data[0]);

        if (cmd == "assign")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(
                TCPPlayer.InstantiateMine(id, data[1], data[2], new Vector2(float.Parse(data[3]), float.Parse(data[4])))
            );
            UnityMainThreadDispatcher.Instance().Enqueue(
                SetupPlayers(id)
            );
            UnityMainThreadDispatcher.Instance().Enqueue(
                SetCorrectCanvas(id)
            );
        }
        else if (cmd == "init")
        {
            //Debug.Log("INSTANTIATING OTHER AT: (" + Int32.Parse(data[0]) + "," + float.Parse(data[1]) + "," + float.Parse(data[2]) + ")");
            UnityMainThreadDispatcher.Instance().Enqueue(
                TCPPlayer.InstantiateOther(id, data[1], new Vector2(float.Parse(data[2]), float.Parse(data[3])))
            );
            UnityMainThreadDispatcher.Instance().Enqueue(
                SetupPlayers(id)
            );
            UnityMainThreadDispatcher.Instance().Enqueue(
                SetCorrectCanvas(id)
            );
        }
        else if (cmd == "pos")
        {
            if (id != TCPPlayer.my_tcp_id)
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
            AsyncSend("(disconnect:" + TCPPlayer.my_tcp_id + ")");
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