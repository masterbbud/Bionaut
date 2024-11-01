using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class SkittCrit_NM : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;  // overall spriteRenderer

    [SerializeField]
    Sprite spriteFleeing;  // fleeing sprite

    [SerializeField]
    Sprite spriteSitting;  // sitting sprite

    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected float dis;   //for seeking player

    NavMeshAgent agent;



    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = spriteSitting;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (Math.Abs(agent.velocity.x) > 0.1f)  // Prevent sprite jitter
        {
            if (agent.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }



        if (Vector2.Distance(Player.main.transform.position, transform.position) < dis)
        {
            spriteRenderer.sprite = spriteFleeing;

            agent.speed = 12.0f;

            Vector3 runTo = (transform.position + (transform.position - Player.main.transform.position)) * 1;
            //float distance = Vector3.Distance(transform.position, Player.main.transform.position);
            
            agent.SetDestination(runTo);
            

            //Vector2 flee = (Player.main.transform.position - transform.position).normalized * 12;

            //agent.SetDestination(flee);

        }
        else
        {
            //agent.speed = 0;

            if (agent.velocity.x == 0) {
                spriteRenderer.sprite = spriteSitting;
            }
        }



    }


   




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (Vector2.Distance(Player.main.transform.position, transform.position) < dis)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
    }




}