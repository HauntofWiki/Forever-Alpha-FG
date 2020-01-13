using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject _player1;
    private GameObject _player2;
    private Camera camera;
    private float _cameraHeightOffset = 1.0f;
    private float _cameraDefaultZ = -4;
    private float _heightOfCamera;
    private float _centerXBetweenCharacters;
    private Vector3 _moveCameraPosition;
    private float _stageMaxX = -18;
    private float _stageMinX = 18;
    private float _stageMaxY;
    private float _stageMinY;

    // Start is called before the first frame update
    void Start()
    {
        _player1 = GameObject.Find("Player1");
        _player2 = GameObject.Find("Player2");
        camera = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _centerXBetweenCharacters = (_player1.transform.position.x + _player2.transform.position.x) / 2;
        
        _heightOfCamera = ((_player1.transform.position.y + _player2.transform.position.y) / 2) + _cameraHeightOffset;
     
        
        _moveCameraPosition = new Vector3(_centerXBetweenCharacters,_heightOfCamera,_cameraDefaultZ);
        //Debug.Log(_player1.transform.position.x + ","+ _player2.transform.position.x+"," + camera.transform.position.x);
        transform.position = Vector3.Lerp(transform.position,_moveCameraPosition, 2.0f);
    }
}
