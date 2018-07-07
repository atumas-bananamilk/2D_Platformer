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
    public float jump_force = 800;

    [Space]
    public PhotonView view;
    private Vector3 self_position;
    public GameObject player_camera;
    public SpriteRenderer sprite;
    public Rigidbody2D body;
    public Text player_name;
    public Color enemy_text_color;
    public GameObject bullet_prefab;

    private bool stupid_stuff_to_delete = false;

    private void Awake()
    {
        if (!dev_testing && view.isMine)
        {
            player_camera.SetActive(true);
            player_name.text = PhotonNetwork.playerName;
        }
        
        //little comment just to test
        // RIDICULOUS 

        if (stupid_stuff_to_delete)
        {
            // never going here so
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

            if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
            {
                //Debug.Log("PING: " + Network.GetAveragePing(Network.player));
                Debug.Log("IP: " + MasterServer.ipAddress);

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
            if (c.gameObject.tag == "Ground")
            {
                //c.gameObject.SendMessage("ApplyDamage", 10);
                is_grounded = true;
            }
        }
        else{
            if (c.gameObject.tag == "Ground")
            {
                is_grounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (!dev_testing && view.isMine)
        {
            if (c.gameObject.tag == "Ground")
            {
                is_grounded = false;
            }
        }
        else
        {
            if (c.gameObject.tag == "Ground")
            {
                is_grounded = false;
            }
        }
    }

	private void jump(){
        body.AddForce(Vector2.up * jump_force);
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
