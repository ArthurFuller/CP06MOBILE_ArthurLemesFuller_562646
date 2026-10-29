using UnityEngine;

[CreateAssetMenu(menuName = "Protocolo de Emergencia/Tipo de Painel")]
public sealed class PanelType : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Color color = Color.white;

    public string DisplayName => displayName;
    public Color Color => color;

}
