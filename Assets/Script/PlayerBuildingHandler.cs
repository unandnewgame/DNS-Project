using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildingHandler : MonoBehaviour
{
    [Header("References")]
    public BuildingSystemManager buildingSystem;
    public Camera playerCamera;
    

    [Header("Buildable Selection")]
    public BuildableDatabase buildableDatabase;
    private int currentIndex = 0;


    private bool isInBuildMode = false;

    private void Update()
    {
        HandleBuildModeToggle();
        if (isInBuildMode)
        {
                HandleBuildSelection();
        }

        

    }

    private void HandleBuildModeToggle()
    {
        // Press B to toggle build mode
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (!isInBuildMode)
                EnterBuildMode();
            else
                ExitBuildMode();
        }
    }

    private void EnterBuildMode()
    {
        buildingSystem.buildMode.SetActive(true);

        buildingSystem.StartBuildMode(buildableDatabase.buildables[0]);
        
 
        isInBuildMode = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Entered Build Mode: Tent");
    }

    private void ExitBuildMode()
    {
        buildingSystem.buildMode.SetActive(false);
        buildingSystem.CancelBuildMode();
        isInBuildMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Exited Build Mode");
    }

    private void HandleBuildSelection()
    {
        if (buildableDatabase == null || buildableDatabase.buildables.Length == 0) return;

        // Scroll through buildables
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0.1f)
        CycleBuildable(1);
        else if (scroll < -0.1f)
        CycleBuildable(-1);

        // Number keys 1–9 shortcut
        for (int i = 0; i < Mathf.Min(9, buildableDatabase.buildables.Length); i++)
        {
            if (Keyboard.current[(Key)(Key.Digit1 + i)].wasPressedThisFrame)
            {
                SetBuildableIndex(i);
            }
        }
    }

    private void CycleBuildable(int direction)
    {
        currentIndex += direction;
        if (currentIndex >= buildableDatabase.buildables.Length) currentIndex = 0;
        if (currentIndex < 0) currentIndex = buildableDatabase.buildables.Length - 1;
        UpdateCurrentBuildable();
    }

    private void SetBuildableIndex(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, buildableDatabase.buildables.Length - 1);
        UpdateCurrentBuildable();
    }

    private void UpdateCurrentBuildable()
    {
        BuildableObject buildable = buildableDatabase.buildables[currentIndex];
        buildingSystem.StartBuildMode(buildable);
        Debug.Log($"Selected Buildable: {buildable.objectName}");
    }

}
