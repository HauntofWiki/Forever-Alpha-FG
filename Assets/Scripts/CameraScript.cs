using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float _player1X;
    private float _player1Y;
    private Transform _player1;
    private float _player2X;
    private float _player2Y;
    private Transform _player2;
    private Transform _cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _player1 = GameObject.Find("Player 1").GetComponent<Transform>();
        _player2 = GameObject.Find("Player 2").GetComponent<Transform>();
        _cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Player1.position.y > Player2.position.y)
        {
            var p1Position = Player1.position.y;
            transform.Translate(0,Player1.position.y + 4,0);
        }*/
            
        //else 
            //this.transform.position.y = Player2.position.y + 4;
    }
}
