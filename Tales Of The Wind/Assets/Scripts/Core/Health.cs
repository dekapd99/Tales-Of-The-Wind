using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour 
    {
        //serializefield disini digunakan untuk mengaktifkan konfigurasi 
        [SerializeField] float healthPoints = 100f;
        
        //ini bool buat assign variabel apakah enemy udah mati/belum --> disini artinya FALSE = belum mati
        bool isDead = false;
        
        //ini simple class untuk mengetahui apakah enemy udah mati atau belum?
        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            //0 sebagai batasnya
            //fungsi matematika untuk menghitung dengan cara mengambil nilai max dari value ini
            //yaitu 100, dan 0 --> jika nanti nilainya minus maka 0 akan lebih besar dari nilai minus
            //maka kita akan mengambil nilai akhir yaitu 0 jadi gak mungkin angkanya kurang dari 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            // healthPoints == 0 --> kenapa tidak <= 0 ?? karena di code healthPoints = Mathf.Max(healthPoints - damage, 0);
            //batasnya gak mungkin kurang dari 0 jadinya healthPoints == 0
            if (healthPoints == 0)
            {
                //method die
                Die();
            }
        }

        private void Die()
        {
            //cek apakah sudah mati, kalau memang true, ya langsung return karena kalau udah mati yaudah mati
            //jangan sampai enemy melakukan apa apa setelah mati
            if(isDead) return;
            //kalau bener mati maka lanjutkan dengan mentrigger animasi die
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            //untuk memberitahu ke actionscheduler agar cancel StartAction untuk memberhentikan enemy/player yang sudah mati
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
