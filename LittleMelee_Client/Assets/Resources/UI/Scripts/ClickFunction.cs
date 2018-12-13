using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class ClickFunction : MonoBehaviour {

    public Canvas canvas;
    public Text messageText;
    public GameObject gameOverUI;

	public void ClickButtonPlay()
    {
        SceneManager.LoadScene(1);
        
    }

    public void ClickButtonGameOverReturn()
    {
        StartCoroutine(CloseGameOverBoardAnimation(0));
    }

    public void ClickButtonScore()
    {
        Animator animator = canvas.GetComponent<Animator>();
        if(animator!=null)
        {
            animator.SetBool("IsPressScoreButton", true);
            animator.SetBool("IsPressReturnButton", false);

        }
    }

    public void ClickButtonAbout()
    {
        messageText.text = "A little game:Little Melee \nProducor: stIck \nmade by unity engine";
        Animator animator = canvas.GetComponent<Animator>();
        if (animator != null)
        {
            bool isPressAboutButton = animator.GetBool("IsPressAboutButton");
            bool isPressNoneNowButton = animator.GetBool("IsPressNoneNowButton");
            animator.SetBool("IsPressAboutButton", !isPressAboutButton);
           
            if(isPressNoneNowButton)
            {
                animator.SetBool("IsPressNoneNowButton", false);
            }

        }
    }

    public void ClickButtonMainReturn()
    {
        Animator animator = canvas.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsPressReturnButton", true);
            animator.SetBool("IsPressScoreButton", false);

        }
    }

    public void ClickButtonNoneNow()
    {
        messageText.text = "None now";
        Animator animator = canvas.GetComponent<Animator>();
        if (animator != null)
        {
            bool isPressAboutButton = animator.GetBool("IsPressAboutButton");
            bool isPressNoneNowButton = animator.GetBool("IsPressNoneNowButton");
            animator.SetBool("IsPressNoneNowButton", !isPressNoneNowButton);

            if (isPressAboutButton)
            {
                animator.SetBool("IsPressAboutButton", false);
            }

        }
    }

    public void ClickButtonExit()
    {
        Application.Quit();
    }

    private IEnumerator CloseGameOverBoardAnimation(int value)
    {
        Animator animator = gameOverUI.GetComponent<Animator>();
        animator.SetBool("ShowGameOverBoard", false);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(0);
    }

}
