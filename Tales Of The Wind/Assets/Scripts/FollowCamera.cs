using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] Transform target; 
    void Update()
    {
        transform.position = target.position; 
    }
}
