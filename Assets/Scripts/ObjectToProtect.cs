using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectToProtect : Entity
{
    private Transform player;

    [SerializeField] private TextMeshPro interactionText;
    [SerializeField] private float naikTurun = 0.3f;
    [SerializeField] private float kecepatan = 1f;
    [SerializeField] private LayerMask PlayerLayer;
    private bool bisaNgobrol = false;
    public bool ApakahBisaNgobrol => bisaNgobrol; // read only for other script


    private Vector3 initialTextPosition;
    private Coroutine floatcoroutine;


    protected override void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<Player>().transform; // ambil object pertama dengan component of a player lalu kita ambil transformnya   

        if (interactionText != null) { 
        interactionText.gameObject.SetActive(false);
        initialTextPosition = interactionText.transform.localPosition;
        }
    }

    protected override void Update()
    {
        handleBalikBadan();
        cekRangeInteraction();
    }

    protected override void handleBalikBadan()
    {
        if (player == null)
            return; // kalau playernya mati

        if (player.transform.position.x > transform.position.x && hadapKanan == false) // posisi player > posisi objectToProtect
            balikBadan();
        else if (player.transform.position.x < transform.position.x && hadapKanan == true)
            balikBadan();
    }

    protected override void mati()
    {
        base.mati();
        UI.instance.EnableGameOverUI();
    }

    public void EffectMenang(bool tampilkan = true)
    {
        interactionText.gameObject.SetActive(true);

        if (tampilkan)
        {
            if (floatcoroutine != null) StopCoroutine(floatcoroutine);
            floatcoroutine = StartCoroutine(FloatingText());
        } else
        {
            // Jika teks dimatikan (saat chat dibuka), hentikan animasi mengambang
            if (floatcoroutine != null)
            {
                StopCoroutine(floatcoroutine);
                floatcoroutine = null;
            }
        }


        if (floatcoroutine != null) StopCoroutine(floatcoroutine);

        floatcoroutine = StartCoroutine(FloatingText());
            

    }

    private IEnumerator FloatingText()
    {
        while (true)
        {
            // naik
            float waktuAwal = Time.realtimeSinceStartup; // kita pake waktu sekarang, karena di ui kita matiin waktu gamenya 
            Vector3 textNaik = initialTextPosition + new Vector3(0, naikTurun);
            while (Time.realtimeSinceStartup < waktuAwal + (naikTurun / kecepatan))
            {
                interactionText.transform.localPosition = Vector3.Lerp(initialTextPosition, textNaik, (Time.realtimeSinceStartup - waktuAwal) / (naikTurun / kecepatan));
                yield return null; // tunggu 1 frame
            }
            interactionText.transform.localPosition = textNaik;

            waktuAwal = Time.realtimeSinceStartup;
            Vector3 textTurun = initialTextPosition;
            while (Time.realtimeSinceStartup < waktuAwal + (naikTurun / kecepatan))
            {
                interactionText.transform.localPosition = Vector3.Lerp(textNaik, textTurun, (Time.realtimeSinceStartup - waktuAwal) / (naikTurun/ kecepatan));
                yield return null;  
            }
            interactionText.transform.localPosition = textTurun;
        }

    }

    private void cekRangeInteraction()
    {
        Collider2D colliderPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, PlayerLayer);

        bisaNgobrol = colliderPlayer != null;
    }
}
