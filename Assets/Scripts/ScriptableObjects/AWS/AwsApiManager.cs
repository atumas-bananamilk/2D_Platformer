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

    public void TryLogin(IDictionary<string, string> keyValuePairs)
    {
        StartCoroutine(POST(base_url + login_endpoint, keyValuePairs, 200));
    }

    public void Register(IDictionary<string, string> keyValuePairs)
    {
        StartCoroutine(POST(base_url + register_endpoint, keyValuePairs, 201));
    }

    //public void Get(IDictionary<string, string>  keyValuePairs)
    //{
    //    //TODO: THIS IS A TEST Change values after
    //    keyValuePairs.Add("email", "test@test.com");
    //    keyValuePairs.Add("password", "testPW");
    //    GET(base_url, keyValuePairs);
    //}

    IEnumerator GET(string url, IDictionary<string, string> keyValuePairs, int response_code)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url + GenerateGetData(keyValuePairs));
        yield return uwr.SendWebRequest();
        Response(ref uwr, response_code);
    }

    IEnumerator POST(string url, IDictionary<string, string> keyValuePairs, int response_code)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, GeneratePostData(keyValuePairs));
        yield return uwr.SendWebRequest();
        Response(ref uwr, response_code);
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
        stringBuilder.Remove(stringBuilder.Length -1 , 1);

        return stringBuilder.ToString();
    }

    /*
     * Generates Data for Post Request through Dictionary values
     */
    private WWWForm GeneratePostData(IDictionary<string, string> keyValuePairs)
    {
        WWWForm postData = new WWWForm();
        foreach (KeyValuePair<string, string> entry in keyValuePairs)
        {
            postData.AddField(entry.Key, entry.Value);
        }
        return postData;
    }

    /*
     * Checks for errors in the request sent to the server, mirror errors from PHP Script in EC2
     * to do some correct Error Handling
     */
    void Response(ref UnityWebRequest www, int response_code)
    {
        APIResponse response = JsonUtility.FromJson<APIResponse>(www.downloadHandler.text);
        if (www.responseCode == response_code)
        {
            gameObject.GetComponent<AuthController>().SetError(false);
            gameObject.GetComponent<AuthController>().AuthenticateUser();
        }
        else if (www.isNetworkError || www.isHttpError)
        {
            gameObject.GetComponent<AuthController>().SetError(true, response.reason);
        }
        else{
            gameObject.GetComponent<AuthController>().SetError(true, "Unknown server response.");
        }
    }

    [Serializable]
    public class APIResponse
    {
        public int status;
        public string status_message;
        public string reason;
        public object data;
    }

}