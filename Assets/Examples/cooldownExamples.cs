//using UnityEngine;

//public class cooldownExamples : MonoBehaviour
//{
//    private SpriteRenderer sr;
//    [SerializeField] private float waktuWarnaMerah = 1.5f;
//    // public float timer;

//    public float waktuSaatIniDigame;
//    public float terakhirKaliDamage;

//    private void Awake()
//    {
//        sr = GetComponent<SpriteRenderer>();
//    }

//    private void Update()
//    {

//        MengubahWarna();
//    }

//    private void MengubahWarna()
//    {
//        // kita pake time dot time, dimana ini akan menunjukkan waktu (detik) yang kita miliki di current frame since the start of the application  
//        // bahasa gampangnya, timernya akan mulai ketika kita pencet play button

//        waktuSaatIniDigame = Time.time;

//        if (waktuSaatIniDigame > terakhirKaliDamage + waktuWarnaMerah)
//        {
//            if (sr.color != Color.white)
//                sr.color = Color.white;
//        }

//        // another way we play with delays, deltaTime gives us interval in seconds that it took from the last frame to current one
//        // Debug.Log(Time.deltaTime); // un slash if you want to see how it works


//        // another way to make timer
//        // timer -= Time.deltaTime; // same with timer = timer - Time.deltaTime;

//        // if(timer < 0 && sr.color != Color.white)
//        // sr.color = Color.white;
//    }

//    // contoh
//    // [ContextMenu("Update Timer")]
//    // private void updateTimer() => timer = waktuWarnaMerah;

//    public void takeDamage()
//    {
//        sr.color = Color.red;
//        terakhirKaliDamage = Time.time;

        
//        // another way to make a timer
//        // timer = waktuWarnaMerah;

        
//        // invoke allows us to call method with a delay, tho we not use it ill keep it here for idk
//        // Invoke(nameof(turnWhite), waktuWarnaMerah); // nama method pake nameof biar jadi string + kalau ganti nama method nanti ini juga ikut keganti
//    }

//    private void turnWhite()
//    {
//        sr.color = Color.white;
//    }

//}
