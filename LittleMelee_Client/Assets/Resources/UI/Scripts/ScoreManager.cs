using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used by method 1
[System.Serializable]
public class Item
{
    public int account;
    public int score;
    public string dateTime;
}

//used by method 2
[System.Serializable]
public class ScoreItem
{
    public string account;
    public string score;
    public string time;
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreItem> scoreList;
}

public class ScoreManager : MonoBehaviour {

    public GameObject scoreRowPefab;
    public Transform contentPanel;
    
    public const int maxScoreNum = 10;
    public List<Item> itemList;

    private string scoreJsonMsg;
    ScoreList scoreMsg;

    private bool canCreateBoardClient=false;

    public static bool SaveScoreDate(int score, string date)
    {
        //Item compareItem=new Item();
        int lastScore;
        string lastDate;

        int i, j;

        for(i=0;i<maxScoreNum;i++)
        {
            //if i has not value ,save 
            if (!PlayerPrefs.HasKey("score" + i.ToString()))
            {
                PlayerPrefs.SetInt("score" + i.ToString(), score);
                PlayerPrefs.SetString("date" + i.ToString(), date);
                return true;
            }
            else if (PlayerPrefs.HasKey("score" + i.ToString()) 
                     && PlayerPrefs.GetInt("score" + i.ToString()) < score) 
            {
                for(j=maxScoreNum;j>i;j--)
                {
                    //if last rank has value,save
                    if(PlayerPrefs.HasKey("score"+(j-1).ToString()))
                    {
                        lastScore = PlayerPrefs.GetInt("score" + (j - 1).ToString());
                        lastDate = PlayerPrefs.GetString("date" + (j - 1).ToString());
                        //save
                        PlayerPrefs.SetInt("score" + j.ToString(), lastScore);
                        PlayerPrefs.SetString("date" + j.ToString(), lastDate);
                    } 
                }
               
                PlayerPrefs.SetInt("score" + j.ToString(), score);
                PlayerPrefs.SetString("date" + j.ToString(), date);

                return true;
            }
        }
        return false;
    }

    public void CreateScoreBoard()
    {
        Item addItem=new Item();
        for (int i=0;i<maxScoreNum;i++)
        {
            if(PlayerPrefs.HasKey("score"+i.ToString()))
            {
                addItem.score = PlayerPrefs.GetInt("score" + i.ToString());
                addItem.dateTime = PlayerPrefs.GetString("date" + i.ToString());
                itemList.Add(addItem);

                GameObject scoreRow = Instantiate(scoreRowPefab) as GameObject;
                ScoreRow scoreRowScript = scoreRow.GetComponent<ScoreRow>();
                scoreRowScript.rankNum.text = i.ToString();
                scoreRowScript.score.text = addItem.score.ToString();
                scoreRowScript.dateTime.text = addItem.dateTime.ToString();

                scoreRow.transform.SetParent(contentPanel,false);
                //Debug.Log(scoreRow.transform.localPosition.ToString());
            }
        }
    }

    private void SaveScoreJsonMsg(string ScoreJsonMsg)
    {
        scoreJsonMsg= "{\"scoreList\":"+ScoreJsonMsg+"}";
        scoreMsg = JsonUtility.FromJson<ScoreList>(scoreJsonMsg);
        //foreach (var item in scoreMsg.scoreList)
        //{
        //    Debug.Log(item.account+item.score+item.time);
        //}
    }

    private void CreateScoreBoardClient()
    {
        if(scoreJsonMsg!=null)
        {
            SaveScoreJsonMsg(scoreJsonMsg);
        }
        for (int i = 0; i < scoreMsg.scoreList.Count&&i<maxScoreNum; i++)
        {
            ScoreItem item = scoreMsg.scoreList[i];
            GameObject scoreRow = Instantiate(scoreRowPefab) as GameObject;
            ScoreRow scoreRowScript = scoreRow.GetComponent<ScoreRow>();
            scoreRowScript.rankNum.text = item.account.ToString();
            scoreRowScript.score.text = item.score.ToString();
            scoreRowScript.dateTime.text = item.time.ToString();
            scoreRow.transform.SetParent(contentPanel, false);
            //Debug.Log(scoreRow.transform.localPosition.ToString());
        }
    }

    public void CanCreateScoreBoardClient(string ScoreMsg)
    {
        scoreJsonMsg = ScoreMsg;
        canCreateBoardClient = true;
        if(scoreJsonMsg[scoreJsonMsg.Length-1]==']')
        {
            CreateScoreBoardClient();
        }    
    }

}
