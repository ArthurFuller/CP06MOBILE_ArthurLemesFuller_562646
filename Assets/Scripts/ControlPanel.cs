using UnityEngine;

[RequireComponent(typeof(Renderer))]
public sealed class ControlPanel : MonoBehaviour
{
    [SerializeField] private PanelType type;
    [SerializeField] private Renderer indicator;
    [SerializeField, Range(0f, 1f)] private float focusBrightness = 0.55f;
    [SerializeField, Min(0f)] private float focusPulseSpeed = 5f;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock properties;
    private bool focused;

    public PanelType Type => type;
    public bool IsActivated { get; private set; }

    private void Awake() => properties = new MaterialPropertyBlock();
    private void Start() => ResetPanel();

    private void Update()
    {
        if (focused) UpdateAppearance();
    }

    public void ResetPanel()
    {
        IsActivated = false;
        focused = false;
        UpdateAppearance();
    }

    public void Activate()
    {
        IsActivated = true;
        focused = false;
        UpdateAppearance();
    }

    public void SetFocused(bool value)
    {
        focused = value && !IsActivated;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (indicator == null || type == null) return;
        Color color = IsActivated ? type.Color * 0.2f : type.Color;
        if (focused)
        {
            float wave = 0.5f + 0.5f * Mathf.Sin(Time.time * focusPulseSpeed);
            color = Color.Lerp(color, Color.white, wave * focusBrightness);
        }
        properties.SetColor(BaseColorId, color);
        indicator.SetPropertyBlock(properties);
    }

}
