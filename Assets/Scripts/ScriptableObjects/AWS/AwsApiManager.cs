using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

//public class AwsApiManager : Singleton<AwsApiManager>
public class AwsApiManager : MonoBehaviour
{
    public static AwsApiManager Instance;

    //private const string base_url = "http://hubrill.com";
    private const string base_url = "http://35.180.106.179";
    private const string login_endpoint = "/login.php";
    private const string register_endpoint = "/register.php";
    private const string stats_endpoint = "/stats.php";
    //private const string map_endpoint = "/map.php";
    private const string mapchanges_endpoint = "/mapchanges.php";
    private const string world_endpoint = "/world.php";

    private enum CALLBACK{
        TRYLOGIN,
        REGISTER,
        //SETDEFAULTSTATS,
        USER_STATS_UPDATE,
        USER_STATS_GET,
        MAP_CHANGES_UPDATE,
        MAP_CHANGES_GET,
        CHECK_WORLD_EXISTS,
        TRY_CREATE_WORLD
    }

    private GameObject reference_obj = null;

    private void Awake()
    {
        Instance = this;
    }

	public void TryLogin(IDictionary<string, string> pairs)
    {
        //PlayerPrefs.DeleteAll();
        StartCoroutine(POST(base_url + login_endpoint, pairs, 200, CALLBACK.TRYLOGIN));
    }
    public void Register(IDictionary<string, string> pairs)
    {
        StartCoroutine(POST(base_url + register_endpoint, pairs, 201, CALLBACK.REGISTER));
    }

    // WILL BE CALLED WHEN REGISTERING
    //public void SetDefaultUserStats(int amount)
    //{
    //    IDictionary<string, string> pairs = new Dictionary<string, string>();
    //    pairs.Add("gold_amount", amount.ToString());
    //    StartCoroutine(POST(base_url + stats_endpoint, pairs, 200, CALLBACK.SETDEFAULTSTATS));
    //}
    public void UpdateUserStats(int amount)
    {
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("gold_amount", amount.ToString());
        StartCoroutine(POST(base_url + stats_endpoint, pairs, 200, CALLBACK.USER_STATS_UPDATE));
    }
    public void GetUserStats()
    {
        StartCoroutine(GET(base_url + stats_endpoint, null, 200, CALLBACK.USER_STATS_GET));
    }
    public void UpdateMap(IDictionary<string, string> pairs, GameObject obj)
    {
        reference_obj = obj;
        StartCoroutine(POST(base_url + mapchanges_endpoint, pairs, 200, CALLBACK.MAP_CHANGES_UPDATE));
    }
    public void GetMapChanges(string world_name, GameObject obj)
    {
        reference_obj = obj;
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("world_name", world_name);
        StartCoroutine(GET(base_url + mapchanges_endpoint, pairs, 200, CALLBACK.MAP_CHANGES_GET));
    }
    public void CheckWorldExists(Action<object> action)
    {
        StartCoroutine(GET(base_url + world_endpoint, null, 200, CALLBACK.CHECK_WORLD_EXISTS, action));
    }
    public void TryCreateWorld(Action<object> action)
    {
        StartCoroutine(POST(base_url + world_endpoint, null, 200, CALLBACK.TRY_CREATE_WORLD, action));
    }


    private void Response(ref UnityWebRequest www, int response_code, CALLBACK callback, Action<object> action = null)
    {
        string response_json = www.downloadHandler.text;
        Debug.Log("RESPONSE: " + response_json);
        APIResponse response = JsonUtility.FromJson<APIResponse>(response_json);

        if (www.responseCode == response_code){
            switch (callback)
            {
                case CALLBACK.TRYLOGIN:{
                    string access_token = response.data[0];
                    PhotonNetwork.playerName = response.data[1];
                    AuthController.Instance.SetError(false);
                    AuthController.Instance.AuthenticateUser(access_token);
                    break;
                }
                //case CALLBACK.SETDEFAULTSTATS:{
                //    GetUserStats();
                //    break;
                //}
                case CALLBACK.USER_STATS_UPDATE:{
                    GetUserStats();
                    break;
                }
                case CALLBACK.USER_STATS_GET:{
                    TCPPlayer.my_player.name = response.data[0];
                    LobbyManager.Instance.welcome_message.text = response.data[0];
                    LobbyManager.Instance.gold_amount_text.text = response.data[1];
                    LobbyManager.Instance.SP_amount_text.text = response.data[2];
                    break;
                }
                case CALLBACK.MAP_CHANGES_UPDATE:{
                    //Debug.Log("MAP UPDATED");
                    break;
                }
                case CALLBACK.MAP_CHANGES_GET:{
                    foreach (string change in response.data){
                        string[] c_str = change.Split(':');
                        reference_obj.GetComponent<MapManager>().map_changes.Add(new MapChange(c_str[0], Int32.Parse(c_str[1]), Int32.Parse(c_str[2])));
                    }
                    reference_obj.GetComponent<MapManager>().UpdateMapChanges();
                    break;
                }
                case CALLBACK.CHECK_WORLD_EXISTS:{
                    action(response.data[0]);
                    break;
                }
                case CALLBACK.TRY_CREATE_WORLD:{
                    GetUserStats();
                    action(response.data[0]);
                    break;
                }
                default: { break; }
            }
        }
        else if (www.isNetworkError){ AuthController.Instance.SetError(true, "No internet connection or server down."); }
        else if (www.isHttpError){ Debug.Log("REASON: "+response.reason); AuthController.Instance.SetError(true, response.reason); }
        else{ AuthController.Instance.SetError(true, "Unknown server response."); }
    }

    [Serializable]
    public class APIResponse
    {
        public int status;
        public string status_message;
        public string reason;
        public string[] data;
    }

    IEnumerator GET(string url, IDictionary<string, string> keyValuePairs, int response_code, CALLBACK callback, Action<object> action = null)
    {
        keyValuePairs = (keyValuePairs == null) ? new Dictionary<string, string>() : keyValuePairs;
        UnityWebRequest uwr = UnityWebRequest.Get(url + GenerateGetData(keyValuePairs));
        bool needs_authenticating = !AttachAccessToken(ref uwr);
        yield return uwr.SendWebRequest();
        if (!needs_authenticating)
        {
            Response(ref uwr, response_code, callback, action);
        }
    }

    IEnumerator POST(string url, IDictionary<string, string> keyValuePairs, int response_code, CALLBACK callback, Action<object> action = null)
    {
        keyValuePairs = (keyValuePairs == null) ? new Dictionary<string, string>() : keyValuePairs;
        UnityWebRequest uwr = UnityWebRequest.Post(url, GeneratePostData(keyValuePairs));
        //bool authenticating = !AttachAccessToken(ref uwr);
        AttachAccessToken(ref uwr);
        yield return uwr.SendWebRequest();
        Response(ref uwr, response_code, callback, action);
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

    private string GenerateGetData(IDictionary<string, string> fields)
    {
        StringBuilder s = new StringBuilder().Append("?");
        foreach (KeyValuePair<string, string> entry in fields)
        {
            s.Append(entry.Key + "=" + entry.Value + "&");
        }
        s.Remove(s.Length - 1, 1);

        return s.ToString();
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