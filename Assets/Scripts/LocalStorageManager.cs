﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalStorageManager {
    private static readonly string ACCESS_TOKEN = "access_token";
    private static readonly string CONNECTED_WORLD_NAME = "connected_world";

    private enum LOADTYPE{
        STRING, INT, FLOAT
    }

    public static void StoreAccessToken(string token)
    {
        Save(ACCESS_TOKEN, token);
    }
    public static string GetAccessToken()
    {
        return (string) Load(ACCESS_TOKEN, LOADTYPE.STRING);
    }
    public static void StoreConnectedWorldName(string world_name){
        Save(CONNECTED_WORLD_NAME, world_name);
    }
    public static string GetConnectedWorldName()
    {
        return (string)Load(CONNECTED_WORLD_NAME, LOADTYPE.STRING);
    }


    private static void Save(string k, object v){
        if (v is string){
            PlayerPrefs.SetString(k, (string) v);
        }
        else if (v is int)
        {
            PlayerPrefs.SetInt(k, (int)v);
        }
        else if (v is float)
        {
            PlayerPrefs.SetFloat(k, (float)v);
        }
    }
    private static object Load(string k, LOADTYPE type){
        switch (type)
        {
            case LOADTYPE.STRING:
                return PlayerPrefs.GetString(k);
            case LOADTYPE.INT:
                return PlayerPrefs.GetInt(k, 0);
            case LOADTYPE.FLOAT:
                return PlayerPrefs.GetFloat(k);
        }

        return null;
    }
}