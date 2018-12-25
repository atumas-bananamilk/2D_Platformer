﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TCPMessageManager : MonoBehaviour {

    private enum CMD{
        FIND, LEAVE, START, ASSIGN, INIT, POS, DISCONNECT, SHOOT, DAMAGE, KILL, FLIP, STATE
    }

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
    public static string Shoot(int id){
        return "(shoot: " + id + ")";
    }
    public static string ApplyDamage(int shooter_id, int receiver_id, ref float amount){
        return "(damage:" + shooter_id + "," + receiver_id + "," + amount + ")";
    }
    public static string KillPlayer(int id){
        return "(kill:" + id + ")";
    }
    public static string FlipPlayer(int id, playerMove.MOVEMENT_DIRECTION d){
        return "(flip:" + id + "," + d + ")";
    }
    public static string ChangePlayerState(int id, playerMove.PLAYERSTATE s){
        return "(state:" + id + "," + s + ")";
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

    public void SetupPlayers(int id)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerMove>().SetupPlayers();
    }

    public void SetCorrectCanvas(int id)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerHealthBar>().SetCorrectCanvas();
    }

    public IEnumerator DestroyPlayer(int id)
    {
        TCPPlayer.RemovePlayer(id);
        yield return null;
    }

    public IEnumerator ShootFromPlayer(int id)
    {
        if (id != TCPPlayer.my_player.id){
            TCPPlayer.GetPlayerGameObject(id).GetComponent<PlayerWeaponManager>().Shoot();
        }
        //TCPPlayer.GetPlayerGameObject(id).GetComponent<PlayerWeaponManager>().Shoot();
        yield return null;
    }

    public IEnumerator Return_ApplyDamageToPlayer(int shooter_id, int receiver_id, float amount)
    {
        TCPPlayer.GetPlayerGameObject(receiver_id).GetComponent<playerHealthBar>().ReduceHealth(amount);
        yield return null;
    }

    public IEnumerator Return_KillPlayer(int id)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerHealthBar>().KillPlayer();
        yield return null;
    }

    public IEnumerator Return_FlipPlayer(int id, playerMove.MOVEMENT_DIRECTION direction)
    {
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerMove>().FlipPlayer(direction);
        yield return null;
    }

    public IEnumerator Return_ChangePlayerState(int id, playerMove.PLAYERSTATE state)
    {
        Debug.Log("CHANGING PLAYER ID: " + id + ", STATE TO: "+state);
        TCPPlayer.GetPlayerGameObject(id).GetComponent<playerMove>().ChangePlayerState(state);
        yield return null;
    }

    public void ProcessMessages(string cmd_str = "", string data_str = "")
    {
        string[] data = data_str.Split(',');
        int id;
        Int32.TryParse(data[0], out id);
        CMD cmd = (CMD) Enum.Parse(typeof(CMD), cmd_str.ToUpper());

        switch(cmd){
            case CMD.FIND:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    UpdateMatchIndicators(Int32.Parse(data[0]))
                );
                break;
            }
            case CMD.LEAVE:{
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
                break;
            }
            case CMD.START:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    StartGame(data[0], Int32.Parse(data[1]))
                );
                break;
            }
            case CMD.ASSIGN:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    InstantiatePlayer(id, data[1], data[2], new Vector2(float.Parse(data[3]), float.Parse(data[4])))
                );
                break;
            }
            case CMD.INIT:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    InstantiatePlayer(id, data[1], "", new Vector2(float.Parse(data[3]), float.Parse(data[4])))
                );
                break;
            }
            case CMD.POS:{
                if (id != TCPPlayer.my_player.id){
                    //Debug.Log("UPDATING OTHER AT: (" + Int32.Parse(data[0]) + "," + float.Parse(data[1]) + "," + float.Parse(data[2]) + ")");
                    UnityMainThreadDispatcher.Instance().Enqueue(
                        TCPPlayer.UpdateOther(id, new Vector2(float.Parse(data[1]), float.Parse(data[2])))
                    );
                }
                break;
            }
            case CMD.DISCONNECT:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    DestroyPlayer(id)
                );
                break;
            }
            case CMD.SHOOT:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    ShootFromPlayer(id)
                );
                break;
            }
            case CMD.DAMAGE:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    Return_ApplyDamageToPlayer(id, Int32.Parse(data[1]), float.Parse(data[2]))
                );
                break;
            }
            case CMD.KILL:{
                UnityMainThreadDispatcher.Instance().Enqueue(
                    Return_KillPlayer(id)
                );
                break;
            }
            case CMD.FLIP:{
                    playerMove.MOVEMENT_DIRECTION d = 
                        (playerMove.MOVEMENT_DIRECTION) Enum.Parse(typeof(playerMove.MOVEMENT_DIRECTION), data[1]);
                    
                    UnityMainThreadDispatcher.Instance().Enqueue(
                            Return_FlipPlayer(id, d)
                );
                break;
            }
            case CMD.STATE:{
                    playerMove.PLAYERSTATE s = 
                        (playerMove.PLAYERSTATE) Enum.Parse(typeof(playerMove.PLAYERSTATE), data[1]);
                    
                    UnityMainThreadDispatcher.Instance().Enqueue(
                            Return_ChangePlayerState(id, s)
                );
                break;
            }
            default:{
                break;
            }
        }

        //if (cmd == "find")
        //if ()
        //{
        //    UnityMainThreadDispatcher.Instance().Enqueue(
        //        UpdateMatchIndicators(Int32.Parse(data[0]))
        //    );
        //}
        //else if (cmd == "leave")
        //{
        //    // someone else left
        //    if (data[0].Length > 0)
        //    {
        //        int players_waiting = Int32.Parse(data[0]);

        //        UnityMainThreadDispatcher.Instance().Enqueue(
        //            UpdateMatchIndicators(players_waiting)
        //        );
        //    }
        //    // this user left
        //    else
        //    {
        //        UnityMainThreadDispatcher.Instance().Enqueue(
        //            LeaveMatchQueue()
        //        );
        //    }
        //}
        //else if (cmd == "start")
        //{
        //    UnityMainThreadDispatcher.Instance().Enqueue(
        //        StartGame(data[0], Int32.Parse(data[1]))
        //    );
        //}
        //else if (cmd == "assign")
        //{
        //    UnityMainThreadDispatcher.Instance().Enqueue(
        //        InstantiatePlayer(id, data[1], data[2], new Vector2(float.Parse(data[3]), float.Parse(data[4])))
        //    );
        //}
        //else if (cmd == "init")
        //{
        //    UnityMainThreadDispatcher.Instance().Enqueue(
        //        InstantiatePlayer(id, data[1], "", new Vector2(float.Parse(data[3]), float.Parse(data[4])))
        //    );
        //}
        //else if (cmd == "pos")
        //{
        //    if (id != TCPPlayer.my_player.id)
        //    {
        //        //Debug.Log("UPDATING OTHER AT: (" + Int32.Parse(data[0]) + "," + float.Parse(data[1]) + "," + float.Parse(data[2]) + ")");
        //        UnityMainThreadDispatcher.Instance().Enqueue(
        //            TCPPlayer.UpdateOther(id, new Vector2(float.Parse(data[1]), float.Parse(data[2])))
        //        );
        //    }
        //}
        //else if (cmd == "disconnect")
        //{
        //    UnityMainThreadDispatcher.Instance().Enqueue(
        //        DestroyPlayer(id)
        //    );
        //}
        //else
        //{

        //}
    }


}
