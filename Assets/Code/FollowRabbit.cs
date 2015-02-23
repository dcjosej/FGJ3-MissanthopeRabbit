using UnityEngine;
using System.Collections;

public class FollowRabbit : MonoBehaviour {


	public Transform Rabbit;
	public Vector2 Margin;
	public Vector2 Smoothing;

	


	void Start () {
		
	}
	
	void Update () {
		var x = transform.position.x;
		var y = transform.position.y;

		if (Mathf.Abs(x - Rabbit.position.x) > Margin.x)
			x = Mathf.Lerp(x, Rabbit.position.x, Smoothing.x * Time.deltaTime);

		if (Mathf.Abs(y - Rabbit.position.y) > Margin.y)
			y = Mathf.Lerp(y, Rabbit.position.y, Smoothing.y * Time.deltaTime);

		var cameraHalfWidth = camera.orthographicSize * ((float)Screen.width / Screen.height);

		transform.position = new Vector3(x, y, transform.position.z);
	}
}
