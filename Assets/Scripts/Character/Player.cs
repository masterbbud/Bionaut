using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Experimental;

/*
 * The main Player script
 */
public class Player : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    public SpriteRenderer SpriteRenderer => m_SpriteRenderer;

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

    public static GameObject main;
    public static PlayerInventory inventory;

    [SerializeField]
    private ItemData rifleData, netData, knifeData;

    public Tool currentTool;

    public GameObject toolbelt;
    public Vector3 facingDirection = Vector3.zero;

    private bool freezeMovement;

    // Start is called before the first frame update
    void Awake()
    {
        if (main != null) {
            // We don't want to initialize the player if they already exist
            // Instead, move the player to this position and kill this instance
            main.transform.position = transform.position;
            DestroyImmediate(gameObject);
            return;
        }

        // Otherwise, we set this to the canon "player" and set it to not destroy on scene load
        // This makes it way easier to test
        main = gameObject;
        DontDestroyOnLoad(gameObject);
        inventory = transform.Find("Inventory").GetComponent<PlayerInventory>();

        // We want the player to be inactive on the planet map scene and the start scene
        SceneManager.sceneLoaded += SetPlayerActiveByScene;
    }

    void SetPlayerActiveByScene(Scene scene, LoadSceneMode mode)
    {
        // Player should be inactive on the planet map scene and start scene
        if (scene.name == "PlanetMapScene" || scene.name == "StartScene") {
            main.SetActive(false);
        }
        // Player should be active on all other scenes
        else {
            main.SetActive(true);

            // Handle first time on Silva
            if (scene.name == "Silva" && inventory.collectedCritters.Count == 0) {
                StartCoroutine(TellPlayerToCatchBlob());
            }
        }
    }
    
    void Start()
    {
        //sprite render for tree check
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        // For debugging purposes, give the player the rifle and net
        inventory.GiveObject(rifleData, 1);
        inventory.GiveObject(netData, 1);
        inventory.GiveObject(knifeData, 1);

        // Select empty hands to start
        SelectTool(3);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement logic
        if (!freezeMovement) {
            Vector2 heading = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = heading * moveSpeed;
        }
        else {
            rb.velocity = Vector2.zero;
        }


        if (!ToolBeltBehavior.showing) { // Don't want to use item or interact if ui is shown
            // Use current item
            if (Input.GetMouseButtonDown(0))
            {
                currentTool.Use();
            }
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

        // We want to use the facing direction based on the player moving direction

        if (rb.velocity != Vector2.zero) {
            double angle = Math.Atan2(rb.velocity.y, rb.velocity.x);
            angle /= Math.PI / 2;
            if (angle == 1.5 || angle == -0.5) {
                angle += 0.5;
                // The animator treats direction slightly differently, so we have to do this to prioritize
                // the sideways angles
            }
            angle = Math.Floor(angle);
            angle *= Math.PI / 2;
            facingDirection = new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
            
            // TODO this has imperfect behavior when traveling against a wall
        }
        
        // Determine the facing direction based on the mouse position
        // We commented out these scripts because it generally looks bad to have the player look
        // this way
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        // float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // // Set the animator parameters based on the direction to the mouse
        // if (angle >= -45f && angle <= 45f)
        // {
        //     // Facing right
        //     animator.SetFloat("Horizontal", 1f);
        //     animator.SetFloat("Vertical", 0f);
        // }
        // else if (angle > 45f && angle < 135f)
        // {
        //     // Facing up (backwards)
        //     animator.SetFloat("Horizontal", 0f);
        //     animator.SetFloat("Vertical", 1f);
        // }
        // else if (angle >= 135f || angle <= -135f)
        // {
        //     // Facing left
        //     animator.SetFloat("Horizontal", -1f);
        //     animator.SetFloat("Vertical", 0f);
        // }
        // else if (angle < -45f && angle > -135f)
        // {
        //     // Facing down (forwards)
        //     animator.SetFloat("Horizontal", 0f);
        //     animator.SetFloat("Vertical", -1f);
        // }
    }
    
    // Selects the Nth tool in the "toolbelt"
    public void SelectTool(int toolIndex)
    {
        if (currentTool) {
            currentTool.gameObject.SetActive(false);
        }
        Tool nextTool = toolbelt.transform.GetChild(toolIndex).GetComponent<Tool>();
        Debug.Log(nextTool);
        if (inventory.HasTool(nextTool.itemData)) {
            currentTool = nextTool;
            currentTool.gameObject.SetActive(true);
        }
    }

    public void AnimateUseKnife(bool useKnife)
    {
        animator.SetTrigger("UseKnife");
        freezeMovement = true;
    }

    public void UnlockMovement()
    {
        freezeMovement = false;
    }

    IEnumerator TellPlayerToCatchBlob() {
        DialogBoxBehavior.ShowBanner("Aw, look, that critter is so cute! I should use my net to catch it so I can bring it along with me on my journey. (Hold Spacebar to select your net)", 8);
        yield return new WaitForSeconds(20);

        // Keep telling the player until they catch the critter
        if (inventory.collectedCritters.Count == 0) {
            StartCoroutine(TellPlayerToCatchBlob());
        }
    }
}
