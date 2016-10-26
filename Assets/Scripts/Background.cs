using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
	float pos = 0;

	void Start () {
	}


	void Update () {

		pos += (GameManager.instance.gameSpeed/1000)*16;
		if (pos > 1.0f)
			pos -= 1.0f;

			// Debug.Log("POS " + pos);

		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(pos, 0);
	}

}