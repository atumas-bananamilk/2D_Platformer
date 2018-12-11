using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private float speed = 20f;
    private float destroy_time = 2f;
    private Vector2 direction;

	void Awake(){
	    StartCoroutine("DestroyObj");
	}

	public void ChangeDirection(playerMove.MOVEMENT_DIRECTION d){
        if (d == playerMove.MOVEMENT_DIRECTION.LEFT){
            direction = Vector2.left;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else{
            direction = Vector2.right;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void Update(){
        transform.Translate(direction * speed * Time.deltaTime);
    }

	//private void OnTriggerEnter2D(Collider2D other){
 //       if (other.tag == "Player"){
            
 //       }
 //       Debug.Log("TOUCHED");
	//}

    private void OnCollisionEnter2D(Collision2D c){
        if (c.collider.tag == "Player"){

        }
        Debug.Log("TOUCHED: "+c.collider.name);
    }

    private void DestroyObj(){
        Destroy(gameObject, destroy_time);
    }

    //IEnumerator destroyObj(){
    //    yield return new WaitForSeconds(destroy_time);
    //    Destroy(gameObject);
    //}
}
