using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float interactionRange = 4f;

    private WorldGenerator worldGenerator;
    private InventorySystem inventorySystem;
    private int selectedItemIndex = 0;
    private float verticalVelocity;
    private Camera playerCamera;
    private bool isGrounded;

    public void Initialize(WorldGenerator worldGeneratorRef, InventorySystem inventory)
    {
        worldGenerator = worldGeneratorRef;
        inventorySystem = inventory;

        var cameraGO = new GameObject("PlayerCamera");
        cameraGO.transform.SetParent(transform);
        cameraGO.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        playerCamera = cameraGO.AddComponent<Camera>();
        playerCamera.fieldOfView = 72f;
        playerCamera.nearClipPlane = 0.1f;

        var body = GetComponent<Renderer>();
        if (body != null)
            body.material.color = new Color(0.14f, 0.48f, 0.9f);

        transform.localScale = new Vector3(1f, 1.4f, 1f);
    }

    private void Update()
    {
        if (worldGenerator == null)
            return;

        HandleMovement();
        HandleSelection();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition * 0.01f;
                transform.position += new Vector3(delta.x, 0f, delta.y);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        transform.position += new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

        if (transform.position.y <= 1.4f)
        {
            transform.position = new Vector3(transform.position.x, 1.4f, transform.position.z);
            verticalVelocity = 0f;
            isGrounded = true;
        }

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        }
    }

    private void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedItemIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedItemIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedItemIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedItemIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedItemIndex = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6)) selectedItemIndex = 5;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            TryPlaceOrBreakBlock();
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            TryInteractWithNPC();
        }
    }

    private void TryPlaceOrBreakBlock()
    {
        var item = inventorySystem.GetCurrentItem(selectedItemIndex);
        if (item == null)
            return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
        if (Physics.Raycast(ray, out var hit, interactionRange))
        {
            if (hit.collider.CompareTag("Block"))
            {
                worldGenerator.RemoveBlockAt(hit.collider.transform.position);
                GameLauncher.Instance.UpdateStatus($"Removed {item.DisplayName} block.");
                return;
            }

            if (item.Placeable)
            {
                Vector3 targetPos = hit.point + hit.normal * 0.6f;
                Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));

                if (worldGenerator.TryPlaceBlock(gridPos, item.Type))
                    GameLauncher.Instance.UpdateStatus($"Placed {item.DisplayName} at {gridPos}.");
                else
                    GameLauncher.Instance.UpdateStatus("That block spot is already occupied.");
            }
            else
            {
                GameLauncher.Instance.UpdateStatus($"{item.DisplayName} cannot be placed as a block.");
            }
        }
    }

    private void TryInteractWithNPC()
    {
        var npcs = FindObjectsOfType<NPCController>();
        foreach (var npc in npcs)
        {
            if (npc != null && Vector3.Distance(transform.position, npc.transform.position) <= 3f)
            {
                npc.Interact();
                return;
            }
        }

        GameLauncher.Instance.UpdateStatus("No NPC nearby. Move closer to talk.");
    }
}
