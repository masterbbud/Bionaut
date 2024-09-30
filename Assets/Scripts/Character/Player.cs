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

    private List<Collectible> collectibles = new List<Collectible>();

    public static GameObject main;
    // Start is called before the first frame update
    void Start()
    {
        main = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement logic
        Vector2 heading = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = heading * moveSpeed;

        // Shooting logic
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // interactions
        if (Input.GetMouseButtonDown(1)) {
            TryInteract();
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Update walking animation
        if (rb.velocity != Vector2.zero)
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("Horizontal", rb.velocity.x);
            animator.SetFloat("Vertical", rb.velocity.y);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        // Determine the facing direction based on the mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        
        // Set the animator parameters based on the direction to the mouse
        //if (angle >= -45f && angle <= 45f)
        //{
        //    // Facing right
        //    animator.SetFloat("Horizontal", 1f);
        //    animator.SetFloat("Vertical", 0f);
        //}
        //else if (angle > 45f && angle < 135f)
        //{
        //    // Facing up (backwards)
        //    animator.SetFloat("Horizontal", 0f);
        //    animator.SetFloat("Vertical", 1f);
        //}
        //else if (angle >= 135f || angle <= -135f)
        //{
        //    // Facing left
        //    animator.SetFloat("Horizontal", -1f);
        //    animator.SetFloat("Vertical", 0f);
        //}
        //else if (angle < -45f && angle > -135f)
        //{
        //    // Facing down (forwards)
        //    animator.SetFloat("Horizontal", 0f);
        //    animator.SetFloat("Vertical", -1f);
        //}
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

    void TryInteract()
    {
        if (InteractibleObject.interactions.Count == 0) {
            return;
        }
        InteractibleObject closest = null;
        float bestDist = 1000;
        foreach (InteractibleObject obj in InteractibleObject.interactions) {
            float dist = Vector2.Distance(obj.gameObject.transform.position, transform.position);
            if (dist < bestDist) {
                bestDist = dist;
                closest = obj;
            }
        }
        if (closest == null) { // shouldn't happen, but just in case
            return;
        }
        closest.Interact();
    }
    
    public void GiveObject(Collectible obj)
    {
        Collectible existingObj = null;
        foreach (Collectible coll in collectibles) {
            if (coll.GetType() == obj.GetType()) {
                existingObj = coll;
                break;
            }
        }
        if (existingObj == null) {
            collectibles.Add(obj);
        }
        else {
            existingObj.quantity += obj.quantity;
        }
    }
}
