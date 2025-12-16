using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D badanPesawat;
    protected Collider2D coli;
    protected SpriteRenderer sr;

    //public Collider2D[] enemyColliders; // tidak dinamis, ukuran tetap // tidak bisa menambah/mengurangi item // faste // anyway ini kita jadiin local variable 
    // public List<Collider2D> exampleList; // dinamis, bisa menambah/mengurangi item // slower

    [Header("Health")]
    [SerializeField] private int maxNyawa = 1;
    [SerializeField] private int nyawaSekarang;
    [SerializeField] private Material damageMaterial; 
    [SerializeField] private float durasiDamageFeedback = .2f; 
    private Coroutine damageFeedbackCorotine; // corroutine allow us to post functionnality of a certain amount of time
    // the reason why we need to store it in a variable is so we can run a multiple coroutine at the same time. 
    // will mess the functionality when get multiple damage at the same time = run multiple couroutine. thats why we store it in variable so we can stop the active and start a new one

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;
    
    [Header("Collision Details")]
    [SerializeField] private float cekJarakTanah; // akan ada laser untuk cek jarak antara player dan tanah
    [SerializeField] protected bool isGrounded;
    [SerializeField] private LayerMask apaItuTanah;

    [Header("Sound Details")] 
    [SerializeField] protected AudioClip attackSound; // File audio tebasan/serangan
    private AudioSource audioSource; // Komponen untuk memutar suara

    // facing direction details
    protected bool bisaBergerak = true;
    protected int facingDir = 1;
    protected bool hadapKanan = true; // karena secara default kita menghadap kanan

    protected virtual void Awake()
    {
        badanPesawat = GetComponent<Rigidbody2D>();
        coli = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>(); // karena tadi animasinya ada di childnya si Entity
        sr = GetComponentInChildren<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>(); // Coba ambil AudioSource di GameObject ini
        if (audioSource == null)
        {
            // Jika belum ada, tambahkan komponen AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Pastikan tidak berbunyi otomatis
        }

        nyawaSekarang = maxNyawa;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
    }

    protected virtual void Update()
    {
        HandleCollision();
        handleKiriKanan();
        handleAnimations();
        handleBalikBadan();       
    }

    public void DamageTarget()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound); // Memutar AudioClip sekali
        }

        // detect anything that is overlapped by the circle we set
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget); // biar biarin
        // dari point kita nge defined (attack point), ke radius tertentu, dan di layer tertentu, kalau ada, maka kita balikkan ke si array

        foreach (Collider2D enemy in enemyColliders)
        {
            Entity entityTarget = enemy.GetComponent<Entity>();
            entityTarget.takeDamage();
        }


    }

    private void takeDamage()
    {
        nyawaSekarang = nyawaSekarang - 1; // means that every damage gives 1 damage

        PlayDamageFeedback();

        if (nyawaSekarang < 0)
            mati();
        
    }
    protected virtual void mati()
    {
        anim.enabled = false; // disable all animations entity has
        coli.enabled = false; // biar jatuh dari tanah

        badanPesawat.gravityScale = 12;
        badanPesawat.linearVelocity = new Vector2 (badanPesawat.linearVelocity.x, 15);

        Destroy(gameObject, 3); // destroy gameobject within 3 seconds, so the animation of falling still visible

    }

    private void PlayDamageFeedback()
    {
        // ngecek apakah ada corotine yang lagi active, kalau ada kita stop
        if (damageFeedbackCorotine != null)
        {
            StopCoroutine(damageFeedbackCorotine);
        }
        // buat pake courotine kita ga bisa langsung nulis "DamageFeedbackCoroutine()", kita harus pakai startCoroutine()
        StartCoroutine(DamageFeedbackCoroutine());
    }

    private IEnumerator DamageFeedbackCoroutine()
    {
        Material OriginalMaterial = sr.material;

        sr.material = damageMaterial;

        yield return new WaitForSeconds(durasiDamageFeedback);

        sr.material = OriginalMaterial;
    }



    public virtual void EnableMovement(bool enable) // public biar bisa dipake diluar script circle
    {
        bisaBergerak = enable;
    }

    protected void handleAnimations()
    {
        // kita perlu akses si isMoving nya, dan kita buat variable secara local ajah, soalnya kita hanya butuh untuk handle animations method
        // dan karena velocity bergerak itu -1/1 jadi kita bisa bedain lewat situ ajhan
        anim.SetFloat("xVelocity", badanPesawat.linearVelocity.x);
        anim.SetFloat("yVelocity", badanPesawat.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    protected virtual void handlePercobaanMenyerang()
    {
        if (isGrounded)
        {
            anim.SetTrigger("attack");
        }
    }

    protected virtual void handleKiriKanan()
    {

    }

    protected virtual void HandleCollision()
    {
        // ini baru untuk cek tanah dibawah kita
        // kita butuh origin point darimana kita casting the rayCast ke darimana kita casting the ray. intinya dari mana kita tembakin laserny
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, cekJarakTanah, apaItuTanah);
        // param2 = arah dari raycast
        // param3 = jarak ke tanah
        // param4 = tanahnya, agar tida mendetect selain tanah


    }

    protected virtual void handleBalikBadan()
    {
        // buat check lagi menghadap kemana dan lagi jalan kemana secara bersamaan
        if (badanPesawat.linearVelocity.x > 0 && hadapKanan == false)
            balikBadan();
        else if (badanPesawat.linearVelocity.x < 0 && hadapKanan == true)
            balikBadan();
    }

    public void balikBadan()
    {
        // transform component selalu tersedia jadi kita ga perlu dapetin dulu
        transform.Rotate(0, 180, 0);
        hadapKanan = !hadapKanan; // update boolean, selain untuk memastikan arah badan juga agar kondisi if di "handleBalikBadan" tidak terpenuhi
        facingDir = facingDir * -1;
    }

    private void OnDrawGizmos()
    {
        // untuk menampilkan garis saja, tidak untuk cek collision 
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -cekJarakTanah));
        // param1 =  center of the player (tempat kita mulai garisnya)
        // param2 = buat tanahnya, misal cekJarakTanah = 5, maka nanti ada garis sepanjang -5 (kebawah)

        if(attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

    }

}

// getkey = kalo tombolnya ditahan
// getkeydown = kalo tombolnya di tekan sekali
// getkeyup = ketika tombolnya di lepas (agak jarang dipake)