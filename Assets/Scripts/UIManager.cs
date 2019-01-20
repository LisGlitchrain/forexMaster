using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    Vector3 panelY;
    public Scrollbar influenceBar; //public //size, fade
    [SerializeField] Scrollbar depositBar; //size, fade
    [SerializeField] Text depositText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text influenceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text profitText;   //text
    [SerializeField] Text openPositionNumText;  //text
    [SerializeField] Text quantityText; //text
    [SerializeField] Text resistancePriceText;  //text
    [SerializeField] Text supportPriceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text currentPriceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] RectTransform currentPricePanel;   //yposition, 
    [SerializeField] RectTransform openPositionPricePanel; //ypos
    [SerializeField] Scrollbar expBar; //size
    [SerializeField] RectTransform gameOverPanel; //active
    [SerializeField] RectTransform gamePausedPanel;  //active
    [SerializeField] RectTransform mainTutorialPanel; //???
    [SerializeField] Text tPPText; //text
    [SerializeField] Text tSPText;  //text
    [SerializeField] Text sLengthText;  //text
    [SerializeField] Text expText;  //text

    //Может сделать один метод перерисовки UI, и изменять данные...Или добавить листенера?
    //Через публичные свойства так сделать? установить сеттер с вызовом перерисовки?
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
