using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMove : Photon.MonoBehaviour {
    [Header("General Booleans")]
    public bool dev_testing = false;
    public bool is_grounded = false;
    public bool disable_move = false;

    [Space]
    [Header("General Floats/Ints")]
    public float move_speed = 100;
    public float jump_force = 1000;
    public int jump_limit = 2;
    private int jumps_done = 2;

    [Space]
    public PhotonView view;
    private Vector3 self_position;
    public GameObject player_camera;
    public SpriteRenderer sprite;
    public Rigidbody2D body;
    public Text player_name;
    public Color enemy_text_color;
    public GameObject bullet_prefab;
    [SerializeField] Text ping_text;




    public TextAsset CSV_file;
    public GameObject block_prefab;
    public Sprite ground_5;
    public Sprite ground_6;
    public Sprite ground_7;
    public Sprite ground_11;



    [HideInInspector] public List<GameObject> block_list = new List<GameObject>();



    public void Start()
    {
        // 101 x 100
        List<List<string>> map = ReadMap();
        PlaceBlocks(ref map);
    }

    [PunRPC]
    public void DestroyBlock()
    {
        if (view.isMine)
        {
            Debug.Log("MINE: " + block_list.Count);
            //player_name.text = "DESTROYED";
        }
        else
        {
            Debug.Log("OTHER: " + block_list.Count);
        }

        Destroy(block_list[0]);
        block_list.RemoveAt(0);
    }

    private void PlaceBlocks(ref List<List<string>> map)
    {
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

    private void SetBlockImage(int cell_id, ref GameObject block)
    {
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


    private List<List<string>> ReadMap()
    {
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


	private void Awake()
    {
        if (!dev_testing && view.isMine)
        {
            player_camera.SetActive(true);
            player_name.text = PhotonNetwork.playerName;
        }

        if (!dev_testing && !view.isMine)
        {
            player_name.text = view.owner.NickName;
            player_name.color = enemy_text_color;
        }
    }

	private void Update()
	{
        //ping_text.text = "Ping: " + PhotonNetwork.GetPing();

        if (!dev_testing){
            // otherwise would control every player
            if (photonView.isMine)
            {
                checkInput();
            }
            else
            {
                smoothNetMovement();
            }
        }
        else{
            checkInput();
        }
    }

    private void checkInput(){
        if (!disable_move){
            var move = new Vector3(Input.GetAxis("Horizontal"), 0);
            transform.position += move * move_speed * Time.deltaTime;

            //if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
            if (Input.GetKeyDown(KeyCode.Space) && jumps_done < jump_limit)
            {
                jump();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                sprite.flipX = false;
                view.RPC("onSpriteFlipFalse", PhotonTargets.Others); // call this on other players (without myself)
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                sprite.flipX = true;
                view.RPC("onSpriteFlipTrue", PhotonTargets.Others);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //view.RPC("DestroyBlock", PhotonTargets.AllBuffered);
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(0, null, true, options);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                shoot();
            }
        }
    }

    // setup our OnEvent as callback:
    void OnEnable()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }
    void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;
    }
    // handle custom events:
    void OnEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 0)
        {
            //PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
            //byte[] selected = content as byte[];
            //for (int i = 0; i < selected.Length; i++)
            //{
            //    byte unitId = selected[i];

            //}

            Destroy(block_list[0]);
            block_list.RemoveAt(0);
        }
    }

    void shoot(){
        if (!dev_testing){
            Vector2 v = new Vector2(this.transform.position.x, this.transform.position.y);
            GameObject obj = PhotonNetwork.Instantiate(bullet_prefab.name, v, Quaternion.identity, 0);

            if (!sprite.flipX){
                // already going right by default
            }
            else{
                obj.GetComponent<PhotonView>().RPC("changeDirectionLeft", PhotonTargets.AllBuffered);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (!dev_testing && view.isMine){
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                //c.gameObject.SendMessage("ApplyDamage", 10);
                //is_grounded = true;
                jumps_done = 0;
            }
        }
        else{
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                //is_grounded = true;
                jumps_done = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (!dev_testing && view.isMine)
        {
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                //is_grounded = false;
                jumps_done = 0;
            }
        }
        else
        {
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                //is_grounded = false;
                jumps_done = 0;
            }
        }
    }

    private void jump(){
        jumps_done++;
        if (jumps_done < jump_limit){
            body.AddForce(Vector2.up * jump_force);
        }
    }

    /// <summary>
    /// NET CODE
    /// </summary>

    [PunRPC]
    private void onSpriteFlipTrue()
    {
        sprite.flipX = true;
    }

    [PunRPC]
    private void onSpriteFlipFalse()
    {
        sprite.flipX = false;
    }

    private void smoothNetMovement(){
        //transform.position = Vector3.Lerp(transform.position, self_position, Time.deltaTime * 8);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.isWriting){
            //stream.SendNext(transform.position);
        }
        else{
            //self_position = (Vector3)stream.ReceiveNext();
        }
    }
}
