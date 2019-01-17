using System;
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
    public Text amount_wood_text;
    public Text amount_brick_text;
    public Text amount_metal_text;
    private int amount_wood = 0;
    private int amount_brick = 0;
    private int amount_metal = 0;
    private int materials_limit = 999;

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

    private bool is_healing_small = false;
    private bool is_healing_big = false;
    private bool is_shielding_small = false;
    private bool is_shielding_big = false;

    private int mouse_down_count = 0;

    public enum PLAYERSTATE{
        IDLE, RUNNING, JUMPING, DEAD
    }

    public enum MOVEMENT_DIRECTION : byte{
        LEFT = 0, RIGHT = 1
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

    public void SetIsHealingSmall(bool h){
        if (!h){
            mouse_down_count = 0;
        }
        is_healing_small = h;
    }
    public void SetIsHealingBig(bool h){
        if (!h){
            mouse_down_count = 0;
        }
        is_healing_big = h;
    }
    public void SetIsShieldingSmall(bool h){
        if (!h){
            mouse_down_count = 0;
        }
        is_shielding_small = h;
    }
    public void SetIsShieldingBig(bool h){
        if (!h){
            mouse_down_count = 0;
        }
        is_shielding_big = h;
    }

    private void Update()
    {
        if (is_healing_small){
            mouse_down_count++;
            KeepLoading(LoadingManager.LOADING_TYPE.HEALTH_SMALL, transform.position, new Vector2(0, 4f));
        }
        if (is_healing_big){
            mouse_down_count++;
            KeepLoading(LoadingManager.LOADING_TYPE.HEALTH_BIG, transform.position, new Vector2(0, 4f));
        }
        if (is_shielding_small){
            mouse_down_count++;
            KeepLoading(LoadingManager.LOADING_TYPE.SHIELD_SMALL, transform.position, new Vector2(0, 4f));
        }
        if (is_shielding_big){
            mouse_down_count++;
            KeepLoading(LoadingManager.LOADING_TYPE.SHIELD_BIG, transform.position, new Vector2(0, 4f));
        }

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
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (!disable_move)
        {
            MovePlayer();

            if (Input.GetKeyDown(KeyCode.Space) && jumps_done < jump_limit)
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                FlipPlayerOnNetwork(MOVEMENT_DIRECTION.RIGHT);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                FlipPlayerOnNetwork(MOVEMENT_DIRECTION.LEFT);
            }

            CheckMouseClicks();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Shoot();
            }
        }
    }
    

    public float GetAngle(Vector2 p1, Vector2 p2){
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;

        if (angle < 0){
            angle += 360;
        }

        return angle;
    }

    private void CheckMouseClicks(){
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = player_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            // clicked on item
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

            //Vector2 v = new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y));
            //if (!dev_testing){
            //    RaiseEventOptions options = new RaiseEventOptions();
            //    options.Receivers = ReceiverGroup.All;
            //    PhotonNetwork.RaiseEvent((byte)PHOTON_EVENTS.ACTION_DIG_BLOCK, v, true, options);
            //}
        }
        else if (Input.GetMouseButtonUp(0)){
            KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject> dig_block = CheckClick(TagManager.GROUND);
            KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject> chest_block = CheckClick(TagManager.CHEST);

            if (dig_block.Value != null){
                dig_block.Value.GetComponent<Block>().StopDig();
                mouse_down_count = 0;
            }
            if (chest_block.Value != null){
                chest_block.Value.GetComponent<Block>().StopOpeningChest();
                mouse_down_count = 0;
            }

            GetComponent<PlayerDigManager>().StopDig();
            StopLoading();
            //mouse_down_count = 0;
        }
        else if (Input.GetMouseButton(0)){
            KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject> dig_block = CheckClick(TagManager.GROUND);
            KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject> chest_block = CheckClick(TagManager.CHEST);

            if (dig_block.Value != null){
                PlayerDigManager.DIG_DIRECTION dig_direction = dig_block.Key;
                GameObject b = dig_block.Value;

                mouse_down_count++;
                GetComponent<PlayerDigManager>().Dig(dig_direction);
                b.GetComponent<Block>().Dig();

                if (mouse_down_count % 10 == 0){
                    b.GetComponent<Block>().health -= 10;
                }
                if (b.GetComponent<Block>().health <= 0){
                    UpdateMaterials(b.GetComponent<Block>().block_type, 15);
                    GetComponent<ItemDropManager>().DropRandomizedItems(b.GetComponent<Block>().transform.position);
                }
            }

            if (chest_block.Value != null){
                GameObject b = chest_block.Value;

                if (b.GetComponent<Block>().unopened){
                    mouse_down_count++;
                    KeepLoading(LoadingManager.LOADING_TYPE.CHEST, b.GetComponent<Block>().transform.position, new Vector2(0, -1.5f));
                    b.GetComponent<Block>().KeepOpeningChest();

                    if (mouse_down_count >= GetComponent<LoadingManager>().GetMouseDownLimit(LoadingManager.LOADING_TYPE.CHEST)){
                        b.GetComponent<Block>().OpenChest();
                        GetComponent<ItemDropManager>().DropRandomizedChestItems(b.GetComponent<Block>().transform.position);
                    }
                }
            }
        }
    }

    public void KeepLoading(LoadingManager.LOADING_TYPE type, Vector2 pos, Vector2 offset){
        GetComponent<LoadingManager>().SetupLoadingPanel(type, pos + offset);
        GetComponent<LoadingManager>().UpdateLoadingCircle(type, mouse_down_count);
        GetComponent<LoadingManager>().SetLoadingPanelVisible(true);
    }

    private void StopLoading(){
        GetComponent<LoadingManager>().SetLoadingPanelVisible(false);
    }

    // clicked down / up on block
    private KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject> CheckClick(string objects_tag){
        Vector2 pos = player_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        GameObject[] ground_blocks = GameObject.FindGameObjectsWithTag(objects_tag);
        foreach (GameObject b in ground_blocks){
            Vector2 player_center = GetComponent<BoxCollider2D>().bounds.center;
            float size = GetComponent<BoxCollider2D>().size.y / 2;
            float player_y_bottom = player_center.y - size;
            float player_y_top = player_center.y + size;

            if (b.GetComponent<BoxCollider2D>().bounds.Contains(pos)){
                float distance = Vector2.Distance(player_center, b.GetComponent<BoxCollider2D>().bounds.center);

                if (distance < 2){
                    PlayerDigManager.DIG_DIRECTION dig_direction;

                    // check if click is on top, side, bottom from player
                    if (pos.y < player_y_bottom){
                        dig_direction = PlayerDigManager.DIG_DIRECTION.BOTTOM;
                    }
                    else if (pos.y > player_y_top){
                        dig_direction = PlayerDigManager.DIG_DIRECTION.TOP;
                    }
                    else{
                        dig_direction = PlayerDigManager.DIG_DIRECTION.SIDE;
                    }
                    return new KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject>(dig_direction, b);
                }
                break;
            }
        }
        return new KeyValuePair<PlayerDigManager.DIG_DIRECTION, GameObject>();
    }

    private void UpdateMaterials(Block.BLOCK_TYPE block_type, int amount){
        switch(block_type){
            case Block.BLOCK_TYPE.WOOD:{
                    amount_wood += amount;
                    if (amount_wood >= materials_limit){
                        amount_wood = materials_limit;
                    }
                    amount_wood_text.text = amount_wood.ToString();
                    break;
                }
            case Block.BLOCK_TYPE.BRICK:{
                    amount_brick += amount;
                    if (amount_brick >= materials_limit){
                        amount_brick = materials_limit;
                    }
                    amount_brick_text.text = amount_brick.ToString();
                    break;
                }
            case Block.BLOCK_TYPE.METAL:{
                    amount_metal += amount;
                    if (amount_metal >= materials_limit){
                        amount_metal = materials_limit;
                    }
                    amount_metal_text.text = amount_metal.ToString();
                    break;
                }
        }
    }

    public void FlipPlayerOnNetwork(MOVEMENT_DIRECTION d){
        FlipPlayer(d);
        TCPNetwork.FlipPlayer(d);
    }

    public void FlipPlayer(MOVEMENT_DIRECTION d){
        direction = d;

        if (d == MOVEMENT_DIRECTION.LEFT){
            sprite.flipX = true;
            gameObject.GetComponent<PlayerWeaponManager>().FlipWeapon(true);
            gameObject.GetComponent<PlayerDigManager>().FlipPickaxe(true);
        }
        else{
            sprite.flipX = false;
            gameObject.GetComponent<PlayerWeaponManager>().FlipWeapon(false);
            gameObject.GetComponent<PlayerDigManager>().FlipPickaxe(false);
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
                    FlipPlayerOnNetwork(MOVEMENT_DIRECTION.RIGHT);
                }
                else{
                    FlipPlayerOnNetwork(MOVEMENT_DIRECTION.LEFT);
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
                    TryChangePlayerState(PLAYERSTATE.RUNNING);
                }
                else{
                    TryChangePlayerState(PLAYERSTATE.JUMPING);
                }
                TCPNetwork.SendMovementInfo(transform.position, ref velocity);
            }
            else{
                if (collision_count > 0){
                    TryChangePlayerState(PLAYERSTATE.IDLE);
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
        GetComponent<PlayerWeaponManager>().Shoot();
        TCPNetwork.Shoot();
    }

    public void Jump()
    {
        jumps_done++;
        if (jumps_done < jump_limit)
        {
            body.AddForce(Vector2.up * jump_force);
        }
    }

    private void ParallaxBackgrounds(){
        //BackgroundManager.Instance.MoveBackgrounds(transform.position);
        GetComponent<BackgroundManager>().MoveBackgrounds(transform.position);
        //BackgroundManager.Instance.MoveBackgrounds(transform.position);
    }

    public void TryChangePlayerState(PLAYERSTATE s){
        if (player_state != s && player_state != PLAYERSTATE.DEAD){
            ChangePlayerState(s);
            TCPNetwork.ChangePlayerState(s);
        }
    }

    public void ChangePlayerState(PLAYERSTATE s){
        GameObject o = gameObject;
        switch (TCPPlayer.GetIdByGameObject(ref o)){
            case 0:{
                switch(s){
                    case PLAYERSTATE.IDLE:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE_WEAPON);
                            break;
                        }
                    case PLAYERSTATE.RUNNING:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN_WEAPON);
                            break;
                        }
                    case PLAYERSTATE.JUMPING:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP_WEAPON);
                            break;
                        }
                    case PLAYERSTATE.DEAD:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_DIE);
                            break;
                        }
                }
                break;
            }
            case 1:{
                switch(s){
                    case PLAYERSTATE.IDLE:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE_WIZARD);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_IDLE_WEAPON);
                            break;
                        }
                    case PLAYERSTATE.RUNNING:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN_WIZARD);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_RUN_WEAPON);
                            break;
                        }
                    case PLAYERSTATE.JUMPING:{
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP_WIZARD);
                            gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_JUMP_WEAPON);
                            break;
                        }
                }
                break;
            }
        }

        player_state = s;
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
            if (c.gameObject.tag == TagManager.GROUND || c.gameObject.tag == TagManager.WALL || c.gameObject.tag == TagManager.PLAYER)
            {
                jumps_done = 0;
            }
        }
        else
        {
            if (c.gameObject.tag == TagManager.GROUND || c.gameObject.tag == TagManager.WALL || c.gameObject.tag == TagManager.PLAYER)
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
