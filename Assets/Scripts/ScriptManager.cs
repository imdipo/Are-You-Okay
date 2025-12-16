using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public GameObject chatBubblePrefab;
    public Transform contentPanel;

    public Color warnaPLayer = Color.blue;
    public Color warnaLLM = Color.red;
    public float maxWidth = 400f; // max lebar bubble
    public Color warnaPesanAkhir = Color.green;

    private void Start()
    {
        warnaPLayer = Color.blue;
        warnaLLM = new Color(1f, 0.4f, 0.6f);
    }


    public void menambahkanPesan(string textPesan, bool apakahPlayer)
    {
        GameObject BubbleChat = Instantiate(chatBubblePrefab, contentPanel);

        Image backgroundImage = BubbleChat.GetComponentInChildren<Image>();

        TMP_Text textComponent = BubbleChat.GetComponentInChildren<TMP_Text>();

        HorizontalLayoutGroup layoutGroup = BubbleChat.GetComponent<HorizontalLayoutGroup>();

    

        if (textComponent != null )
        {
            textComponent.text = textPesan;
        }

        if (backgroundImage != null && layoutGroup != null)
        {
            if (apakahPlayer)
            {
                backgroundImage.color = warnaPLayer;
                layoutGroup.childAlignment = TextAnchor.MiddleRight;
                textComponent.alignment = TextAlignmentOptions.Right;
            }
            else
            {
                backgroundImage.color = warnaLLM;
                layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                textComponent.alignment = TextAlignmentOptions.Left;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(BubbleChat.GetComponent<RectTransform>());

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.GetComponent<RectTransform>());
        }



    }

    public void tampilkanPesanAkhir(string pesanAkhir, bool isWin)
    {
        // 1. Buat Bubble Chat
        GameObject BubbleChat = Instantiate(chatBubblePrefab, contentPanel);

        Image backgroundImage = BubbleChat.GetComponentInChildren<Image>();
        TMP_Text textComponent = BubbleChat.GetComponentInChildren<TMP_Text>();
        HorizontalLayoutGroup layoutGroup = BubbleChat.GetComponent<HorizontalLayoutGroup>();

        // 2. Set Teks
        if (textComponent != null)
        {
            textComponent.text = pesanAkhir;
        }

        // 3. Set Tampilan Pesan Akhir
        if (backgroundImage != null && layoutGroup != null)
        {
            // Tampilkan di tengah atau di kiri/kanan. Kita set di tengah sebagai pesan sistem.
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            textComponent.alignment = TextAlignmentOptions.Center;

            // Atur warna berdasarkan Menang atau Kalah
            if (isWin)
            {
                backgroundImage.color = Color.green; // Warna Hijau untuk Menang
            }
            else
            {
                backgroundImage.color = Color.red; // Warna Merah untuk Kalah
            }

            // Memastikan UI diperbarui
            LayoutRebuilder.ForceRebuildLayoutImmediate(BubbleChat.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.GetComponent<RectTransform>());
        }
    }
}
