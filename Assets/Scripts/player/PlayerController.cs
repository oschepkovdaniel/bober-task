using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Params")]
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float MouseSens;
    [SerializeField] private float InteractDistance;
    [SerializeField] private int MaxInventoryItems;
    [SerializeField] private float ItemDropForce;
    [SerializeField] private KeyCode[] InventoryKeys;

    [Header("Components")]
    [SerializeField] private CharacterController characterController;

    [Header("GameObjects")]
    [SerializeField] private GameObject Head;
    [SerializeField] private GameObject Hand;

    // state
    private PlayerState CurrentPlayerState;

    private IdleState idleState;
    private WalkState walkState;

    // flags
    private bool bWalking;
    private bool bHasInteract;

    // input
    private PlayerInput playerInput;

    // values
    private Vector3 velocity;

    // interact
    private RaycastHit interactHit;

    // inventory
    private InventoryItem[] inventory;
    private int currentInventoryItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeInventory();
        InitializeInput();
        InitializeStates();
        InitializePhysics();
        SetCurrentPlayerState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerInput();
        UpdateCurrentState();
        HoldCurrentItem();
    }

    // utils
    private void UpdateCurrentState()
    {
        if (CurrentPlayerState != null)
        {
            CurrentPlayerState.UpdateState();
        }
    }

    public void InitializeInventory()
    {
        inventory = new InventoryItem[MaxInventoryItems];
        for (int i = 0; i < MaxInventoryItems; i++)
        {
            inventory[i] = null;
        }
    }

    public void InitializeInput()
    {
        playerInput = new PlayerInput();

        playerInput.movement = Vector2.zero;
        playerInput.mouse = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void InitializePhysics()
    {
        velocity = Vector3.zero;
    }

    public void InitializeStates()
    {
        idleState = new IdleState(this);
        walkState = new WalkState(this);
    }

    public void SetCurrentPlayerState(PlayerState playerState)
    {
        CurrentPlayerState = playerState;
    }

    private void HandlePlayerInput()
    {
        playerInput.movement.x = Input.GetAxis("Horizontal");
        playerInput.movement.y = Input.GetAxis("Vertical");

        playerInput.mouse.x = Input.GetAxis("Mouse X");
        playerInput.mouse.y = Input.GetAxis("Mouse Y");

        playerInput.mouseWheel = Input.mouseScrollDelta;

        if (playerInput.mouseWheel.y > 0)
        {
            NextItem();
        }

        if (playerInput.mouseWheel.y < 0)
        {
            PrevItem();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DropItem();
        }

        if (Input.GetMouseButtonDown(0))
        {
            UseItem();
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (Input.GetKeyDown(InventoryKeys[i]))
            {
                if (inventory[i] != null) {
                    HideAllItems();

                    currentInventoryItem = i;
                    inventory[currentInventoryItem].gameObject.SetActive(true);
                    inventory[currentInventoryItem].PlayPickupSound();
                }
            }
        }
    }

    private void HoldCurrentItem()
    {
        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem)
            {
                GameObject item = inventoryItem.gameObject;
                item.gameObject.transform.position = Vector3.Lerp(item.transform.position, Hand.transform.position, 0.5f);
                item.gameObject.transform.rotation = Head.transform.rotation;
            }
        }
    }

    private void NextItem()
    {
        if (currentInventoryItem + 1 < inventory.Length)
        {
            currentInventoryItem += 1;
        }

        HideAllItems();
        
        if (inventory[currentInventoryItem])
        {
            inventory[currentInventoryItem].gameObject.SetActive(true);
            inventory[currentInventoryItem].PlayPickupSound();
        }

        Debug.Log(currentInventoryItem);
    }

    private void PrevItem()
    {
        if (currentInventoryItem - 1 >= 0)
        {
            currentInventoryItem -= 1;
        }

        HideAllItems();

        if (inventory[currentInventoryItem])
        {
            inventory[currentInventoryItem].gameObject.SetActive(true);
            inventory[currentInventoryItem].PlayPickupSound();
        }

        Debug.Log(currentInventoryItem);
    }

    public void TakeItem(InventoryItem target)
    {
        int index = GetAvailableIndex();
        if (index >= 0)
        {
            inventory[index] = target;

            GameObject item = target.gameObject;

            if (item.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
            }

            if (item.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }

            HideAllItems();

            currentInventoryItem = index;
            inventory[currentInventoryItem].gameObject.SetActive(true);

            target.PlayPickupSound();

            Debug.Log(index);
        }
    }

    public void DropItem()
    {
        if (inventory[currentInventoryItem])
        {
            InventoryItem currentItem = inventory[currentInventoryItem];
            GameObject item = currentItem.gameObject;

            if (item.TryGetComponent(out Collider collider))
            {
                collider.enabled = true;
            }

            if (item.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
                rigidbody.AddForce(Head.transform.forward * ItemDropForce);
            }

            inventory[currentInventoryItem] = null;
        }
    }

    private void UseItem()
    {
        if (inventory[currentInventoryItem])
        {
            if (inventory[currentInventoryItem].gameObject.TryGetComponent(out InteractObject interactObject))
            {
                interactObject.Interact();
            }
        }
    }

    private void HideAllItems()
    {
        foreach (InventoryItem item in inventory)
        {
            if (item)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private int GetAvailableIndex()
    {
        int index = -1;
        for (int i = 0; i < MaxInventoryItems; i++)
        {
            InventoryItem item = inventory[i];
            if (item == null)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    // control
    public void LookX(float value)
    {
        transform.Rotate(0, value * MouseSens, 0);
    }

    public void LookY(float value)
    {
        Head.transform.Rotate(-value * MouseSens, 0, 0);
    }

    public void Move(Vector3 direction, float value)
    {
        characterController.Move(direction * value * MovementSpeed * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime * Time.deltaTime;

        characterController.Move(velocity);
    }

    public void CastForwardRay()
    {
        Vector3 from = Head.transform.position;
        Vector3 to = Head.transform.TransformDirection(Vector3.forward);

        int layerMask = 1 << 3;

        Physics.Raycast(from, to, out interactHit, InteractDistance, layerMask);

        if (interactHit.transform)
        {
            GameObject target = interactHit.transform.gameObject;
            if (target.TryGetComponent(out InteractObject interactObject))
            {
                bHasInteract = true;
            }
            else
            {
                bHasInteract = false;
            }
        } else
        {
            bHasInteract = false;
        }
    }

    public void Interact()
    {
        if (interactHit.transform)
        {
            GameObject target = interactHit.transform.gameObject;
            if (target.TryGetComponent(out InventoryItem item))
            {
                TakeItem(item);
            }
            else if (target.TryGetComponent(out InteractObject interactObject))
            {
                interactObject.Interact();
            }
        }
    }

    // getters / setters
    public void SetWalking(bool bWalking)
    {
        this.bWalking = bWalking;
    }

    public bool GetWalking()
    {
        return bWalking;
    }

    public bool GetHasInteract()
    {
        return bHasInteract;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public PlayerState GetIdleState()
    {
        return idleState;
    }

    public PlayerState GetWalkState()
    {
        return walkState;
    }

    public InventoryItem GetInventoryItem(int index)
    {
        return inventory[index];
    }

    public InventoryItem[] GetInventory()
    {
        return inventory;
    }

    public int GetCurrentItem()
    {
        return currentInventoryItem;
    }

    public bool GetHasSpecificItem(string ItemName)
    {
        bool bHasItem = false;

        foreach (InventoryItem item in inventory)
        {
            if (item)
            {
                if (item.GetItemName().Equals(ItemName)) {
                    bHasItem = true;
                    break;
                }
            }
        }

        return bHasItem;
    }

    public string GetCurrentItemName()
    {
        if (inventory[currentInventoryItem])
        {
            return inventory[currentInventoryItem].GetItemName();
        }

        return "NONE";
    }
}
