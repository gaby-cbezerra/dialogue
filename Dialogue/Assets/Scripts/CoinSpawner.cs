using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnTime = 5f;
    
    private List<GameObject> spawnedCoins = new List<GameObject>();


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


        Vector3 pos = spawnPoints[index].position;


        // altura do chão
        pos.y = -0.8f;


        Instantiate(
            coinPrefab,
            pos,
            Quaternion.identity
        );


        Debug.Log("Moeda criada em: " + pos);
    }
}