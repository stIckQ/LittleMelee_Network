using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

	public GameObject player;
    

    [Range(0.01f,1.0f)]
    public float smoothFactor=0.5f;

    public float rotationSpeed=5.0f;
    public bool rotateAroundPlayer=true;

    [Range(0.0f,5.0f)]
    public float lookAtHeight = 2.0f;

	private Vector3 offset;
    private Vector3 lookAtPosition;

    private GameObject gameManager;
    private GameController gameController;

    // Use this for initialization
    void Start () {
		offset = transform.position-player.transform.position;

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameController = gameManager.GetComponent<GameController>();
    }
	

	void LateUpdate () {
        if(gameController.isPlaying)
        {
            if (rotateAroundPlayer)
            {
                Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
                offset = camTurnAngle * offset;
            }

            Vector3 newPosition = player.transform.position + offset;
            transform.position = Vector3.Slerp(transform.position, newPosition, smoothFactor);

            lookAtPosition = player.transform.position;
            lookAtPosition.y += lookAtHeight;
            transform.LookAt(lookAtPosition);
        }
	}
}
