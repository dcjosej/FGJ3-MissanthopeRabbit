using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PointsText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<Text>().text = "<size=50>" + GlobalData.points + " Humans!</size>";
	}
}
