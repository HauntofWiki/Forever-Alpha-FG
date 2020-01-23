using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryMovement : MonoBehaviour
{
    private GameObject _centerPlanet;
    public Transform target;
    private Vector3 _gravity;

    private Vector3 _moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        _gravity = Physics.gravity;
        _centerPlanet = GameObject.Find("Main Planet");
        target = _centerPlanet.transform;

    }
    

    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        transform.LookAt(target);
        //transform.position.
    }
}
