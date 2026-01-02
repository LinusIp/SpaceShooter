using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Laser : MonoBehaviour
{
    private float _speed = 8.0f;
    private float _ramSpeed = 6.0f;
    private Enemy _enemy;

    private bool _isEnemyLaser = false;
    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponent<Enemy>();
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectEnemy();
        if(_isEnemyLaser  == false)
        {
            HomingProjectile();
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    public void MoveUp()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);

        if (transform.position.y > 10.7f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);

        }
    }

    private void DetectEnemy()
    {
        if (_enemy == null)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, _enemy.transform.position);

    }

    private void HomingProjectile()
    {
        if (_enemy == null)
        {
            return;
        }
        Vector2 direction = (_enemy.transform.position - transform.position).normalized;

        transform.position += (Vector3)(direction * _ramSpeed * Time.deltaTime);

    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);

        if (transform.position.y < -10.7f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);

        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }
    public void AssigEnemyBAckLaser()
    {
        _isEnemyLaser = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.DamageAnim();
                player.Damage();
            }
        }
        if(other.tag == "PowerUp" && _isEnemyLaser == true)
        {
            PowerUp powerup = other.GetComponent<PowerUp>();
            powerup.Damage();
        }
    }
}
