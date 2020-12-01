using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction 
    {
        //dummy weapon range (2f = ibarat 2meter/2frame)
        [SerializeField] float weaponRange = 2f;
        //timeBetweenAttacks memberikan delay kepada SetTrigger sebelum melakukan attack lagi (1f = ibarat 1 detik)
        [SerializeField] float timeBetweenAttacks = 1f; 
        //weaponDamage memberikan damage kepada enemy sebanyak 5 setiap attacknya
        [SerializeField] float weaponDamage = 5f; 
        
        //semua enemy mempunyai darah termasuk player untuk memudahkan pemanggilan yang spesifik
        //semakin spesifik variablenya maka semakin spesifik juga melakukan ekstraksi nilai komponennya
        //kondisi sekarang adalah kita mengakses Health melalui variable ini
        Health target;
        //menentukan berapa lama jeda waktu dari lastattack sebelumnya
        //menggunakan Mathf Library, kita bisa langsung attack enemy
        //kenapa infinity karena pada if(timeSinceLastAttack > timeBetweenAttacks) akan menghasilkan return true
        //sehingga kedua object bisa menyerang disaat yang bersamaan
        float timeSinceLastAttack = Mathf.Infinity;

        private void Update() 
        {
            //+= artinya increment
            //Time.deltaTime, time setelah lasttime dipanggil
            timeSinceLastAttack += Time.deltaTime;


            //kalau target tidak ada maka lanjut ke if selanjutnya
            if (target == null) return;
            if (target.IsDead()) return;

            //kalau target tidak didalam range
            if(!GetIsInRange())   
            {
                //maka player ke posisi dari target
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                //stop vergerak
                GetComponent<Mover>().Cancel();
                //taroh attack animasi disini karena attack terjadi ketika player stop bergerak/movement
                //method animasi attackbehaviour
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            //dokumentasi unity transform.LookAt
            //fixing bug ketika menyerang lawan player tidak melihat ke lawannya
            transform.LookAt(target.transform);
            //saat kita akan attack maka akan menjalankan animasi attack
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
            //method trigget attack
            TriggerAttack();
            timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            //Fixing bug ketika kita attack musuh terus kabur terus attack balik lagi Player akan diam dulu sebentar baru attack.
            //code dibawah ini artinya animasi stopAttack akan direset agar pada saat kembali lagi ke musuh maka player bisa langsung menyerangnya
            GetComponent<Animator>().ResetTrigger("stopAttack");
            //setelah semua ini akan mentrigger void Hit() event dibawah
            //panggil animasinya
            //SetTrigger cara kerjanya seperti boolean value cuma lebih pinter, tapi trigger bekerja kalau diaktifkan
            //yang artinya transisi akan segera terjadi/dilakukan. contoh saat klik target sekali aja, maka dia aktif sekali aja 
            // dan set value langsung jadi FALSE sehingga membantu canceling attack animation
            GetComponent<Animator>().SetTrigger("attack");
            //reset time sejak attack sebelumnya
        }

        //animation event untuk taking damage
        //taking damage ditaroh disini karena kalau ditaroh di AttackBehaviour(), damage langsung diterima sebelum enemy diserang
        void Hit()
        {
            //fixing bug (error handling pada console) --> null reference exception (saat tidak dapat menemukan target)
            if (target == null) 
            {
                return;
            }
            //saat enemy taking damage
            target.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            //isInRange jika target ada didalam range serangan weaponRange
            //transform.position adalah posisi player
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        //fixing bug dimana capsule collider enemy yang sudah mati menghalangi raycast
        //method ini berfungsi untuk mengetahui apakah musuh bisa diserang
        public bool CanAttack(GameObject combatTarget)
        {
            //jika combatTarget tidak ada maka return false yang berati tidak perlu lanjut kebawah dari if ini 
            //tapi kembali lempar ke PlayerController bagian InteractWithCombat loop if foreach pertama
            if (combatTarget == null) 
            { 
                return false; 
            }
            //storing health variable untuk targetToTest
            Health targetToTest = combatTarget.GetComponent<Health>();
            //jika targetnya ada dan targetnya belum mati maka target tersebut bisa diserang
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            //action scheduler
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        //tadinya kita klik target terus klik area selain target maka dia akan mengaktifkan method cancel untuk cancel
        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            //fixing bug untuk mengatasi apabila kebalikan dari stop attack terjadi
            //melakukan reset trigger sehingga bisa langsung attack
            GetComponent<Animator>().ResetTrigger("Attack");
            //fixing bug
            //cancelling attack animation ketika pergi dari musuh
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}