using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientLogin : MonoBehaviour {
    
    [Header("ControlledUI")]
    public Canvas canvas;
    public GameObject Panel_Main;
    public GameObject Panel_Login;
    public GameObject Btn_Score;
    public InputField accountIF;
    public InputField passwordIF;
    public Text Txt_ShowMsg;
    public Button Btn_Play;
    public Button Btn_Option;
    public Button Btn_About;
    public Button Btn_Login;


    public GameObject CSConnect;
    public GameObject scoreBoardController;

    public static string account;
    public static string password;

    private ClientConnect clientConnect;
    private bool canReceiveLogin=false;
    private string resultStr="";
    private Animator animator;
    private ScoreManager scoreManager;
    private Animator showMsgAnimator;

    private static string MSG_loginSuccess="Login success";
    private static string MSG_registerSuccess = "register success";
    private static string MSG_passwordError = "Password is wrong";
    // Use this for initialization
    void Start () {
        Panel_Main.SetActive(false);
        Panel_Login.SetActive(true);
        Btn_Score.SetActive(false);
        clientConnect = CSConnect.GetComponent<ClientConnect>();
        clientConnect.InitClientConnect();
        animator = canvas.GetComponent<Animator>();
        scoreManager = scoreBoardController.GetComponent<ScoreManager>();
        showMsgAnimator = Txt_ShowMsg.GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {

        if(canReceiveLogin)
        {
            resultStr = clientConnect.GetResultStr();  
            if(resultStr!=null&&resultStr.Length!=0)
            {
                //Debug.Log(resultStr.Length);
                //Debug.Log(resultStr);
                if (resultStr.Substring(0, MSG_loginSuccess.Length) == MSG_loginSuccess)
                {
                    StartCoroutine(WaitCloseLogin(1));
                    Txt_ShowMsg.text = MSG_loginSuccess;
                    //clientConnect.EmptyByte();
                    canReceiveLogin = false;
                }
                else if (resultStr.Substring(0, MSG_registerSuccess.Length) == MSG_registerSuccess)
                {
                    StartCoroutine(WaitCloseLogin(1));
                    Txt_ShowMsg.text = MSG_registerSuccess;
                    canReceiveLogin = false;
                }
                else if (resultStr.Substring(0, MSG_passwordError.Length) == MSG_passwordError)
                {
                    Txt_ShowMsg.text = MSG_passwordError;
                    //clientConnect.EmptyByte();
                    canReceiveLogin = false;
                    Debug.Log("wrong");
                }
            } 
        }
	}

    public void Login()
    {
        account = accountIF.text;
        password = passwordIF.text;
        string sendMsg = "{"+"\"account\":\""+account+"\","+"\"password\":\""+password+"\"}";
        clientConnect.SocketSender(sendMsg);
        canReceiveLogin = true;
        //StartCoroutine(WaitCloseLogin(1));
    }

    private IEnumerator WaitCloseLogin(int num)
    {
        animator.SetBool("Login", true);
        //when play canvas animator,set the btn animator enabled false;
        SetUseBtnAnimator(false);

        //wait 1 sec, set ui sate
        yield return new WaitForSeconds(1f);
        Panel_Login.SetActive(false);
        Panel_Main.SetActive(true);
        Btn_Score.SetActive(true);

        //wait 1 sec,
        yield return new WaitForSeconds(1f);
        animator.SetBool("Login", false);
        //after play the canvas animator,set the button animator enabled  
        SetUseBtnAnimator(true);
        // get serversender message
        string scoreJsonMsg = clientConnect.GetResultStr();
        scoreJsonMsg = scoreJsonMsg.Replace("\0","");
        scoreManager.CanCreateScoreBoardClient(scoreJsonMsg);
        clientConnect.CloseConnecting();

        //fade out txt_showMsg
        showMsgAnimator.SetBool("CanFadeOut", true);

        yield return new WaitForSeconds(1f);
        showMsgAnimator.SetBool("CanFadeOut", false);
        Txt_ShowMsg.gameObject.SetActive(false);
    }

    private void SetUseBtnAnimator(bool b)
    {
        Btn_Play.GetComponent<Animator>().enabled = b;
        Btn_Option.GetComponent<Animator>().enabled = b;
        Btn_About.GetComponent<Animator>().enabled = b;
        Btn_Login.GetComponent<Animator>().enabled = b;
    }
}
