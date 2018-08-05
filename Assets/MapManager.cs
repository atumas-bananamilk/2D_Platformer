using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
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

    public void Start()
    {
        MapBlockImages();
        // 101 x 100
        map = ReadMap();
        PlaceBlocks();
    }

    private void MapBlockImages(){
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

                    GameObject block = Instantiate(block_prefab, new Vector2(x, y), Quaternion.identity);
                    block_list.Add(block);

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

    public void TryDestroyBlock(Vector3 click_position, string action, Camera camera){

        int x = (int) Math.Round(camera.ScreenToWorldPoint(click_position).x);
        int y = (int) Math.Round(camera.ScreenToWorldPoint(click_position).y);
        Vector3 v = new Vector3(x, y, 0);

        //foreach (GameObject block in block_list)
        //{
            //if (block.transform.position == v){
                //Destroy(block);
                //block_list.Remove(block);
                Destroy(block_list[0]);
                block_list.RemoveAt(0);
                UpdateMapOnServer(ref action, x.ToString(), y.ToString());
                //break;
            //}
        //}
    }

    private void UpdateMapOnServer(ref string action, string x, string y){
        IDictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("action", action);
        pairs.Add("X", x);
        pairs.Add("Y", y);
        AwsApiManager.Instance.UpdateMap(pairs);
    }

    private void SetBlockImage(int cell_id, ref GameObject block){
        block.GetComponent<SpriteRenderer>().sprite = block_images.ContainsKey(cell_id) ? block_images[cell_id] : ground_11;
    }

    private List<List<string>> ReadMap(){
        List<List<string>> list = new List<List<string>>();
        string[] records = CSV_file.text.Split('\n');
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