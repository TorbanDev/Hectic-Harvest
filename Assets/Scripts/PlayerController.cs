using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float speed;
    [SerializeField]
    public PlayerInventory inventory;
    [SerializeField] PlayerInteract interact;
    public float ActionTime = 1.5f;
    [SerializeField]
    public Transform playerCenter;
    public AudioSource audioSource;

    public AudioClip pickupClip;
    public AudioClip dropClip;
    public AudioClip cantDoThatclip;
    float lastX;

    public float throwForce = 2f;

    private Vector2 movement_direction;
    public Vector2 interactionDirection;
    private Rigidbody2D player_rigidbody;
    private Vector2 nullDirection = new Vector2(0f, 0f);
    Player_State state = Player_State.ACTIVE;

    Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        player_rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GM_STATE.PLAY)
        {

            movement_direction.x = Input.GetAxisRaw("Horizontal");
            movement_direction.y = Input.GetAxisRaw("Vertical");
            if(state==Player_State.ACTIVE)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ButtonPress(KeyCode.Space);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    interact.GrabOrUseItem();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    interact.ThrowItem();
                    ButtonPress(KeyCode.Mouse1);
                }
            }
            if(movement_direction!= nullDirection)
            {
                if(movement_direction==Vector2.up||movement_direction==Vector2.down)
                {
                    
                }
                else
                {
                    lastX = interactionDirection.x;
                }
                interactionDirection = movement_direction;
                
            }
        }
    }

    private void ButtonPress(KeyCode button)
    {

    }
    public bool IsStationary()
    {
        return movement_direction == nullDirection;
    }
    private void FixedUpdate()
    {
        if(GameManager.Instance.state==GM_STATE.PLAY)
        {
            if (state == Player_State.ACTIVE)
            {
                Vector2 move = player_rigidbody.position + movement_direction * speed * Time.deltaTime;
                player_rigidbody.MovePosition(move);
                if (!IsStationary())
                {

                    animator.SetTrigger("Move");
                }
                else
                {
                    animator.ResetTrigger("Move");
                }
                animator.SetFloat("xDir", interactionDirection.x==0 ? lastX : interactionDirection.x);
            }

        }
    }

    public void SetState(Player_State newState)
    {
        state = newState;
    }
}
public enum Player_State
{
    ACTIVE=0,
    INACTVE=1
}
