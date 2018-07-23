using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AwsApiManager : MonoBehaviour
{
    private const string base_url = "http://35.180.41.35";
    private const string login_endpoint = "/login.php";
    private const string register_endpoint = "/register.php";

    }

    {


    {
        UnityWebRequest uwr = UnityWebRequest.Get(url + GenerateGetData(keyValuePairs));
        yield return uwr.SendWebRequest();
    }

    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, GeneratePostData(keyValuePairs));
        yield return uwr.SendWebRequest();
    }

    /*
     * TODO: Check if this works with stringbuilder, try to implement the one with HTTPUtils Extension method
     */
    private string GenerateGetData(IDictionary<string, string> fields)
    {
        StringBuilder stringBuilder = new StringBuilder().Append("?");
        foreach (KeyValuePair<string, string> entry in fields)
        {
            stringBuilder.Append(entry.Key + "=" + entry.Value + "&");
        }

        return stringBuilder.ToString();
    }

    private WWWForm GeneratePostData(IDictionary<string, string> keyValuePairs)
    {
        WWWForm postData = new WWWForm();
        foreach (KeyValuePair<string, string> entry in keyValuePairs)
        {
            postData.AddField(entry.Key, entry.Value);
        }
        return postData;
    }
}