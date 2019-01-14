using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private float speed = 20f; // 40f
    private float destroy_time = 2f;
    private Vector2 direction;
    public float damage = 0.2f;

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

	private void OnTriggerEnter2D(Collider2D c){
        if (c.GetComponent<Collider2D>().tag == TagManager.PLAYER){
            GameObject o = c.gameObject;
            if (o && c.gameObject){
                TCPNetwork.ApplyDamage(ref o, damage);
            }
            DestroyObjImmediate();
        }
        if (c.GetComponent<Collider2D>().tag == TagManager.WALL){
            GameObject o = c.gameObject;
            if (o && c.gameObject){
                c.GetComponent<WorldItem>().TakeDamage();
            }
            DestroyObjImmediate();
        }
	}

	private void DestroyObj(){
        if (gameObject){
            Destroy(gameObject, destroy_time);
        }
    }

    private void DestroyObjImmediate(){
        if (gameObject){
            Destroy(gameObject);
        }
    }

    //IEnumerator destroyObj(){
    //    yield return new WaitForSeconds(destroy_time);
    //    Destroy(gameObject);
    //}
}