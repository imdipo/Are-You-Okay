using Newtonsoft.Json; // Anda perlu menginstal package Newtonsoft.Json di Unity
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
// Kelas untuk Request ke Modal.com
[System.Serializable]
public class LLMRequest
{
    // Struktur harus sesuai dengan yang diterima oleh FastAPI Anda
    public Message[] messages;
}

// Struktur pesan untuk dikirim ke API (sesuai dengan format chat template)
[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

// Kelas untuk Response dari Modal.com
[System.Serializable]
public class LLMResponse
{
    // Harus sesuai dengan JSONResponse yang dikembalikan oleh Modal.com
    public string response;
}

[System.Serializable]
public class JudgeResponse
{
    // Kunci ini harus sama persis dengan JSON output yang dihasilkan Juri LLM Anda
    public string result;
    public float score;
    public string feedback;
    public string player_final_advice;
}


public class LLM_CHAT_API : MonoBehaviour
{
    private List<Message> conversationHistory = new List<Message>();
    private int turnCount = 0;
    private const int MAX_TURNS = 5;
    // Ganti dengan URL endpoint Modal.com Anda
    [SerializeField] private string modalApiUrl = "URL_ENDPOINT";
    [SerializeField] private string judgeApiUrl = "URL_JURI";
    [SerializeField] private TMP_InputField playerInputField;

    // Referensi ke ScriptManager (agar bisa menampilkan pesan)
    private ScriptManager scriptManager;

    private void Start()
    {
        // Cari dan simpan referensi ScriptManager
        scriptManager = FindAnyObjectByType<ScriptManager>();

        // ** (Opsional) Langsung Kirim Pesan Pertama saat Start **
        // Untuk memicu percakapan awal ("Kamu ngga apa apa?")
        StartCoroutine(MulaiPercakapanOtomatis());
    }

    private IEnumerator MulaiPercakapanOtomatis()
    {
        // Pesan awal yang akan dikirim ke model (LLM yang akan menjawab)
        string initialPrompt = "Kamu ngga apa apa?";

        // Simulasi pesan Player: "Kamu ngga apa apa?"
        // Kita gunakan prompt ini untuk memicu respon awal dari LLM

        if (scriptManager != null)
        {
            scriptManager.menambahkanPesan(initialPrompt, true);
        }

        // 1. Tambahkan pesan pemicu ke riwayat
        conversationHistory.Add(new Message { role = "user", content = initialPrompt });

        // 2. Kirim riwayat (yang baru berisi 1 pesan)
        yield return KirimPesanKeAPI(conversationHistory.ToArray());
    }

    public void KirimPesanDariInputField(TMP_InputField inputField)
    {
        // 1. Ambil teks dari input field
        string pesanPlayer = inputField.text;

        // 2. Cek apakah pesan kosong (pencegahan error)
        if (string.IsNullOrWhiteSpace(pesanPlayer))
        {
            Debug.Log("Pesan kosong, tidak dikirim.");
            return;
        }

        // 3. Panggil fungsi utama untuk mengirim dan menampilkan
        KirimPesanPlayer(pesanPlayer);

        // 4. Kosongkan input field setelah dikirim (UX)
        inputField.text = "";
        inputField.ActivateInputField(); // Opsional: Fokuskan kembali kursor
    }

    // Catatan: Fungsi KirimPesanPlayer(string pesanPlayer) yang lama tetap ada, 
    // tetapi sekarang hanya dipanggil secara internal.


    // Fungsi Publik yang akan dipanggil oleh script lain (misalnya dari tombol Kirim)
    public void KirimPesanPlayer(string pesanPlayer)
    {
        if (turnCount >= MAX_TURNS)
        {
            Debug.Log("Batas giliran sudah tercapai. Data dikirim untuk penilaian.");
            return; // Mengabaikan input setelah batas
        }
        // 1. Tampilkan pesan Player di UI
        if (scriptManager != null)
        {
            scriptManager.menambahkanPesan(pesanPlayer, true);
        }


        // 2. Tambahkan pesan Player ke riwayat
        conversationHistory.Add(new Message { role = "user", content = pesanPlayer });

        // 3. Kirim SELURUH riwayat ke API
        StartCoroutine(KirimPesanKeAPI(conversationHistory.ToArray()));
    }

    private IEnumerator KirimPesanKeAPI(Message[] currentMessages)
    {
        // 1. Persiapan Data (Menggunakan array currentMessages yang berisi seluruh riwayat)
        LLMRequest requestData = new LLMRequest { messages = currentMessages };

        // Serialisasi data C# ke JSON
        string jsonPayload = JsonConvert.SerializeObject(requestData);

        // 2. Siapkan Request
        using (UnityWebRequest www = new UnityWebRequest(modalApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // 3. Proses Respon JSON (Urutan Kritis Dibenahi)
                string jsonResponse = www.downloadHandler.text;

                // Deserialisasi JSON ke objek C#
                LLMResponse responseData = JsonConvert.DeserializeObject<LLMResponse>(jsonResponse);

                // Ambil jawaban dari objek yang sudah didefinisikan
                string jawabanLLM = responseData.response.Trim();

                conversationHistory.Add(new Message { role = "assistant", content = jawabanLLM });

                // === LOGIKA HITUNGAN DAN PENILAIAN BARU ===
                turnCount++;
                Debug.Log($"Giliran ke-{turnCount} selesai.");

                // 4. Tambahkan Pesan LLM ke UI
                if (scriptManager != null)
                {
                    scriptManager.menambahkanPesan(jawabanLLM, false); // false = LLM
                }

                if (turnCount >= MAX_TURNS)
                {
                    // PENTING: Panggil fungsi baru untuk mengirim data ke Juri
                    StartCoroutine(KirimDataUntukPenilaian(conversationHistory.ToArray()));
                }

            }
            else
            {
                Debug.LogError($"LLM API Error: {www.error}");
            }
        }
    }

    private IEnumerator KirimDataUntukPenilaian(Message[] finalHistory)
    {
        // 1. Serialisasi data riwayat (sudah dalam format array of Message)
        string jsonPayload = JsonConvert.SerializeObject(new LLMRequest { messages = finalHistory });

        // 2. Kirim ke Judge API Endpoint
        using (UnityWebRequest www = new UnityWebRequest(judgeApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data berhasil dikirim ke Juri LLM untuk penilaian.");
                string judgeResponse = www.downloadHandler.text;
                Debug.Log($"Respon Juri: {judgeResponse}");

                // START: Blok TRY ditambahkan untuk menangani error deserialisasi JSON
                try
                {
                    // DESERIALISASI
                    JudgeResponse responseData = JsonConvert.DeserializeObject<JudgeResponse>(judgeResponse);
                    bool playerWon = (responseData.result != null && responseData.result.ToLower() == "win");

                    if (playerWon)
                    {
                        string winMessage = $"Selamat! Anda berhasil membantu LLM! Skor Juri: {responseData.score:F2}.\nFeedback: {responseData.feedback}";

                        // Tampilkan pesan Menang
                        if (scriptManager != null)
                        {
                            scriptManager.tampilkanPesanAkhir(winMessage, true); // true = Menang
                        }
                        
                    }
                    else
                    {
                        string loseMessage = $"Sayang sekali, Anda kalah. Skor Juri: {responseData.score:F2}.\nFeedback: {responseData.feedback}";

                        // Tampilkan pesan Kalah
                        if (scriptManager != null)
                        {
                            scriptManager.tampilkanPesanAkhir(loseMessage, false); // false = Kalah
                        }
                    }

                    if (UI.instance != null)
                    {
                        UI.instance.EnableLLMRestartButton();
                        Time.timeScale = 1f;
                    }
                }
                catch (System.Exception e) // CATCH: Menangani error jika JSON tidak valid
                {
                    Debug.LogError($"Gagal memproses JSON dari Juri: {e.Message}. Respon Mentah: {judgeResponse}");
                    if (scriptManager != null)
                    {
                        // Tampilkan pesan kegagalan penilaian
                        scriptManager.tampilkanPesanAkhir("Terjadi kesalahan saat penilaian Juri (JSON tidak valid).", false);
                    }
                }
                // END: Blok catch/try

            }
            else // ELSE: Menangani error koneksi HTTP
            {
                Debug.LogError("Gagal mengirim data ke Juri LLM: " + www.error);
                if (scriptManager != null)
                {
                    scriptManager.tampilkanPesanAkhir("Gagal terhubung ke Juri LLM. Cek URL/Koneksi.", false);
                }
            }
            DisableInput();
        }


    }
    public void DisableInput()
    {
        if (playerInputField != null)
        {
            playerInputField.interactable = false;
            playerInputField.text = "Game Selesai.";
        }
    }


}