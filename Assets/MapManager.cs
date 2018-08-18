using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapChange
{
    public string Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public MapChange(string action, int x, int y)
    {
        this.Action = action;
        this.X = x;
        this.Y = y;
    }
}

public class MapManager : MonoBehaviour
{
    public const float BLOCK_SIZE = 32;

    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;

    public TextAsset CSV_file;
    public GameObject block_prefab;
    public Sprite ground_5;
    public Sprite ground_6;
    public Sprite ground_7;
    public Sprite ground_11;

    private IDictionary<int, Sprite> block_images = new Dictionary<int, Sprite>();
    [HideInInspector] public List<GameObject> block_list = new List<GameObject>();
    private List<List<string>> map;
    [HideInInspector] public List<MapChange> map_changes = new List<MapChange>();

    public void Start()
    {
        MapBlockImages();

        if (gameObject.GetComponent<playerMove>().dev_testing){
            map = ReadMap(CSV_file);
            PlaceBlocks();
        }
        else{
            // maps - stored locally
            if (PhotonNetwork.room.Name.Equals("main_world"))
            {
                // 101 x 100
                map = ReadMap(CSV_file);
            }
            // map changes - stored remotely
            //else if (PhotonNetwork.room.Name.Equals(PhotonNetwork.playerName)){

            //}
            else
            {
                map = ReadMap(CSV_file);
                AwsApiManager.Instance.GetMapChanges(PhotonNetwork.room.Name, gameObject);
            }

            if (gameObject.GetComponent<playerMove>().view.isMine)
            {
                PlaceBlocks();
            }
        }
    }

    private void MapBlockImages()
    {
        block_images[5] = ground_5;
        block_images[6] = ground_6;
        block_images[7] = ground_7;
        block_images[11] = ground_11;
    }

    private void PlaceBlocks()
    {
        float x = block_prefab.transform.position.x - 50;
        float y = block_prefab.transform.position.y;

        foreach (List<string> row in map)
        {
            foreach (string cell in row)
            {
                if (!cell.Equals("-1"))
                {
                    int cell_id = 0;
                    Int32.TryParse(cell, out cell_id);

                    GameObject block = null;
                    block = Instantiate(block_prefab, new Vector2(x, y), Quaternion.identity) as GameObject;
                    //block.transform.SetParent(map_obj.transform, false);
                    //block.transform.localScale = new Vector2(0.08f, 1);

                    SetBlockImage(cell_id, ref block);
                }
                x++;
            }
            x = block_prefab.transform.position.x;
            y--;
        }
    }

    public void UpdateMapChanges()
    {
        foreach (MapChange change in map_changes)
        {
            UpdateMapLocally(new Vector3(change.X, change.Y, 0), false, change.Action);
        }
    }

    public void UpdateMapLocally(Vector3 unit_pos, bool updating_from_player, string action)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject block in blocks)
        {
            if (block.transform.position == unit_pos)
            {
                // local update
                if (action.Equals(playerMove.ACTION_DIG_BLOCK))
                {
                    Destroy(block);
                }
                else if (action.Equals(playerMove.ACTION_PUT_BLOCK))
                {

                }

                // remote update
                if (updating_from_player && PhotonNetwork.room.Name.Equals(PhotonNetwork.playerName))
                {
                    UpdateMapRemotely(ref action, unit_pos.x.ToString(), unit_pos.y.ToString());
                }
                break;
            }
        }
    }

    private void UpdateMapRemotely(ref string action, string x, string y)
    {
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("action", action);
        pairs.Add("X", x);
        pairs.Add("Y", y);
        AwsApiManager.Instance.UpdateMap(pairs, gameObject);
    }

    private void SetBlockImage(int cell_id, ref GameObject block)
    {
        block.GetComponent<SpriteRenderer>().sprite = block_images.ContainsKey(cell_id) ? block_images[cell_id] : ground_11;
    }

    private List<List<string>> ReadMap(TextAsset file)
    {
        List<List<string>> list = new List<List<string>>();
        string[] records = file.text.Split('\n');
        foreach (string record in records)
        {
            string[] fields = record.Split(',');
            List<string> ll = new List<string>();
            foreach (string field in fields)
            {
                ll.Add(field);
            }
            list.Add(ll);
        }
        return list;
    }

    // called by photon
    //public void OnPhotonPlayerConnected(PhotonPlayer player){
    //    GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
    //    obj.transform.SetParent(player_grid.transform, false);
    //    obj.GetComponent<Text>().text = player.NickName + " joined the server";
    //    obj.GetComponent<Text>().color = Color.green;
    //}

    //public void OnPhotonPlayerDisconnected(PhotonPlayer player){
    //    GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
    //    obj.transform.SetParent(player_grid.transform, false);
    //    obj.GetComponent<Text>().text = player.NickName + " left the server";
    //    obj.GetComponent<Text>().color = Color.red;
    //}


}