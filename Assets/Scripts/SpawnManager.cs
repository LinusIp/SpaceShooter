using System.Collections;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField] 
    private GameObject _powerUpContainer;
    [SerializeField]
    private GameObject _asteroidContainer;
    [SerializeField]
    private bool _stopSpawning = false;
    private int _firstWave = 10;
    private int _incrementWaveBy = 10;
    private int _currentWave = 1;
    private int _skip;
    private GameObject _enemy;
    private GameObject _asteroid;
    private bool _isBossSpawn = false;
    [SerializeField] private int _maxEnemyWaves = 3;
    [SerializeField] private int _maxWavesBeforeBoss = 3;
    private bool _bossSpawned = false;



    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }
    
    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
            int randomPowerUp = Random.Range(0, 5);
            if(randomPowerUp == 3)
            {
                _skip++;
                if(_skip % 2 == 0)
                {
                    Instantiate(_powerups[3], posToSpawn, Quaternion.identity);
                }
            }
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(20.0f);
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            //  AFTER 3 WAVES  SPAWN BOSS  STOP
            if (_currentWave > _maxWavesBeforeBoss)
            {
                if (!_bossSpawned)
                {
                    SpawnBoss();
                    _bossSpawned = true;
                }

                yield break; // stop spawning enemies forever
            }

            int enemiesInThisWave =
                _firstWave + (_incrementWaveBy * (_currentWave - 1));

            for (int i = 0; i < enemiesInThisWave; i++)
            {
                EnemySpawn();

                // Shielded enemies
                if (i % 5 == 0 && i != 0)
                {
                    Enemy enemy = _enemy.GetComponent<Enemy>();
                    enemy.EnemyShieldActive();
                }

                yield return new WaitForSeconds(2.0f);
            }

            // Wait until ALL enemies are dead
            yield return new WaitUntil(() =>
                _stopSpawning ||
                (_enemyContainer != null &&
                 _enemyContainer.transform.childCount == 0));

            yield return new WaitForSeconds(2.0f);
            _currentWave++;
        }
    }



    private void EnemySpawn()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
        _enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
        _enemy.transform.parent = _enemyContainer.transform;
    }

    private void SpawnBoss()
    {
        Vector3 spawnPos = new Vector3(0, 2, 0);
        GameObject boss = Instantiate(_bossPrefab, spawnPos, Quaternion.identity);
        boss.transform.parent = _enemyContainer.transform;
    }


    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
