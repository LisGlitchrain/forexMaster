using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
	float pos = 0;
    float zeroPos = 0;
    float minPos = -1;
    [SerializeField] float speed;

	void Start () {
    }


	public void BackgroundUpdate (float deltaTime)
    {
		pos -= speed * deltaTime;
        if (pos < minPos)
		{
			pos = zeroPos;
		}
		this.transform.position = new Vector3(pos, 0, 0);
	}

}