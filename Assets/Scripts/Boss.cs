
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    private float _speed = 3.0f;
    private float _fireRate;
    private float _canFire;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8f, 8),transform.position.y, 0);
    }

    private void FireLaser()
    {
        _fireRate = Random.Range(3.0f, 7.0f);
        _canFire = Time.time + _fireRate;

        Vector3 spawnPos = transform.position + new Vector3(0, 1, 0);
        GameObject enemyLaser = Instantiate(_laserPrefab, spawnPos, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        foreach (var laser in lasers)
        {
            laser.AssignEnemyLaser();
        }
    }
}
