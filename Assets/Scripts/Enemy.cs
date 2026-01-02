using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    private Player _player;
    private PowerUp _powerup;
    private Laser _laser;
    [SerializeField]
    private float _speed = 4.0f;
    private Animator _anim;
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;
    private float _frequency = 1.0f;
    private float _amplitude = 5.0f;
    private float _cycleSpeed = 1.0f;
    private Vector3 pos;
    private Vector3 axis;
    [SerializeField]
    private GameObject _enemyShieldVizualizer;
    private bool _isShieldActive = false;
    private bool _isEnemyHasShield = false;
    private float _detectionRange = 5f;
    private bool _isRamming = false;
    private float _ramSpeed = 6f;
    private bool _backFire = false;
    [SerializeField]
    private int _enemyID;

    
    // Start is called before the first frame update
    void Start()
    {

        pos = transform.position;
        axis = transform.right; 


        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _powerup = GameObject.Find("Spawn_Manager").GetComponent<PowerUp>();
        _laser = GameObject.Find("Laser").GetComponent<Laser>();

        _audioSource = GetComponent<AudioSource>();

        if( _player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        if(_enemyID == 1)
        {
            transform.position = new Vector3(0, 2, 0);
        }

        _anim = GetComponentInChildren<Animator>();


        if (_anim == null)
        {
            Debug.LogError("The Animation is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {

        DetectPlayer();
        DetectPlayerBehind();
        DetectPowerUp();
        DetectLaser();
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

    private void FireLaser()
    {

        if (_backFire == true)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 5, 0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
                lasers[i].AssigEnemyBAckLaser();
            }
        }
        else
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
                
        }
    }

    void CalculateMovement()
    {

        pos += Vector3.down * Time.deltaTime * _cycleSpeed;
        transform.position = pos + axis * Mathf.Sin(Time.time * _frequency) * _amplitude;

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && _isEnemyHasShield == false)
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.DamageAnim();
                player.Damage();
            }

            EnemyDeath();
        }
        else if(other.tag == "Player" && _isEnemyHasShield == true)
        {
            Player player = other.transform.GetComponent<Player>();

            if(player != null)
            {
                player.DamageAnim();
                player.Damage();
                _enemyShieldVizualizer.SetActive(false);
                _isEnemyHasShield = false;
            }
        }
        else if (other.tag == "Laser" && _isEnemyHasShield == false)
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            EnemyDeath();
        }
        else if(other.tag == "Laser" && _isEnemyHasShield == true)
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _isEnemyHasShield = false;
                _enemyShieldVizualizer.SetActive(false);
                _player.AddScore(0);
            }
        }
    }

    private void DetectPlayer()
    {
        if(_player  == null)
        {
            return;
        }
        float distance = Vector2.Distance( transform.position, _player.transform.position);
        if(distance <= _detectionRange && _player.transform.position.y < transform.position.y )
        {
            _isRamming = true;
        }
        

    }

    private void DetectPowerUp()
    {
        if(_powerup == null)
        {
            return;
        }
        float distance_powerup = Vector2.Distance(transform.position, _powerup.transform.position);

        if(distance_powerup <= 2 && _powerup.transform.position.y < transform.position.y)
        {
            FireLaser();
        }
    }

    private void DetectLaser()
    {
        if(_powerup == null)
        {
            return;
        }
        float distance_laser = Vector2.Distance(transform.position, _laser.transform.position);

        if(distance_laser <= 4)
        {
            transform.position = transform.position + new Vector3(4, 0, 0);
        }

    }

    private void DetectPlayerBehind()
    {
        if (_player == null)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, _player.transform.position);
        if (transform.position.y < _player.transform.position.y)
        {
            _backFire = true;
        }
    }

    private void RamPlayer()
    {
        if(_player == null)
        {
            return;
        }
        Vector2 direction = (_player.transform.position - transform.position).normalized;

        transform.position += (Vector3)(direction * _ramSpeed * Time.deltaTime);

    }

    private void EnemyDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        Destroy(this.gameObject, 2.3f);
        _isRamming = false;
    }

    public void EnemyShieldActive()
    {
        _isShieldActive = true;
        _enemyShieldVizualizer.SetActive(true);

    }
}
