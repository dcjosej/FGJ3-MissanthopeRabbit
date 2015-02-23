using UnityEngine;
using System.Collections;

public class ButtonReceiver : MonoBehaviour {


	public void ChangeScene(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
