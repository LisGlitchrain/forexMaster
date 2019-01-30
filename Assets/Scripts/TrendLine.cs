using UnityEngine;
using System.Collections;

public class TrendLine : MonoBehaviour, IPauseble
{
    ParticleSystem ps;

    /// <summary>
    /// Pauses trendLine.
    /// </summary>
    public void Pause()
    {
        ps.Pause(false);
    }
    /// <summary>
    /// Resumes trendLine.
    /// </summary>
    public void Resume()
    {
        ps.Play(false);
    }

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
	
	}
	
	/// <summary>
    /// Update position of trendLine
    /// </summary>
    /// <param name="coin"></param>
	public void UpdateLine (CoinController coin) {
		transform.position = new Vector2(coin.transform.position.x, coin.transform.position.y);
	}
}
