    using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI instance; // called singleton and it allows us to access the component from any script without getting a reference
    // we can only have one singleton of a certain type, so UI? only one 

    [SerializeField] private GameObject gameOverUI;
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] public int menang;

    [SerializeField] private EnemyRespawner respawner;
    [SerializeField] private ObjectToProtect NPC;

    private int killCount = 0;
    private const int BatasKemenangan = 2;
    private bool sudahMenang = false;

    [Header("LLM Chat")]
    [SerializeField] private GameObject llmChatUI;

    [Header("LLM Outcome")]
    [SerializeField] private GameObject restartButtonLLM;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Komponen AudioSource di scene Anda
    [SerializeField] private AudioClip chatBGM;


    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        if (llmChatUI != null)
        {
            llmChatUI.SetActive(false); // Pastikan UI Chat tertutup saat game dimulai
        }
    }

    private void Update()
    {
        timerText.text = Time.time.ToString("F2") + "s"; // Time.time give us time in sec. toString bcs we want it to be a string, and f2 = ya biasa 2 angka setelah desimal

        if (sudahMenang && NPC != null)
        {
            bool hasil = NPC.ApakahBisaNgobrol;

            if (hasil && Input.GetKeyDown(KeyCode.E))
            {
                if (llmChatUI != null)
                {
                    llmChatUI.SetActive(true);
                }
                PlayChatBGM();
                Time.timeScale = 0f;

                NPC.EffectMenang(false);
                Debug.Log("Membuka UI Chat LLM");
            }
        }

    }

    public void PlayChatBGM()
    {
        if (audioSource != null && chatBGM != null)
        {
            // Pastikan AudioSource diatur untuk looping
            audioSource.loop = true;
            audioSource.clip = chatBGM;
            audioSource.Play();
        }
    }

    public void StopChatBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void EnableGameOverUI()
    {
        Time.timeScale = .5f; // will slowdown time by 50%
        gameOverUI.SetActive(true);
    }

    public void Restart()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex; // mendapatkan index yang kita punya di scene sekarang
        SceneManager.LoadScene(sceneIndex); // akan merestart scenenya
    }

    public void EnableLLMRestartButton()
    {
        if (restartButtonLLM != null)
        {
            restartButtonLLM.SetActive(true);
        }

        StopChatBGM();

        Time.timeScale = 1f;
    }


    public void addKillCount()
    {
        killCount++;
        killCountText.text = killCount.ToString();
        CheckMenang();
    }

    public void CheckMenang()
    {
        if (BatasKemenangan <= killCount && !sudahMenang)  
        {
            sudahMenang = true;
            //Time.timeScale = 0f; // ternyata gamenya stop, aku butuh tetep jalan

            respawner.enabled = false;

            NPC.EffectMenang();

            Debug.Log("Menang");
        }
    }


}
