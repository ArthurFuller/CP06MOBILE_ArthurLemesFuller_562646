using UnityEngine;
using UnityEngine.UI;

public sealed class GameHUD : MonoBehaviour
{
    [Header("Partida")]
    [SerializeField] private GameFacade facade;
    [SerializeField] private GameMediator mediator;

    [Header("Indicadores")]
    [SerializeField] private Image timeFill;
    [SerializeField] private Text progressLabel;
    [SerializeField] private Image targetIndicator;
    [SerializeField] private Text statusLabel;

    [Header("Pulso do indicador")]
    [SerializeField, Min(0f)] private float pulseSpeed = 4f;
    [SerializeField, Range(0f, 0.3f)] private float pulseScale = 0.1f;

    [Header("Telas e botoes")]
    [SerializeField] private GameObject gameplayGroup;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text endMessage;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    private string progressCaption;
    private string introCaption;
    private RectTransform timeRect;
    private RectTransform timeTrack;
    private RectTransform targetRect;

    private void Awake()
    {
        progressCaption = progressLabel.text;
        introCaption = endMessage.text;
        timeRect = timeFill.rectTransform;
        timeTrack = (RectTransform)timeRect.parent;
        targetRect = targetIndicator.rectTransform;
        startButton.onClick.AddListener(new StartGameCommand(facade).Execute);
        restartButton.onClick.AddListener(new RestartGameCommand(facade).Execute);
        mediator.Changed += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (mediator != null) mediator.Changed -= Refresh;
    }

    private void Update()
    {
        if (!mediator.IsRunning) return;
        UpdateBar();
        float scale = 1f + pulseScale * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulseSpeed));
        targetRect.localScale = Vector3.one * scale;
    }

    private void Refresh()
    {
        UpdateBar();
        progressLabel.text = $"{progressCaption}  {mediator.Step}/{mediator.StepCount}";
        statusLabel.text = mediator.Status;

        PanelType next = mediator.NextType;
        if (next != null) targetIndicator.color = next.Color;

        gameplayGroup.SetActive(mediator.IsRunning);
        endPanel.SetActive(!mediator.IsRunning);
        endMessage.text = mediator.HasEnded ? mediator.Status : introCaption;
        startButton.gameObject.SetActive(!mediator.HasEnded);
        restartButton.gameObject.SetActive(mediator.HasEnded);
    }

    private void UpdateBar()
    {
        float amount = Mathf.Clamp01(mediator.SecondsLeft / mediator.RoundSeconds);
        float width = Mathf.Max(0f, timeTrack.rect.width - 2f * timeRect.anchoredPosition.x);
        timeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * amount);
    }
}
