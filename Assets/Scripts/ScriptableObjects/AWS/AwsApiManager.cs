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
    private const string stats_endpoint = "/stats.php";

    private enum CALLBACK{
        TRYLOGIN,
        REGISTER,
        SETDEFAULTSTATS,
        GETUSERSTATS
    }

	public void TryLogin(IDictionary<string, string> pairs)
    {
        PlayerPrefs.DeleteAll();

        StartCoroutine(POST(base_url + login_endpoint, pairs, 200, CALLBACK.TRYLOGIN));
    }
    public void Register(IDictionary<string, string> pairs)
    {
        StartCoroutine(POST(base_url + register_endpoint, pairs, 201, CALLBACK.REGISTER));
    }
    public void SetDefaultStats(int amount)
    {
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("gold_amount", amount.ToString());
        StartCoroutine(POST(base_url + stats_endpoint, pairs, 200, CALLBACK.SETDEFAULTSTATS));
    }
    public void GetUserStats()
    {
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        StartCoroutine(GET(base_url + stats_endpoint, pairs, 200, CALLBACK.GETUSERSTATS));
    }

    IEnumerator GET(string url, IDictionary<string, string> keyValuePairs, int response_code, CALLBACK callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url + GenerateGetData(keyValuePairs));
        bool needs_authenticating = !AttachAccessToken(ref uwr);
        yield return uwr.SendWebRequest();
        if (!needs_authenticating){
            Response(ref uwr, response_code, callback);
        }
    }

    IEnumerator POST(string url, IDictionary<string, string> keyValuePairs, int response_code, CALLBACK callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, GeneratePostData(keyValuePairs));
        //bool authenticating = !AttachAccessToken(ref uwr);
        AttachAccessToken(ref uwr);
        yield return uwr.SendWebRequest();
        Response(ref uwr, response_code, callback);
    }

    private bool AttachAccessToken(ref UnityWebRequest uwr)
    {
        if (!string.IsNullOrEmpty(LocalStorageManager.GetAccessToken()))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + LocalStorageManager.GetAccessToken());
            return true;
        }
        return false;
    }

    private void Response(ref UnityWebRequest www, int response_code, CALLBACK callback)
    {
        string response_json = www.downloadHandler.text;
        Debug.Log("RESPONSE: " + response_json);
        APIResponse response = JsonUtility.FromJson<APIResponse>(response_json);

        if (www.responseCode == response_code){
            Debug.Log("REQUEST SUCCEEDED");
            switch (callback)
            {
                case CALLBACK.TRYLOGIN:{
                    string access_token = response.data[0];
                    gameObject.GetComponent<AuthController>().SetError(false);
                    gameObject.GetComponent<AuthController>().AuthenticateUser(access_token);
                    break;
                }
                case CALLBACK.SETDEFAULTSTATS:{
                    GetUserStats();
                    break;
                }
                case CALLBACK.GETUSERSTATS:{
                    gameObject.GetComponent<LobbyManager>().welcome_message.text = "Welcome back, " + response.data[0] + "!";
                    gameObject.GetComponent<LobbyManager>().gold_amount_text.text = response.data[1] + " gold";
                    gameObject.GetComponent<LobbyManager>().SP_amount_text.text = response.data[2] + " SP";
                    PhotonNetwork.playerName = response.data[0];
                    break;
                }
                default: { break; }
            }
        }
        else if (www.isNetworkError){ gameObject.GetComponent<AuthController>().SetError(true, "No internet connection."); }
        else if (www.isHttpError){ gameObject.GetComponent<AuthController>().SetError(true, response.reason); }
        else{ gameObject.GetComponent<AuthController>().SetError(true, "Unknown server response."); }
    }

    [Serializable]
    public class APIResponse
    {
        public int status;
        public string status_message;
        public string reason;
        public string[] data;
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
        stringBuilder.Remove(stringBuilder.Length - 1, 1);

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