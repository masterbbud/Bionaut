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

    public static GameObject main;
    public static PlayerInventory inventory;

    [SerializeField]
    private ItemData rifleData;

    public Tool currentTool;

    public GameObject toolbelt;
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
        if (scene.name == "PlanetMapScene" || scene.name == "StartScene") {
            main.SetActive(false);
            Debug.Log(inventory);
            Debug.Log(inventory.HasTool(rifleData));
        }
        else {
            main.SetActive(true);
            Debug.Log(inventory);
            Debug.Log(inventory.HasTool(rifleData));
        }
    }
    
    void Start()
    {
        inventory.GiveObject(rifleData, 1);
        SelectTool(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement logic
        Vector2 heading = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = heading * moveSpeed;

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

        // Determine the facing direction based on the mouse position
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
        if (inventory.HasTool(nextTool.itemData)) {
            currentTool = nextTool;
            currentTool.gameObject.SetActive(true);
        }
    }
}
