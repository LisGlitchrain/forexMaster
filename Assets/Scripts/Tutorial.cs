using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    RectTransform mainTutorialPanel;
    [SerializeField] RectTransform[] tutorialPanels;
    GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
        mainTutorialPanel = this.GetComponent<RectTransform>();
    }

    // Use this for initialization
    public void RunTutorial(int tutorStep)
    {
        if (tutorStep == 0)
        {
            tutorialPanels[0].gameObject.SetActive(true);
            tutorStep = 1;
        }

        else if (tutorStep == 1)
        {
            tutorialPanels[0].gameObject.SetActive(false);
            tutorialPanels[1].gameObject.SetActive(true);
            tutorStep = 2;
        }

        else if (tutorStep == 2)
        {
            tutorialPanels[1].gameObject.SetActive(false);
            tutorialPanels[2].gameObject.SetActive(true);
            tutorStep = 3;
        }

        else if (tutorStep == 3)
        {
            tutorialPanels[2].gameObject.SetActive(false);
            tutorialPanels[3].gameObject.SetActive(true);
            tutorStep = 4;
        }

        else if (tutorStep == 4)
        {
            tutorialPanels[3].gameObject.SetActive(false);
            tutorialPanels[4].gameObject.SetActive(true);
            tutorStep = 5;
        }

        else if (tutorStep == 5)
        {
            tutorialPanels[4].gameObject.SetActive(false);
            tutorialPanels[5].gameObject.SetActive(true);
            tutorStep = 6;
        }

        else if (tutorStep == 6)
        {
            tutorialPanels[5].gameObject.SetActive(false);
            tutorialPanels[6].gameObject.SetActive(true);
            tutorStep = 7;
        }

        else if (tutorStep == 7)
        {
            tutorialPanels[6].gameObject.SetActive(false);
            gm.firstRun = false;
            gm.ResumeGame();
            mainTutorialPanel.gameObject.SetActive(false);

        }
    }
}
