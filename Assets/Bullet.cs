using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour {
    public bool moving_direction = false;
    public float speed = 0.4f;
    public float destroy_time = 2;

    //void Awake(){
    //    StartCoroutine("destroyObj");
    //}
	
    //IEnumerator destroyObj(){
    //    yield return new WaitForSeconds(destroy_time);
    //    Destroy(gameObject);
    //}

    [PunRPC]
    public void changeDirectionLeft(){
        moving_direction = true;
    }

    void Update(){
        if (!moving_direction){
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else{
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (!photonView.isMine){
            return;
        }
        PhotonView target = other.gameObject.GetComponent<PhotonView>();

        if (target != null && (!target.isMine || target.isSceneView)){
            if (other.tag == "Player"){
                other.GetComponent<PhotonView>().RPC("reduceHealth", PhotonTargets.All);
                this.GetComponent<PhotonView>().RPC("destroyObj", PhotonTargets.All);
            }
        }
	}

    [PunRPC]
    private void destroyObj(){
        Destroy(gameObject);
    }
}
