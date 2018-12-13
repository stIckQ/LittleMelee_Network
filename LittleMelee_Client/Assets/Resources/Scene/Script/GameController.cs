using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [Header("Countdown")]
    public float countdownMinute=2.5f;

    [Header("Controlled UI")]
    public GameObject health;
    public GameObject score;
    public GameObject gameOverUI;
    public GameObject messageBoard;
    public GameObject countdownUI;
    public Text countdownText;

    [Header("GameState")]
    public bool isPlaying;
    public bool canGameOver;


    private string dateTime;
    private int scoreNum;
    private float countdown;
    private string countdownString;

    private GameOverBoardManager gameOverBoardManager;
    private Animator gameOverBoardAnimator;
    private Animator messageBoardAnimator;

    private ClientConnect clientConnect;

    // Use this for initialization
    void Start () {
        isPlaying = true;
        gameOverBoardManager = gameOverUI.GetComponent<GameOverBoardManager>();
        gameOverBoardAnimator = gameOverBoardManager.GetComponent<Animator>();

        StartCoroutine(ShowMessageBoard(0));

        countdown = countdownMinute * 60;

        //set Client Connect
        clientConnect = new ClientConnect();
        clientConnect.serverAddress = "127.0.0.1";
        clientConnect.port = 5381;

        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

        //countdown
        if(countdown>0) countdown -= Time.deltaTime;
        else canGameOver = true;

        if(countdown%60<10)
        {
            countdownString = ((int)(countdown / 60)).ToString() + " : 0" + ((int)(countdown % 60)).ToString();
        }
        else
        {
            countdownString = ((int)(countdown / 60)).ToString() + " : " + ((int)(countdown % 60)).ToString();
        }
        countdownText.text = countdownString;

        //gameover
        if (canGameOver)
        {        
            GameOver();
            canGameOver = false;
        }

	}

    void GameOver()
    {
        //set game state
        isPlaying = false;
        // Time.timeScale = 0;
        
        //save score
        scoreNum = PlayerManager.instance.player.GetComponent<PlayerStates>().scoreNum;
        dateTime = System.DateTime.Now.ToString();
        string value = "{\"account\":\"" + ClientLogin.account + "\",\"password\":\"" + ClientLogin.password + "\",\"score\":\"" + scoreNum + "\",\"time\":\"" + dateTime + "\"}";

        //connect to server
        clientConnect.InitClientConnect();
        clientConnect.SocketSender(value);
        StartCoroutine(ClientCloseConnect(1));

        //show and disappear UI and cursor
        Cursor.visible = true;
        health.SetActive(false);
        score.SetActive(false);
        countdownUI.SetActive(false);
        gameOverBoardManager.ShowBoard(scoreNum);
        gameOverBoardAnimator.SetBool("ShowGameOverBoard", true);


    }

    private IEnumerator ShowMessageBoard(int num)
    {
        messageBoardAnimator = messageBoard.GetComponent<Animator>();
        messageBoardAnimator.SetBool("ShowMessageBoard", true);

        yield return new WaitForSeconds(10);

        messageBoardAnimator.SetBool("ShowMessageBoard", false);
    }

    private IEnumerator ClientCloseConnect(int num)
    {
        yield return new WaitForSeconds(1);
        clientConnect.CloseConnecting();
    }
}
