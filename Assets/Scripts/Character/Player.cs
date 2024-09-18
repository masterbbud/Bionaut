using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [SerializeField]
    private GameObject bulletFab;

    [SerializeField]
    private float bulletSpeed;
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 velocity;
    public Animator animator;
    [SerializeField]
    private GameObject podObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 heading = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = heading * moveSpeed;

        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }

        UpdateAnimation();
    }


    void UpdateAnimation()
    {

        if(rb.velocity != Vector2.zero)
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("Horizontal", rb.velocity.x);
            animator.SetFloat("Vertical", rb.velocity.y);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }


    void Shoot()
    {
        GameObject fab = Instantiate(bulletFab);
        fab.GetComponent<Projectile>().player = this;
        fab.transform.position = transform.position;
        Vector2 towards = (MouseCursor.GetPosition() - (Vector2)transform.position).normalized;
        fab.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(towards.y, towards.x) * Mathf.Rad2Deg);
        fab.GetComponent<Rigidbody2D>().velocity = towards * bulletSpeed;
    }


}
