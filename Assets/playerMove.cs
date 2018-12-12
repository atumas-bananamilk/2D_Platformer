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
    public SpriteRenderer player_flag;
    public Color enemy_text_color;
    public GameObject bullet_prefab;
    [SerializeField] Text ping_text;

    //public float moveSpeed = 4f;
    public Joystick joystick;

    Vector3 prev_velocity;
    float velocity;
    public MOVEMENT_DIRECTION direction;
    public PLAYERSTATE player_state;
    private int collision_count = 0;

    private bool shooting = false;
    public LayerMask to_hit;
    Vector2 player_weapon_point;
    RaycastHit2D weapon_raycast;
    private int next_shot = 0;

    public enum PLAYERSTATE{
        IDLE, RUNNING, JUMPING
    }

    public enum MOVEMENT_DIRECTION{
        LEFT, RIGHT
    }

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
        //Screen.fullScreen = true;
    }

	private void Awake()
    {
        //if (!dev_testing && view.isMine)
        //{
        //    player_camera.SetActive(true);
        //    player_name.text = PhotonNetwork.playerName;
        //}
        //if (!dev_testing && !view.isMine)
        //{
        //    player_name.text = view.owner.NickName;
        //    player_name.color = enemy_text_color;
        //}

        CalculateWeaponPointDistance();
        FlipPlayer(MOVEMENT_DIRECTION.RIGHT);

        if (dev_testing){
            player_camera.SetActive(true);
        }
    }

    private void CalculateWeaponPointDistance(){
        GetComponent<PlayerWeaponManager>().weapon_point_distance_x = 
            Math.Abs(transform.position.x - GetComponent<PlayerWeaponManager>().weapon_point.transform.position.x);

        GetComponent<PlayerWeaponManager>().weapon_point_distance_y = 
            transform.position.y - GetComponent<PlayerWeaponManager>().weapon_point.transform.position.y;
    }

    public void SetupPlayers(){
        if (!dev_testing && TCPPlayer.IsMine(gameObject))
        {
            player_camera.SetActive(true);
            player_name.text = TCPPlayer.my_player.name;
        }
        else{
            player_name.text = TCPPlayer.GetNameByGameObj(gameObject);
            player_name.color = enemy_text_color;
        }
    }

    private void UpdateVelocity(){
        velocity = ((transform.position - prev_velocity).magnitude) / Time.deltaTime;
        prev_velocity = transform.position;
    }

    private void Update()
    {
        // shoot every 5 updates
        if (shooting && next_shot >= 5){
            Shoot();
            next_shot = 0;
        }
        next_shot++;

        ParallaxBackgrounds();
        UpdateVelocity();

        //ping_text.text = "Ping: " + PhotonNetwork.GetPing();
        ping_text.text = "Ping: " + TCPNetwork.GetPing();

        if (!dev_testing && TCPPlayer.IsMine(gameObject))
        {
            checkInput();
        }

        if (collision_count == 0){
            ChangePlayerState(PLAYERSTATE.JUMPING);
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
                FlipPlayer(MOVEMENT_DIRECTION.RIGHT);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                FlipPlayer(MOVEMENT_DIRECTION.LEFT);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = player_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                Vector2 v = new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y));

                GameObject[] items = GameObject.FindGameObjectsWithTag(TagManager.PICK_UP_ITEM);
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
                Shoot();
            }
        }
    }

    private void FlipPlayer(MOVEMENT_DIRECTION d){
        direction = d;

        if (d == MOVEMENT_DIRECTION.LEFT){
            sprite.flipX = true;
            gameObject.GetComponent<PlayerWeaponManager>().FlipWeapon(true);
            if (!dev_testing)
            {
                //view.RPC("onSpriteFlipFalse", PhotonTargets.Others);
            }
        }
        else{
            sprite.flipX = false;
            gameObject.GetComponent<PlayerWeaponManager>().FlipWeapon(false);
            if (!dev_testing)
            {
                //view.RPC("onSpriteFlipTrue", PhotonTargets.Others);
            }
        }
    }

    private void MovePlayer(){
        if (!dev_testing)
        {
            Vector3 joystick_move = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

            // controlling with joystick
            if (joystick_move != Vector3.zero)
            {
                if (joystick_move.x > 0){
                    FlipPlayer(MOVEMENT_DIRECTION.RIGHT);
                }
                else{
                    FlipPlayer(MOVEMENT_DIRECTION.LEFT);
                }
                transform.Translate(joystick_move * (float)(move_speed * 0.7) * Time.deltaTime);
            }
            // controlling with buttons
            else
            {
                var keyboard_move = new Vector3(Input.GetAxis("Horizontal"), 0);
                transform.Translate(keyboard_move * move_speed * Time.deltaTime);
            }

            if (Math.Abs(velocity) > 0)
            {
                if (collision_count > 0){
                    ChangePlayerState(PLAYERSTATE.RUNNING);
                }
                TCPNetwork.SendMovementInfo(transform.position, ref velocity);
            }
            else{
                if (collision_count > 0){
                    ChangePlayerState(PLAYERSTATE.IDLE);
                }
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
    
    public void EnableShooting(bool e){
        shooting = e;
    }

    public void Shoot()
    {
        GetComponent<PlayerWeaponManager>().TryFlipWeaponShootVector();
        GetComponent<PlayerWeaponManager>().Shoot();
        TCPNetwork.Shoot();

        //player_weapon_point = GetComponent<PlayerWeaponManager>().weapon_point.transform.position;
        //weapon_raycast = Physics2D.Raycast(player_weapon_point, weapon_target, GetComponent<PlayerWeaponManager>().range, to_hit);

        //if (weapon_raycast.collider != null){
        //    if (weapon_raycast.collider.tag == TagManager.PLAYER){
        //        GameObject damaged_obj = weapon_raycast.collider.gameObject;
        //        TCPNetwork.ApplyDamage(ref damaged_obj, GetComponent<PlayerWeaponManager>().damage);
        //    }
        //}
    }

    public void jump()
    {
        jumps_done++;
        if (jumps_done < jump_limit)
        {
            body.AddForce(Vector2.up * jump_force);
        }
    }

    private void ParallaxBackgrounds(){
        BackgroundManager.Instance.MoveBackgrounds(transform.position);
    }

    public void ChangePlayerState(PLAYERSTATE s){
        switch(s){
            case PLAYERSTATE.IDLE:{
                    player_state = PLAYERSTATE.IDLE;
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE);
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE_WEAPON);
                    break;
                }
            case PLAYERSTATE.RUNNING:{
                    player_state = PLAYERSTATE.RUNNING;
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN);
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN_WEAPON);
                    break;
                }
            case PLAYERSTATE.JUMPING:{
                    player_state = PLAYERSTATE.JUMPING;
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP);
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP_WEAPON);
                    break;
                }
        }
    }

	private void OnCollisionEnter2D(Collision2D c)
    {
        collision_count++;
        reset_jumps(ref c);

        if (c.gameObject.tag == TagManager.PLAYER)
        {
            Physics2D.IgnoreCollision(c.collider, GetComponent<Collider2D>());
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        collision_count--;
        reset_jumps(ref c);
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
		
	}

	private void reset_jumps(ref Collision2D c)
    {
        if (!dev_testing && view.isMine)
        {
            if (c.gameObject.tag == TagManager.GROUND || c.gameObject.tag == TagManager.PLAYER)
            {
                jumps_done = 0;
            }
        }
        else
        {
            if (c.gameObject.tag == TagManager.GROUND || c.gameObject.tag == TagManager.PLAYER)
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
