using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Switcher : MonoBehaviour {

	void Awake ()
	{
		// SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

	}
	// Use this for initialization
	// void Start () {
	
	// }
	
	// Update is called once per frame
	// void Update () {
	
	// }

	public void StartNewGame () 
	{
		Application.LoadLevel(1);
		
	// SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);	// SceneManager.SetActiveScene(1);
	}

	public void CrimeScene () 
	{
		Application.LoadLevel(2);
		
	// SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);	// SceneManager.SetActiveScene(1);
	}
}
