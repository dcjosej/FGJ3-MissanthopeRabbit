using UnityEngine;
using System.Collections;

public class DestroyHead : MonoBehaviour {

	void Update () {
		if (transform.position.y < -60f)
		{
			Destroy(this.gameObject);
		}
	}
}
