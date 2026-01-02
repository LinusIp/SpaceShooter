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
    private bool _stopSpawning = false;
    private int _firstWave = 10;
    private int _incrementWaveBy = 10;
    private int _currentWave = 1;
    private int _skip;
    private GameObject _enemy;

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
            int enemiesInThisWave = _firstWave + (_incrementWaveBy * (_currentWave - 1));
            for(int i = 0; i < enemiesInThisWave; i++)
            {
                if(i % 5 == 0 && i != 0)
                {
                    EnemySpawn();
                    Enemy enemy = _enemy.transform.GetComponent<Enemy>();
                    enemy.EnemyShieldActive();
                }
                yield return new WaitForSeconds(2.0f);
                EnemySpawn();
            }
            yield return new WaitUntil(() =>
            _stopSpawning || _enemyContainer != null && _enemyContainer.transform.childCount == 0);

            yield return new WaitForSeconds(3.0f);

            _currentWave++;
        }
    }

    private void EnemySpawn()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
        _enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
        _enemy.transform.parent = _enemyContainer.transform;
    }

    private void BossSpawn()
    {
        Vector3 posToSpawn = new Vector3(0, 2, 0);
        _enemy = Instantiate(_bossPrefab, posToSpawn, Quaternion.identity);
        _enemy.transform.parent = _enemyContainer.transform;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
