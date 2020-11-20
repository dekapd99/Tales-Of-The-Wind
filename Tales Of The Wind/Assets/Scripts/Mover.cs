using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //bagian dari Nav Mesh Agent = AI

public class Mover : MonoBehaviour
{
    [SerializeField] Transform target;
    // Start is called before the first frame update
    Ray lastRay;

    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MoveToCursor(); //method movetocursor
        }
        // Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
        // lastRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // GetComponent<NavMeshAgent>().destination = target.position;
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit); //nembak Ray, dapet posisi lalu di store di Hit lalu di passing keluar ke kurusor yang baru
        if (hasHit)
        {
            GetComponent<NavMeshAgent>().destination = hit.point; //kalo cursor udh kita klik ke plan, si agent nergerak ke titik/point baru tersebut
        }
    }
}
