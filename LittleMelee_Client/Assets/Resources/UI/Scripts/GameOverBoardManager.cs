using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameOverBoardManager : MonoBehaviour {

    public GameObject gameOverUIController;
    public Text winText;
    public Text scoreText;
    public Image Champion;

    public void GameStart()
    {
        gameOverUIController.SetActive(false);
    }

    public void ShowBoard(float scoreNum)
    {
        Sprite sprite=new Sprite();  

        if (scoreNum==5)
        { 
            winText.text="Perfect!";
            sprite = Resources.Load("UI/Texture/UI_Icon_Crown", sprite.GetType()) as Sprite;
        }
        else if(scoreNum>=3)
        {
            winText.text = "Good!";
            sprite = Resources.Load("UI/Texture/Emo1", sprite.GetType()) as Sprite;
        }
        else
        {
            winText.text = "Bad Score!";
            sprite = Resources.Load("UI/Texture/UI_Icon_SmileyUnhappy", sprite.GetType()) as Sprite;
        }

        Champion.GetComponent<Image>().sprite = sprite;
        scoreText.text = scoreNum.ToString();

        gameOverUIController.SetActive(true);
    }
}
