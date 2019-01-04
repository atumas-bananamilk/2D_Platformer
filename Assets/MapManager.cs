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
    public static Sprite[] tile_sprites;

    public static IDictionary<int, Sprite> block_images = new Dictionary<int, Sprite>();
    [HideInInspector] public List<GameObject> block_list = new List<GameObject>();
    private List<List<string>> map;
    [HideInInspector] public List<MapChange> map_changes = new List<MapChange>();

    public string tiles_path = "AssetsTiles/tiles";
    public static int TILE_ID_EMPTY = -1;
    public static int TILE_ID_STORM = 10;
    public static List<int> TILES_WOOD = new List<int>();
    public static List<int> TILES_BRICK = new List<int>();
    public static List<int> TILES_METAL = new List<int>();
    
    int offset = 2;
    public int map_width;
    public int map_height;
    public float start_x;
    public float start_y;

	private void Awake(){
        tile_sprites = Resources.LoadAll<Sprite>(tiles_path);
        SetupTilesWood();
        SetupTilesBrick();
        SetupTilesMetal();
	}

    private void SetupTilesWood(){
        TILES_WOOD.Add(0);
        TILES_WOOD.Add(1);
        TILES_WOOD.Add(2);
        TILES_WOOD.Add(3);
        TILES_WOOD.Add(7);
        TILES_WOOD.Add(8);
        TILES_WOOD.Add(9);
        TILES_WOOD.Add(14);
        TILES_WOOD.Add(15);
        TILES_WOOD.Add(16);
        TILES_WOOD.Add(21);
        TILES_WOOD.Add(22);
        TILES_WOOD.Add(23);
        TILES_WOOD.Add(24);
        TILES_WOOD.Add(25);
        TILES_WOOD.Add(28);
        TILES_WOOD.Add(29);
        TILES_WOOD.Add(30);
        TILES_WOOD.Add(31);
        TILES_WOOD.Add(32);
    }

    private void SetupTilesBrick(){
        TILES_BRICK.Add(5);
        TILES_BRICK.Add(12);
        TILES_BRICK.Add(19);
        TILES_BRICK.Add(26);
        TILES_BRICK.Add(33);
    }

    private void SetupTilesMetal(){
        TILES_METAL.Add(6);
        TILES_METAL.Add(13);
        TILES_METAL.Add(20);
        TILES_METAL.Add(27);
        TILES_METAL.Add(34);
    }

	public void Start()
    {
        //MapBlockImages();
        if (gameObject.GetComponent<playerMove>().dev_testing){
            map = ReadMap(CSV_file);
            PlaceBlocks();
        }
        else{
            // maps - stored locally
            //if (TCPPlayer.my_player.room.Equals("main_world"))
            //{
                // 101 x 100
                map = ReadMap(CSV_file);
            //}
            //else
            //{
            //    map = ReadMap(CSV_file);
            //    AwsApiManager.Instance.GetMapChanges(PhotonNetwork.room.Name, gameObject);
            //}
            
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

    private void PlaceSingleBlock(float x, float y, int cell_id){
        GameObject block = null;
        block = Instantiate(block_prefab, new Vector2(x, y), Quaternion.identity) as GameObject;
        block.GetComponent<Block>().SetBlockType(cell_id);
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
                if (!cell.Equals(TILE_ID_EMPTY.ToString()))
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