using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private GameMediator mediator;
    [SerializeField, Min(0.1f)] private float speed = 5f;
    [SerializeField, Min(0.1f)] private float interactionRadius = 1.4f;

    private CharacterController controller;
    private Vector3 startingPosition;
    private ControlPanel focusedPanel;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (!mediator.IsRunning || Keyboard.current == null)
        {
            SetFocusedPanel(null);
            return;
        }

        Keyboard keys = Keyboard.current;
        float x = (keys.dKey.isPressed || keys.rightArrowKey.isPressed ? 1f : 0f)
                - (keys.aKey.isPressed || keys.leftArrowKey.isPressed ? 1f : 0f);
        float z = (keys.wKey.isPressed || keys.upArrowKey.isPressed ? 1f : 0f)
                - (keys.sKey.isPressed || keys.downArrowKey.isPressed ? 1f : 0f);
        Vector3 direction = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);
        controller.Move(direction * (speed * Time.deltaTime));
        if (direction.sqrMagnitude > 0.01f) transform.forward = direction;

        SetFocusedPanel(FindClosestPanel());
        if (keys.eKey.wasPressedThisFrame && focusedPanel != null)
        {
            mediator.TryActivate(focusedPanel);
            SetFocusedPanel(null);
        }
    }

    private ControlPanel FindClosestPanel()
    {
        ControlPanel closest = null;
        float bestDistance = interactionRadius * interactionRadius;
        foreach (Collider hit in Physics.OverlapSphere(transform.position, interactionRadius))
        {
            ControlPanel panel = hit.GetComponentInParent<ControlPanel>();
            if (panel == null || panel.IsActivated) continue;
            float distance = (panel.transform.position - transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = panel;
            }
        }
        return closest;
    }

    private void SetFocusedPanel(ControlPanel panel)
    {
        if (focusedPanel == panel) return;
        if (focusedPanel != null) focusedPanel.SetFocused(false);
        focusedPanel = panel;
        if (focusedPanel != null) focusedPanel.SetFocused(true);
    }

    public void ResetPosition()
    {
        SetFocusedPanel(null);
        controller.enabled = false;
        transform.position = startingPosition;
        controller.enabled = true;
    }

    private void OnDisable() => SetFocusedPanel(null);
}
