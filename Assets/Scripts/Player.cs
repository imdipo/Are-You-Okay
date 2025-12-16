using UnityEngine;

public class Player : Entity
{
    [Header("Movement Details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float tinggiLompat = 12;

    public static Vector3 posisiTerakhir;
    private float xInput;
    private bool bisaMelompat = true;

    protected override void Update()
    {
        handleInput();
        base.Update();
    }

    private void handleInput() // buat handle input xixixi
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            mencobaMelompat();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            handlePercobaanMenyerang();
    }

    protected override void handleKiriKanan()
    {
        if (bisaBergerak)
            badanPesawat.linearVelocity = new Vector2(xInput * moveSpeed, badanPesawat.linearVelocity.y); // x itu input, y dikasih nilai gitu itu biar axis y-nya ga berubah
        else
            badanPesawat.linearVelocity = new Vector2(0, badanPesawat.linearVelocity.y);
    }

    private void mencobaMelompat()
    {
        if (isGrounded && bisaMelompat)
            badanPesawat.linearVelocity = new Vector2(badanPesawat.linearVelocity.x, tinggiLompat);
    }

    public override void EnableMovement(bool enable)
    {
        base.EnableMovement(enable);
        bisaMelompat = enable;
    }

    protected override void mati()
    {
        base.mati();
        posisiTerakhir = this.transform.position;
        UI.instance.EnableGameOverUI();

    }
}
