using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class BuildingSystemManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public LayerMask buildSurfaceMask;
    public Material validMaterial;
    public Material invalidMaterial;
    public GameObject buildMode;

    public PlayerBuildingHandler playerBuildHandler;

    [Header("Settings")]
    public float maxBuildDistance = 10f;
    public bool snapToGround = true;
    public bool alignToSurfaceNormal = true;

    [Header("Placement Controls")]
    public float rotationSpeed = 90f;   // degrees per second for scroll
    public float snapRotationAmount = 45f; // degrees added when pressing R
    private float previewYaw = 0f;

    [Header("Grid Controls")]
    public float gridSize = 1f;
    public bool useGridSnap = true;

    [Header("Destroy Settings")]
    public Key destroyKey = Key.Delete;
    public LayerMask buildableLayer; // Layer for placed buildables
    public Material destroyHighlightMaterial;


    [Header("Resource System")]
    public PlayerInventory playerInventory;
    public AudioClip notEnoughResourcesSound;




    private GameObject currentPreviewObject;
    private Renderer[] previewRenderers;
    private bool isValidPlacement = false;
    private BuildableObject currentBuildableData;
    private GameObject highlightedObject;
    private Material[] originalMaterials;
    private bool canHighlight = true;



    private void Start()
    {
        buildMode.SetActive(false);
        playerBuildHandler = GetComponent<PlayerBuildingHandler>();
    }

    private void Update()
    {
        if (currentPreviewObject == null) return;

        HandlePreviewPosition();
        HandleRotationInput();    // new
        HandleToggleOptions();    // new
        HandleBuildInput();
        HandleDestroyInput();
        if (canHighlight)
        {
            HandleDestroyHighlight();
        }
        


        
    }

    

    // Called to start building a new object
    public void StartBuildMode(BuildableObject buildable)
    {   

        


        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        currentBuildableData = buildable;
        currentPreviewObject = Instantiate(buildable.previewPrefab);
        previewRenderers = currentPreviewObject.GetComponentsInChildren<Renderer>();
    }

    private void HandlePreviewPosition()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxBuildDistance, Color.yellow);
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, maxBuildDistance, buildSurfaceMask))
        {
            Vector3 placementPos = hit.point;

            if (snapToGround)
            placementPos.y = hit.point.y;

            // grid snapping (if enabled)
            if (useGridSnap)
            {
            placementPos.x = Mathf.Round(placementPos.x / gridSize) * gridSize;
            placementPos.z = Mathf.Round(placementPos.z / gridSize) * gridSize;
            }

            currentPreviewObject.transform.position = placementPos + currentPreviewObject.transform.TransformDirection(currentBuildableData.placementOffset);


            // Compute base rotation aligned to surface normal:
            Quaternion baseRotation;
            if (alignToSurfaceNormal)
            {
            Vector3 up = hit.normal;

            // Compute a forward vector for the preview that is projected onto the surface plane.
            Vector3 camForward = playerCamera.transform.forward;
            Vector3 projectedForward = Vector3.ProjectOnPlane(camForward, up);
            if (projectedForward.sqrMagnitude < 0.0001f)
            {
                projectedForward = Vector3.forward;
            }

            baseRotation = Quaternion.LookRotation(projectedForward.normalized, up);
        }
        else
        {
            // No alignment to surface normal: face camera forward direction (flat)
            Vector3 flatForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
            if (flatForward.sqrMagnitude < 0.0001f) flatForward = Vector3.forward;
            baseRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        }

        // Apply yaw rotation on top of base rotation
        Quaternion finalRotation = baseRotation * Quaternion.Euler(0f, previewYaw, 0f);
        currentPreviewObject.transform.rotation = finalRotation;

        // Validation check
        isValidPlacement = ValidatePlacement(hit);
        UpdatePreviewMaterial(isValidPlacement);
    }
    else
    {
        isValidPlacement = false;
        UpdatePreviewMaterial(false);
    }


    }

    private bool ValidatePlacement(RaycastHit hit)
    {
        Collider[] ownColliders = currentPreviewObject.GetComponentsInChildren<Collider>();
        Bounds combinedBounds = new Bounds(currentPreviewObject.transform.position, Vector3.zero);

        // Combine all child colliders’ bounds
        foreach (var col in ownColliders)
        {
        combinedBounds.Encapsulate(col.bounds);
        }

        Collider[] overlaps = Physics.OverlapBox(
        combinedBounds.center,
        combinedBounds.extents,
        currentPreviewObject.transform.rotation,
        ~0, // check against everything
        QueryTriggerInteraction.Ignore
        );

        foreach (Collider col in overlaps)
        {
        // ignore the preview’s own colliders
        if (System.Array.Exists(ownColliders, c => c == col))
            continue;

        // ignore the ground surface
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            continue;

        return false;
        }

        return true;
    }


    private void UpdatePreviewMaterial(bool valid)
    {
        if (previewRenderers == null) return;

        Material targetMat = valid ? validMaterial : invalidMaterial;

        foreach (var rend in previewRenderers)
        {
            rend.sharedMaterial = targetMat;
        }
    }

    private void HandleBuildInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && isValidPlacement)
        {
            ConfirmBuild();
            
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelBuildMode();
        }
    }

    private void ConfirmBuild()
    {

        playerInventory.SpentMoney(currentBuildableData);
        
            if (playerInventory == null)
            {
                Debug.LogWarning("PlayerInventory not assigned.");
                return;
            }

            if (playerInventory.money <= currentBuildableData.turretCost)
            {
                Debug.Log("Not enough resources to build!");
                return; // cancel build
            }
            else if (playerInventory.money >= currentBuildableData.turretCost)
            {
                Debug.Log("Resources consumed for build.");
            }
        

        

        GameObject builtObject = Instantiate(
        currentBuildableData.finalPrefab,
        currentPreviewObject.transform.position,
        currentPreviewObject.transform.rotation
        );

        

        // Assign buildable layer
        builtObject.layer = LayerMask.NameToLayer("Buildable");
        foreach (Transform child in builtObject.GetComponentsInChildren<Transform>())
            child.gameObject.layer = LayerMask.NameToLayer("Buildable");

        Debug.Log($"Built: {currentBuildableData.name}");


        // Clear any highlight effect
        ClearHighlight();

        // Prevent highlight or preview refresh for one frame
        StartCoroutine(ResetBuildCooldown());

        // Don’t cancel build mode — only remove the preview
        if (currentPreviewObject != null)
            Destroy(currentPreviewObject);

        currentPreviewObject = null;
        currentBuildableData = null;
        isValidPlacement = false;
    }



    public void CancelBuildMode()
    {


        if (currentPreviewObject != null)
            Destroy(currentPreviewObject);

        currentPreviewObject = null;
        currentBuildableData = null;
        isValidPlacement = false;
    }

    private void HandleRotationInput()
    {

        // R key rotates by a fixed snap amount (instant)
        if (Input.GetKeyDown(KeyCode.R))
        {
        previewYaw += snapRotationAmount;
        }

        // keep yaw normalized to avoid overflow
        if (previewYaw > 360f || previewYaw < -360f)
        previewYaw = Mathf.Repeat(previewYaw, 360f);
    }


    private void HandleToggleOptions()
    {
        // Toggle grid snap
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
        useGridSnap = !useGridSnap;
        Debug.Log($"Grid Snap: {useGridSnap}");
        }

        // Toggle surface normal alignment
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
        alignToSurfaceNormal = !alignToSurfaceNormal;
        Debug.Log($"Align to Surface: {alignToSurfaceNormal}");
        }
    }

    private void HandleDestroyInput()
    {
        if (Keyboard.current[destroyKey].wasPressedThisFrame)
        {
        TryDestroyTarget();
        }
    }

    private void TryDestroyTarget()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxBuildDistance, buildableLayer))
        {
            GameObject target = hit.collider.gameObject;

            if (highlightedObject == target)
            {
                ClearHighlight();
            }

                // Refund logic: only if this object has a BuildableObject reference
            
            if (currentBuildableData != null && playerInventory != null)
            {
                foreach (var cost in currentBuildableData.buildCost)
                {
                    int refundAmount = Mathf.RoundToInt(cost.amount * currentBuildableData.refundRate);
                    if (refundAmount <= 0) continue;
                        playerInventory.AddResource(cost.resource, refundAmount);
                    
                }

                Debug.Log($"Refunded {currentBuildableData.refundRate * 100}% of {currentBuildableData.objectName}");
            }

            

            Destroy(target);
        }
    }


    private void HandleDestroyHighlight()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxBuildDistance, buildableLayer))
        {
            GameObject target = hit.collider.gameObject;

            // If this is a new target, clear previous highlight first
            if (highlightedObject != target)
            {
                ClearHighlight();
                highlightedObject = target;

                Renderer[] renderers = highlightedObject.GetComponentsInChildren<Renderer>();
                originalMaterials = new Material[renderers.Length];

                for (int i = 0; i < renderers.Length; i++)
                {
                // Store current runtime material (not sharedMaterial)
                originalMaterials[i] = renderers[i].material;
                renderers[i].material = destroyHighlightMaterial;
                }
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void ClearHighlight()
    {
        if (highlightedObject == null || originalMaterials == null) return;

        Renderer[] renderers = highlightedObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length && i < originalMaterials.Length; i++)
        {
            // Restore runtime material safely
            renderers[i].material = originalMaterials[i];
        }

        highlightedObject = null;
        originalMaterials = null;
    }

    private System.Collections.IEnumerator ResetBuildCooldown()
    {
        // Temporarily disable highlight logic
        canHighlight = false;
        yield return null; // wait one frame
        canHighlight = true;
    }

   


}
