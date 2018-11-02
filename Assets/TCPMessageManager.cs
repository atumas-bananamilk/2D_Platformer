using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TCPMessageManager {

    public static string FindMatch()
    {
        return "(find)";
    }
    public static string LeaveMatch()
    {
        return "(leave)";
    }
    public static string AssignPlayerId(ref string prefab, ref string room_name, Vector2 position, float velocity)
    {
        return "(assign:" + TCPPlayer.my_player.name + "," + room_name + "," + position.x + "," + position.y + "," + velocity + ")";
    }
    public static string SendMovementInfo(Vector2 position, ref float velocity)
    {
        return "(pos:" + TCPPlayer.my_player.id + "," + position.x.ToString("0.##") + "," + position.y.ToString("0.##") + "," + velocity.ToString("0.##") + ")";
    }
    public static string Disconnect()
    {
        return "(disconnect:" + TCPPlayer.my_player.id + ")";
    }



}
