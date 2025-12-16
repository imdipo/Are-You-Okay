//using UnityEditor.Build.Content;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] respawnPoint;
    [SerializeField] private float cooldown = 2f; // awalnya cuman spawn 1 enemy/2 detik

    [SerializeField] private float colldownDecreaseRate = .05f; // penurunan 0.05 per
    [SerializeField] private float cooldownCap = .7f; // cooldown cannot be lower than
    private float timer;    

    private Transform player;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = cooldown;
            SpawnEnemy();

            cooldown = Mathf.Max(cooldownCap, cooldown - colldownDecreaseRate); // setiap pembuatan skeleton kita reduce 0,5 detik  
        }
    }


    public void SpawnEnemy()
    {
        Vector3 posisiAcuan;

        int respawnPointIndex = Random.Range(0, respawnPoint.Length);
        Vector3 spawnPoint = respawnPoint[respawnPointIndex].position; // so when we type transform.position ini akan mengembalikan vector3 yang equal dengan transform.position

        GameObject MusuhBaru = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);

        if (player != null)
        {
            posisiAcuan = player.transform.position;
        } else
        {
            posisiAcuan = Player.posisiTerakhir;
            this.enabled = false;
            return;
        }

        // cek musuh di spawn di sebelah kiri/kana
        bool dibuatDiKanan = MusuhBaru.transform.position.x > posisiAcuan.x;  // true, jika di sebelah kanan

        if (dibuatDiKanan)
        {

            MusuhBaru.GetComponent<Enemy>().balikBadan();
        }

    }
}
