using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject player;

    public GameObject LineCastObject_0;
    public GameObject LineCastObject_1;
    public GameObject LineCastObject_2;
    public GameObject LineCastObject_3;
    public GameObject LineCastObject_4;
}
