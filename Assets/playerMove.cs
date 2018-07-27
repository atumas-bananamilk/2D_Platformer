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
    public int jump_count = 2;
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

    public GameObject main_game_manager;

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
            if (Input.GetKeyDown(KeyCode.Space) && jumps_done < jump_count)
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
                view.RPC("dig", PhotonTargets.AllBuffered);
                //main_game_manager_view.RPC("DestroyBlock", PhotonTargets.AllBuffered);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                shoot();
            }
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
            if (c.gameObject.tag == "Ground")
            {
                //is_grounded = false;
                jumps_done = 0;
            }
        }
        else
        {
            if (c.gameObject.tag == "Ground")
            {
                //is_grounded = false;
                jumps_done = 0;
            }
        }
    }

    private void jump(){
        jumps_done++;
        if (jumps_done < jump_count){
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

    [PunRPC]
    private void dig()
    {
        Debug.Log("BLOCK LIST SIZE BEFORE: " + this.GetComponent<MainGameManager>().block_list.Count);
        this.GetComponent<MainGameManager>().DestroyBlock();
        Debug.Log("BLOCK LIST SIZE AFTER: " + this.GetComponent<MainGameManager>().block_list.Count);
    }

    private void smoothNetMovement(){
        transform.position = Vector3.Lerp(transform.position, self_position, Time.deltaTime * 8);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.isWriting){
            stream.SendNext(transform.position);
        }
        else{
            self_position = (Vector3)stream.ReceiveNext();
        }
    }
}
