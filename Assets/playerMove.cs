﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class playerMove : Photon.MonoBehaviour
{
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


    //public float moveSpeed = 4f;
    public Joystick joystick;

    int aa = 0;
    long milliseconds = 0;

    public enum PHOTON_EVENTS : byte
    {
        NO_ACTION = 0,
        ACTION_DIG_BLOCK = 1,
        ACTION_PUT_BLOCK = 2,
        SEND_MSG = 3
    }
    public static readonly string NO_ACTION = "NO_ACTION";
    public static readonly string ACTION_DIG_BLOCK = "ACTION_DIG_BLOCK";
    public static readonly string ACTION_PUT_BLOCK = "ACTION_PUT_BLOCK";
    public static readonly string SEND_MSG = "SEND_MSG";

	private void Start()
	{
        Screen.fullScreen = true;
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

        if (dev_testing){
            player_camera.SetActive(true);
        }
    }

    private void Update()
    {
        //ping_text.text = "Ping: " + PhotonNetwork.GetPing();
        ping_text.text = "Ping: " + TCPNetwork.GetPing();

        if (!dev_testing)
        {
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
        else
        {
            if (aa == 10){
                //Thread t1 = new Thread(new ThreadStart(TCPNetwork.SendAndReceiveResponse));
                //t1.Start();
                aa = 0;
            }
            aa++;
            checkInput();
        }
    }

    private void checkInput()
    {
        if (!disable_move)
        {
            MovePlayer();

            if (Input.GetKeyDown(KeyCode.Space) && jumps_done < jump_limit)
            {
                jump();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                FlipPlayerRight();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                FlipPlayerLeft();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = player_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                Vector2 v = new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y));

                GameObject[] items = GameObject.FindGameObjectsWithTag("PickUpItem");
                foreach (GameObject item in items)
                {
                    if (item.GetComponent<BoxCollider2D>().bounds.Contains(pos))
                    {
                        gameObject.GetComponent<PlayerInventory>().AddToInventory(item);
                        Destroy(item);
                        break;
                    }
                }

                if (!dev_testing){
                    RaiseEventOptions options = new RaiseEventOptions();
                    options.Receivers = ReceiverGroup.All;
                    PhotonNetwork.RaiseEvent((byte)PHOTON_EVENTS.ACTION_DIG_BLOCK, v, true, options);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                shoot();
            }
        }
    }

    private void FlipPlayerRight()
    {
        sprite.flipX = false;
        if (!dev_testing)
        {
            view.RPC("onSpriteFlipFalse", PhotonTargets.Others);
        }
    }

    private void FlipPlayerLeft()
    {
        sprite.flipX = true;
        if (!dev_testing)
        {
            view.RPC("onSpriteFlipTrue", PhotonTargets.Others);
        }
    }

    private void MovePlayer(){
        if (photonView.isMine || dev_testing)
        {
            Vector3 joystick_move = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

            // controlling with joystick
            if (joystick_move != Vector3.zero)
            {
                if (joystick_move.x > 0){
                    FlipPlayerRight();
                }
                else{
                    FlipPlayerLeft();
                }
                transform.Translate(joystick_move * (float)(move_speed * 0.7) * Time.deltaTime);
            }
            // controlling with buttons
            else
            {
                var keyboard_move = new Vector3(Input.GetAxis("Horizontal"), 0);
                transform.Translate(keyboard_move * move_speed * Time.deltaTime);
            }
        }
    }

    void OnEnable()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }
    void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;
    }
    void OnEvent(byte evt_code, object content, int senderid)
    {
        switch ((PHOTON_EVENTS)evt_code)
        {
            case PHOTON_EVENTS.ACTION_DIG_BLOCK:
                {
                    Vector2 v = (Vector2)content;
                    gameObject.GetComponent<MapManager>().UpdateMapLocally(v, true, ACTION_DIG_BLOCK);
                    break;
                }
            case PHOTON_EVENTS.ACTION_PUT_BLOCK:
                {

                    break;
                }
            case PHOTON_EVENTS.SEND_MSG:
                {
                    object[] list = (object[]) content;
                    if (list.Length >= 2){
                        int photon_view_id = (int) list[0];
                        string msg = (string) list[1];
                        PhotonView.Find(photon_view_id).gameObject.GetComponent<ChatManager>().ShowMessage(msg);
                    }
                    break;
                }
            default: { break; }
        }
    }

    public void shoot()
    {
        if (!dev_testing)
        {
            Vector2 v = new Vector2(this.transform.position.x, this.transform.position.y);
            GameObject obj = PhotonNetwork.Instantiate(bullet_prefab.name, v, Quaternion.identity, 0);

            if (!sprite.flipX)
            {
                // already going right by default
            }
            else
            {
                obj.GetComponent<PhotonView>().RPC("changeDirectionLeft", PhotonTargets.AllBuffered);
            }
        }
    }

    public void jump()
    {
        jumps_done++;
        if (jumps_done < jump_limit)
        {
            body.AddForce(Vector2.up * jump_force);
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        reset_jumps(ref c);

        if (c.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(c.collider, GetComponent<Collider2D>());
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        reset_jumps(ref c);
    }

    private void reset_jumps(ref Collision2D c)
    {
        if (!dev_testing && view.isMine)
        {
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                jumps_done = 0;
            }
        }
        else
        {
            if (c.gameObject.tag == "Ground" || c.gameObject.tag == "Player")
            {
                jumps_done = 0;
            }
        }
    }

    /// <summary>
    /// NET CODE
    /// </summary>

    //[PunRPC]
    //public void DestroyBlock()
    //{
    //    gameObject.GetComponent<MainGameManager>().DestroyBlock();
    //}

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

    private void smoothNetMovement()
    {
        //transform.position = Vector3.Lerp(transform.position, self_position, Time.deltaTime * 8);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(transform.position);
        }
        else
        {
            //self_position = (Vector3)stream.ReceiveNext();
        }
    }
}
