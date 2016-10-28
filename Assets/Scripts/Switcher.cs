using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Switcher : MonoBehaviour {

	public AudioClip click;
	AudioSource audioPlayer;

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
		ClickSound();
		
	// SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);	// SceneManager.SetActiveScene(1);
	}

	public void CrimeScene () 
	{
		Application.LoadLevel(2);
		ClickSound();
		
	// SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);	// SceneManager.SetActiveScene(1);
	}

	public void ClickSound()
	{
		audioPlayer = GetComponent<AudioSource>();
		audioPlayer.clip = click;
		audioPlayer.Play();
	}
}
