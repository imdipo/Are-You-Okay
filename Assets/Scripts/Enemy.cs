using UnityEngine;
using UnityEngine.Windows;

public class Enemy : Entity
{

    private bool playerTerdeteksi;

    [Header("Movement Details")]
    [SerializeField] protected float moveSpeed = 3.5f;

    protected override void Update()
    {
        base.Update();
        handlePercobaanMenyerang();
    }

    protected override void handlePercobaanMenyerang()
    {
        if (playerTerdeteksi)
            anim.SetTrigger("attack");
    
    }

    protected override void handleKiriKanan()
    {
        if (bisaBergerak)
            badanPesawat.linearVelocity = new Vector2(facingDir * moveSpeed, badanPesawat.linearVelocity.y); // x itu input, y dikasih nilai gitu itu biar axis y-nya ga berubah
        else
            badanPesawat.linearVelocity = new Vector2(0, badanPesawat.linearVelocity.y);
    }   

    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerTerdeteksi = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }

    protected override void mati()
    {
        base.mati();
        UI.instance.addKillCount();
    }
}
