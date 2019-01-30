using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour {

    [SerializeField] GameObject obstacleToSpawn;
	[SerializeField] int frequency;
    [SerializeField] float obstacleSpeed;
    [SerializeField] float deleteXcoord;

    List<GameObject> obstacles;

    GameObject lastSpawnedCandle;
    float candleHeight;
	float candleWidth;

	// Use this for initialization
	void Start () {
		candleWidth = obstacleToSpawn.GetComponent<BoxCollider2D>().size.x;
        obstacles = new List<GameObject>();
	}
	
	public void UpdateIt(float deltaTime)
    {
        ObstacleSpawn();
        MoveObstacles(obstacles, deltaTime);
        DeleteObstacles(obstacles, deleteXcoord);
    }


    void MoveObstacles(List<GameObject> obstacles, float deltaTime)
    {
        for (var i =0; i< obstacles.Count; i++)
        {
            obstacles[i].transform.position = new Vector3(obstacles[i].transform.position.x - obstacleSpeed * deltaTime, 
                                                          obstacles[i].transform.position.y, 
                                                          obstacles[i].transform.position.z);
        }
    }

    void DeleteObstacles(List<GameObject> obstacles, float deleteXcoord)
    {
        for (var i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].transform.position.x < deleteXcoord)
            {
                DestroyImmediate(obstacles[i]);
                obstacles.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// Smart spawn.
    /// </summary>
	void ObstacleSpawn () {
        if (lastSpawnedCandle == null)
        {
            lastSpawnedCandle = new GameObject();
            lastSpawnedCandle.transform.position = new Vector3(0, 0, 0);
        }
		if (lastSpawnedCandle.transform.position.x + candleWidth < this.transform.position.x)
		{
			int probability = Random.Range(0, frequency);
			if (probability == 1) Spawn (obstacles);
		}
	}

	void Spawn (List<GameObject> obstacles) 
	{
		//Debug.Log("SPAWN!");
		transform.position = new Vector3 (transform.position.x, Random.Range(3.0f,1.0f), 0);
		lastSpawnedCandle = Instantiate (obstacleToSpawn, transform.position, transform.rotation);
        obstacles.Add(lastSpawnedCandle);
    }
}
