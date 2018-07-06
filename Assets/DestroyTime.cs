using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour {
    [SerializeField] private float time_amount = 10f;

	private void Awake()
	{
        StartCoroutine("Destroy");
	}

    IEnumerator Destroy(){
        yield return new WaitForSeconds(time_amount);
        Destroy(this.gameObject);
    }
}
