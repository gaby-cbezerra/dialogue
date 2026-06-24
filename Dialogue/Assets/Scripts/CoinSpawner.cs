using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnTime = 5f;


    private void Awake()
    {
        Debug.Log("TESTE COIN");
        Debug.Log("COIN SPAWNER EXISTE");
    }


    private void Start()
    {
        Debug.Log("START DO SPAWNER");

        InvokeRepeating(
            nameof(SpawnCoin),
            2f,
            spawnTime
        );
    }


    private void SpawnCoin()
    {
        int index = Random.Range(0, spawnPoints.Length);

        Vector3 spawnPosition = spawnPoints[index].position;

        spawnPosition.y = 1f;


        GameObject coin = Instantiate(
            coinPrefab,
            spawnPosition,
            Quaternion.identity
        );


        coin.transform.localScale = Vector3.one * 0.5f;


        Debug.Log(
            "Coin criada em: " + coin.transform.position
        );
    }
}