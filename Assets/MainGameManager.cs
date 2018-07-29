using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour {
    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;

    public TextAsset CSV_file;
    public GameObject block_prefab;
    public Sprite ground_5;
    public Sprite ground_6;
    public Sprite ground_7;
    public Sprite ground_11;

    //[HideInInspector] public List<GameObject> block_list = new List<GameObject>();

    public void Start()
    {
        // 101 x 100
        List<List<string>> map = ReadMap();
        PlaceBlocks(ref map);
    }

    private void PlaceBlocks(ref List<List<string>> map){
        float x = block_prefab.transform.position.x - map[0].Count / 2;
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
                    //block_list.Add(block);
                    gameObject.GetComponent<playerMove>().block_list.Add(block);

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

    public void DestroyBlock(){
        //Destroy(block_list[0]);
        //block_list.RemoveAt(0);
    }

    private void SetBlockImage(int cell_id, ref GameObject block){
        switch (cell_id)
        {
            case 5:
                {
                    block.GetComponent<SpriteRenderer>().sprite = ground_5;
                    break;
                }
            case 6:
                {
                    block.GetComponent<SpriteRenderer>().sprite = ground_6;
                    break;
                }
            case 7:
                {
                    block.GetComponent<SpriteRenderer>().sprite = ground_7;
                    break;
                }
            case 11:
                {
                    block.GetComponent<SpriteRenderer>().sprite = ground_11;
                    break;
                }
            default:
                {
                    block.GetComponent<SpriteRenderer>().sprite = ground_11;
                    break;
                }
        }
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