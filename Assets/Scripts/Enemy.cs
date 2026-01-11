using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _enemyShieldVisualizer;
    [SerializeField] private float _detectionRange = 5f;
    [SerializeField] private float _ramSpeed = 5f;
    [SerializeField] private GameObject _boss;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;

    // Movement variables
    private float _frequency = 1.0f;
    private float _amplitude = 2.0f;
    private Vector3 _startPos;

    private bool _isShieldActive = false;
    private bool _isRamming = false;
    private bool _backFire = false;
    private bool _isDead = false;

    void Start()
    {
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _startPos = transform.position;

        if (_player == null) Debug.LogError("Player is NULL on Enemy.");
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (_isDead) return;

        DetectPlayerPosition();
        DetectPowerUpNear();

        if (_isRamming)
        {
            RamPlayer();
        }
        else
        {
            CalculateMovement();
        }

        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        // Move downward
        _startPos += Vector3.down * _speed * Time.deltaTime;

        // Apply Sine Wave offset to the horizontal axis
        transform.position = _startPos + Vector3.right * Mathf.Sin(Time.time * _frequency) * _amplitude;

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            _startPos = new Vector3(randomX, 7, 0);
        }
    }

    private void FireLaser()
    {
        _fireRate = Random.Range(3.0f, 7.0f);
        _canFire = Time.time + _fireRate;

        Vector3 spawnPos = transform.position;
        if (_backFire) spawnPos += new Vector3(0, 1.5f, 0); // Spawn slightly above if firing back

        GameObject enemyLaser = Instantiate(_laserPrefab, spawnPos, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        foreach (var laser in lasers)
        {
            laser.AssignEnemyLaser();
            if (_backFire) laser.AssigEnemyBAckLaser();
        }
    }

    private void DetectPlayerPosition()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        // Logic for Ramming
        if (distance <= _detectionRange && _player.transform.position.y < transform.position.y)
        {
            _isRamming = true;
        }
        else
        {
            _isRamming = false;
        }

        // Logic for Backfire (Player is above enemy)
        _backFire = (_player.transform.position.y > transform.position.y);
    }

    private void DetectPowerUpNear()
    {
        // Using Physics to find powerups in a small radius in front of the enemy
        Collider2D hit = Physics2D.OverlapCircle(transform.position + Vector3.down, 2.0f);
        if (hit != null && hit.CompareTag("PowerUp"))
        {
            if (Time.time > _canFire) FireLaser(); // Only fire if cooldown is ready
        }
    }

    private void RamPlayer()
    {
        if (_player == null) return;

        Vector3 direction = (_player.transform.position - transform.position).normalized;
        transform.position += direction * _ramSpeed * Time.deltaTime;

        // Sync the base movement position so it doesn't "snap" back after ramming
        _startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isShieldActive)
            {
                DestroyShield();
            }
            else
            {
                _player.Damage();
                EnemyDeath();
            }
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_isShieldActive)
            {
                DestroyShield();
            }
            else
            {
                if (_player != null) _player.AddScore(10);
                EnemyDeath();
            }
        }
    }

    private void DestroyShield()
    {
        _isShieldActive = false;
        _enemyShieldVisualizer.SetActive(false);
    }

    private void EnemyDeath()
    {
        if (_isDead) return;
        _isDead = true;

        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _ramSpeed = 0;
        _audioSource.Play();

        // Disable collider so it doesn't hit the player while exploding
        GetComponent<Collider2D>().enabled = false;
        Destroy(this.gameObject, 2.5f);
    }

    public void EnemyShieldActive()
    {
        _isShieldActive = true;
        _enemyShieldVisualizer.SetActive(true);
    }
}