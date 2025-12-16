using UnityEngine;

public class entityAnimationEvents : MonoBehaviour
{
    // when game object has animator component on it and if you attach another script to the same game object
    // we will be able to call functions from the scripts.
    // jadi intinya kita bisa panggil method/fungsi dari animat
    // contoh
    // private void mulaiMenyerang()
    // {
        // Debug.Log("mulaiMenyerang");  
    // }

        // memanggil method dari "circle" script
        // method ini harus memberhentikan pergerakan dari player game object
    private Entity entity;

    private void Awake()  
    {
        entity = GetComponentInParent<Entity>(); // karena script ini child game object, parentnya ya si Entity, jadi kalau mau ambil componentnya harus pake "GetComponentInParent"
    }


    public void DamageTarget() => entity.DamageTarget();

    private void disableJumpAndMovement() => entity.EnableMovement(false);   
    private void EnableJumpAndMovement() => entity.EnableMovement(true);
    



}
