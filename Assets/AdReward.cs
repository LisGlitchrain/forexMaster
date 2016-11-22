using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdReward : MonoBehaviour
{
	
	public Button AdButton;

	public void ShowRewardedAd()
	    {
	        const string RewardedZoneId = "rewardedVideo";

	        #if UNITY_ADS
	        if (!Advertisement.IsReady(RewardedZoneId))
	        {
	            Debug.Log(string.Format("Ads not ready for zone '{0}'", RewardedZoneId));
	            return;
	        }

	        var options = new ShowOptions { resultCallback = HandleShowResult };
	        Advertisement.Show(RewardedZoneId, options);
	        #endif
	    }

	    #if UNITY_ADS
	    private void HandleShowResult(ShowResult result)
	    {
	        switch (result)
	        {
	            case ShowResult.Finished:
	                Debug.Log("The ad was successfully shown.");
	                //
	                // YOUR CODE TO REWARD THE GAMER
	                // Give coins etc.
	                GameManager.instance.topSessionProfit += 20;
	                GameManager.instance.deposit += 20;
	                GameManager.instance.tSPText.text = GameManager.instance.topSessionProfit.ToString();
	                AdButton.enabled = false;
	                // AdButton.colors = Color.red;

	                break;
	            case ShowResult.Skipped:
	                Debug.Log("The ad was skipped before reaching the end.");
	                break;
	            case ShowResult.Failed:
	                Debug.LogError("The ad failed to be shown.");
	                break;
	        }
	    }
 	   #endif
}