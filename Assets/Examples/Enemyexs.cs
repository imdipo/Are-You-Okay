//using UnityEngine;

//// inheritance allows one class to inherit properties or functionalities from another class (parent n child relationship)
//// polymorphism means same function but different execution
//public class Enemyexs : MonoBehaviour
//{
//    // each enemy contain:
//    [SerializeField] protected float kecepatanBerjalan;
//    [SerializeField] protected string namaMusuh; // making the variable public to get. so any script can read the information. but only this script can change the informations
//    // and others like nyawa, damage, armor dll
//    //public string namaMusuh { get; private set; } // another ex of encapsulation, making the variable public to get. so any script can read the information. but only this script can change the informations

//    private void Update()
//    {
//        //MoveAround();

//        if(Input.GetKeyDown(KeyCode.F))
//        {
//            Attack();
//        }
//    }

//    private void MoveAround()
//    {
//        Debug.Log(namaMusuh + " berjalan pada kecepatan " +  kecepatanBerjalan);
//    }

//    protected virtual void Attack() // virtual bikin method ini bisa di override
//    {
//        Debug.Log(namaMusuh + " menyerang!");
//    }

//    public void takeDamage()
//    {

//    }

//    public string GetEnemyName() // contoh encapsulations
//    {
//        return namaMusuh;
//    }
//}
