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

    /*
     * Starts Coroutine for Login Request
     */
    public void Login(IDictionary<string, string> keyValuePairs)
    {
        StartCoroutine(PostRequest(base_url + login_endpoint, keyValuePairs));
    }

    /*
     * Starts Coroutine for Register Request
     */
    public void Register(IDictionary<string, string> keyValuePairs)
    {
        StartCoroutine(PostRequest(base_url + register_endpoint, keyValuePairs));
    }

    public void Get(IDictionary<string, string>  keyValuePairs)
    {
        //TODO: THIS IS A TEST Change values after
        keyValuePairs.Add("email", "test@test.com");
        keyValuePairs.Add("password", "testPW");
        GetRequest(base_url, keyValuePairs);
    }

    /*
     * Creates Get Request to send to AWS EC2 Instance as a Coroutine
     */
    IEnumerator GetRequest(string url, IDictionary<string, string> keyValuePairs)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url + GenerateGetData(keyValuePairs));
        yield return uwr.SendWebRequest();
        GetResponse(ref uwr);
    }

    /*
     * Creates Post Request to send to AWS EC2 Instance as a Coroutine
     */
    IEnumerator PostRequest(string endpoint, IDictionary<string, string> keyValuePairs)
    {
        WWWForm postData = new WWWForm();
        GeneratePostData(keyValuePairs, ref postData);

        UnityWebRequest uwr = UnityWebRequest.Post(endpoint, postData);
        yield return uwr.SendWebRequest();

        GetResponse(ref uwr);
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
    private void GeneratePostData(IDictionary<string, string> keyValuePairs, ref WWWForm postData)
    {
        foreach (KeyValuePair<string, string> entry in keyValuePairs)
        {
            postData.AddField(entry.Key, entry.Value);
        }   
    }

    /*
     * Checks for errors in the request sent to the server, mirror errors from PHP Script in EC2
     * to do some correct Error Handling
     */
    void GetResponse(ref UnityWebRequest www)
    {
        if (www.isNetworkError
            || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {   // code 200
            Debug.Log(www.downloadHandler.text);
        }
    }

}