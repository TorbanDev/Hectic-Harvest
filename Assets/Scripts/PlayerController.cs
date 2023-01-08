using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private PlayerInventory inventory;
    [SerializeField] PlayerInteract interact;
    public float ActionTime = 1.5f;
    [SerializeField]
    public Transform playerCenter;

    public float throwForce = 2f;

    private Vector2 movement_direction;
    public Vector2 interactionDirection;
    private Rigidbody2D player_rigidbody;
    private Vector2 nullDirection = new Vector2(0f, 0f);
    Player_State state = Player_State.ACTIVE;

    // Start is called before the first frame update
    void Awake()
    {
        player_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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
            interactionDirection = movement_direction;
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
        if (state == Player_State.ACTIVE)
        {
            player_rigidbody.MovePosition(player_rigidbody.position + movement_direction * speed * Time.fixedDeltaTime);
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
