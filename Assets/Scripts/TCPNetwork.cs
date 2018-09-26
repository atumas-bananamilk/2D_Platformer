using System;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TCPNetwork {

    //private static readonly string IP = "35.180.106.179";
    //private static readonly Int32 PORT = 8999;
    private static readonly string IP = "127.0.0.1";
    private static readonly Int32 PORT = 8999;
    private static long ping = 0;
    private static Boolean Connected = false;

    private static TcpClient client;
    private static NetworkStream stream;
    private static Byte[] data;
    private static String response = String.Empty;
    private static System.Diagnostics.Stopwatch watch;
    private static Thread thread;

    public static void Connect(){
        if (!Connected){
            try
            {
                client = new TcpClient(IP, PORT);
                //client = new TcpClient();
                //IPEndPoint server_endpoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                //client.Connect(server_endpoint);

                stream = client.GetStream();
                Connected = true;
            }
            catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
            catch (SocketException e) { Debug.Log("SocketException: " + e); }
        }
    }

    public static void Instantiate(string prefab, Vector2 position, Quaternion rotation){
        thread = new Thread(() => AsyncInstantiate(prefab, position, rotation));
        thread.Start();
    }

    private static void AsyncInstantiate(string prefab, Vector2 position, Quaternion rotation){
        Connect();
        //SendPlayerInfo(prefab, position, rotation);
        Listen();
    }

    private static void SendPlayerInfo(string prefab, Vector2 position, Quaternion rotation){
        //Vector2 v = new Vector2(this.transform.position.x, this.transform.position.y);
        //GameObject obj = PhotonNetwork.Instantiate(bullet_prefab.name, v, Quaternion.identity, 0);

        Instantiate(prefab, position, rotation);
        string msg = position.ToString() + rotation.ToString();
        Send(msg);
    }

    public static void Listen(){
        while (true){
            try
            {
                data = new Byte[256];
                response = System.Text.Encoding.ASCII.GetString(data, 0, stream.Read(data, 0, data.Length));
                Debug.Log("RECEIVED: " + response);
            }
            catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
            catch (SocketException e) { Debug.Log("SocketException: " + e); }
        }
    }

    public static void Send(string msg){
        watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // send
            data = new Byte[256];
            data = System.Text.Encoding.ASCII.GetBytes(msg);
            stream.Write(data, 0, data.Length);
            Debug.Log("SENT: " + msg);

            // receive
            data = new Byte[256];
            response = System.Text.Encoding.ASCII.GetString(data, 0, stream.Read(data, 0, data.Length));
            //Debug.Log("RECEIVED: " + response);
        }
        catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
        catch (SocketException e) { Debug.Log("SocketException: " + e); }

        watch.Stop();
        ping = watch.ElapsedMilliseconds;
    }

    public static void Disconnect(){
        try
        {
            stream.Close();
            client.Close();
            Connected = false;
        }
        catch (ArgumentNullException e) { Debug.Log("ArgumentNullException: " + e); }
        catch (SocketException e) { Debug.Log("SocketException: " + e); }
    }

    public static long GetPing(){
        return ping;
    }
}