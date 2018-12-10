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

public class MapInfo{
    public int width;
    public int height;

    public MapInfo(int width, int height){
        this.width = width;
        this.height = height;
    }
}

public class MapManager : MonoBehaviour
{
    //public const float BLOCK_SIZE = 32;
    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;

    public TextAsset CSV_file;
    public GameObject block_prefab;
    public Sprite TILE_ground_5;
    public Sprite TILE_ground_6;
    public Sprite TILE_ground_7;
    public Sprite TILE_ground_11;
    public Sprite TILE_storm;

    private int TILE_ID_STORM = 0;
    private int TILE_ID_GROUND_5 = 5;
    private int TILE_ID_GROUND_6 = 6;
    private int TILE_ID_GROUND_7 = 7;
    private int TILE_ID_GROUND_11 = 11;

    private IDictionary<int, Sprite> block_images = new Dictionary<int, Sprite>();
    [HideInInspector] public List<GameObject> block_list = new List<GameObject>();
    private List<List<string>> map;
    [HideInInspector] public List<MapChange> map_changes = new List<MapChange>();
    
    int offset = 2;
    public int map_width;
    public int map_height;
    public float start_x;
    public float start_y;

    public void Start()
    {
        MapBlockImages();

        if (gameObject.GetComponent<playerMove>().dev_testing){
            map = ReadMap(CSV_file);
            PlaceBlocks();
        }
        else{
            // maps - stored locally
            if (TCPPlayer.my_player.room.Equals("main_world"))
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
            
            map_width = map.Count;
            map_height = (map.Count > 0) ? map[0].Count : 0;
            map_width += offset;
            map_height += offset;

            if (TCPPlayer.IsMine(gameObject))
            {
                PlaceBlocks();
            }
        }
    }

    private void MapBlockImages()
    {
        block_images[TILE_ID_STORM] = TILE_storm;
        block_images[TILE_ID_GROUND_5] = TILE_ground_5;
        block_images[TILE_ID_GROUND_6] = TILE_ground_6;
        block_images[TILE_ID_GROUND_7] = TILE_ground_7;
        block_images[TILE_ID_GROUND_11] = TILE_ground_11;
    }

    private void PlaceSingleBlock(float x, float y, int cell_id){
        GameObject block = null;
        block = Instantiate(block_prefab, new Vector2(x, y), Quaternion.identity) as GameObject;
        SetBlockImage(cell_id, ref block);
    }

    private void PlaceBlocks()
    {
        start_x = block_prefab.transform.position.x - 1;
        start_y = block_prefab.transform.position.y;
        float x = start_x;
        float y = start_y;

        DrawBoundaries(x, y);

        // draw blocks
        foreach (List<string> row in map)
        {
            foreach (string cell in row)
            {
                if (!cell.Equals("-1"))
                {
                    int cell_id = 0;
                    Int32.TryParse(cell, out cell_id);
                    PlaceSingleBlock(x, y, cell_id);
                }
                x++;
            }
            x = block_prefab.transform.position.x;
            y--;
        }
    }

    private void DrawBoundaries(float x, float y){
        x--;
        y++;

        for (int i = 0; i < map_width; i++){
            PlaceSingleBlock(x, y, TILE_ID_STORM);
            x++;
        }
        for (int i = 0; i < map_height; i++){
            PlaceSingleBlock(x, y, TILE_ID_STORM);
            y--;
        }
        for (int i = 0; i < map_width; i++){
            PlaceSingleBlock(x, y, TILE_ID_STORM);
            x--;
        }
        for (int i = 0; i < map_height; i++){
            PlaceSingleBlock(x, y, TILE_ID_STORM);
            y++;
        }
    }

    public void UpdateMapChanges()
    {
        foreach (MapChange change in map_changes)
        {
            UpdateMapLocally(new Vector3(change.X, change.Y, 0), false, change.Action);
        }
    }

    public void UpdateMapLocally(Vector2 unit_pos, bool updating_from_player, string action)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(TagManager.GROUND);
        foreach (GameObject block in blocks)
        {
            if ((Vector2) block.transform.position == unit_pos)
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
        block.GetComponent<SpriteRenderer>().sprite = TILE_ground_5;

        if (block_images.ContainsKey(cell_id)){
            block.GetComponent<SpriteRenderer>().sprite = block_images[cell_id];
        }
        //block.GetComponent<SpriteRenderer>().sprite = block_images.ContainsKey(cell_id) ? block_images[cell_id] : ground_11;
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