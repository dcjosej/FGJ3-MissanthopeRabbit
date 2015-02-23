using UnityEngine;
using System.Collections;

public class LevelManager:MonoBehaviour {

	public static LevelManager Instance { get; set; }

	public GameObject [] enemiesPosibles;
	
	private GameObject[] _spawnersEnemies;
	private Vector2 _offset = new Vector2(0, 5);

	public void Awake()
	{
		GlobalData.points = 0;
	}


	public void Start()
	{
		Instance = this;
		_spawnersEnemies = GameObject.FindGameObjectsWithTag("Platform");

		InvokeRepeating("CreateEnemy", 5, 4);
	}


	public void CreateEnemy()
	{
		//print("Creando enemigo: " + );
		try
		{
			Instantiate(enemiesPosibles[Random.Range(0, (enemiesPosibles.Length - 1))], _spawnersEnemies[Random.Range(0, (_spawnersEnemies.Length - 1))].transform.position + (Vector3)_offset, Quaternion.identity);

		}
		catch (System.IndexOutOfRangeException e)
		{
			print(e);
		}
	}


	public void ChangeScene(string nameScene)
	{
		Application.LoadLevel(nameScene);
	}

	public void AddMurderer()
	{
		GlobalData.points ++;
	}
}
