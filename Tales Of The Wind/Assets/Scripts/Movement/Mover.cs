using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //bagian dari Nav Mesh Agent = AI
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] Transform target;
        //custom max speed
        [SerializeField] float maxSpeed = 6f;
        //private variable navmeshagent
        NavMeshAgent navMeshAgent;
        //cache reference untuk health
        Health health;

        void Start() 
        {
            //declare navMeshAgent supaya gak pake getComponent terus
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        private void Update()
        {
            //menghilangkan navMeshAgent apabila enemy/player sudah mati
            navMeshAgent.enabled = !health.IsDead();
            // method update animator
            UpdateAnimator();
        }
        //passing in speedFraction ke parameter
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            //canceling fighting sebelum kita starting moving
            
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            //kalo cursor udh kita klik ke plane, si agent nergerak ke destination baru tersebut
            navMeshAgent.destination = destination;
            //Mathf.Clamp01() --> berapapun nilai value diantara 0-1 
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            //terjadi ketika kursor tidak mendekati lawan maka player terus bergerak
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            //isStopped Documentation Unity
            //terjadi ketika mendekati lawan maka player akan berhenti
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // variable global velocity = perubahan posisi terhadap waktu
            Vector3 velocity =  navMeshAgent.velocity;
            /*convert global velocity ke local velocity
            alasannya: saat kita menggunakan global velocity kita make koordinat global dimana velocity animasi itu
            akan selalu berubah kalau animasi bergerak dalam terrain. konversi local ini memberitau si animasi ini
            apakah dia lari kedepan atau berjalan... karena ini kita menggunakan InverseTransformDirection buat ngasih
            tau komponen animator, dimana animasi berada untuk bergerak terus.
            kalau menggunakan global ini Vector3 velocity = GetComponent<NavMeshAgent>().velocity; doang itu gak bisa
            ngasih tau animasi buat melakukan komando
            */ 
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            //variable speed digunakan untuk memberitahu animasi berapa velocity untuk animasi maju kedepan (sumbu z)
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
    }
}
