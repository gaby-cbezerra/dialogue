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

        GameObject coin = Instantiate(
            coinPrefab,
            spawnPoints[index].position,
            Quaternion.identity
        );


        coin.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


        Debug.Log(
            "Coin criada em: " + coin.transform.position +
            " escala: " + coin.transform.localScale
        );
    }
}