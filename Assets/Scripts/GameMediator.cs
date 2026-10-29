using System;
using UnityEngine;

public sealed class GameMediator : MonoBehaviour
{
    [SerializeField, Min(1f)] private float roundSeconds = 8f;
    [SerializeField] private PanelType[] sequence;
    [SerializeField] private ControlPanel[] panels;
    [SerializeField] private PlayerController player;

    [Header("Mensagens")]
    [SerializeField] private string introMessage = "Ative o protocólo para começar.";
    [SerializeField] private string startMessage = "Encontre a cor indicada.";
    [SerializeField] private string correctMessage = "Painel correto! Próximo sinal liberado.";
    [SerializeField] private string wrongMessage = "Painel incorreto. Reator em sobrecarga!";
    [SerializeField] private string winMessage = "Reator desligado. Estação salva!";
    [SerializeField] private string timeoutMessage = "Tempo esgotado. Reator em sobrecarga!";

    private int step;
    private float secondsLeft;
    private PanelType[] currentSequence;

    public event Action Changed;
    public bool IsRunning { get; private set; }
    public bool HasEnded { get; private set; }
    public int Step => step;
    public int StepCount => sequence == null ? 0 : sequence.Length;
    public float SecondsLeft => secondsLeft;
    public float RoundSeconds => roundSeconds;
    public PanelType NextType => currentSequence != null && step < StepCount ? currentSequence[step] : null;
    private string status;
    public string Status => status ?? introMessage;

    private void Update()
    {
        if (!IsRunning) return;
        secondsLeft = Mathf.Max(0f, secondsLeft - Time.deltaTime);
        if (secondsLeft <= 0f) End(timeoutMessage);
    }

    public void Begin()
    {
        if (sequence == null || sequence.Length == 0 || panels == null || player == null || !SequenceIsValid())
        {
            Debug.LogError("Protocolo: confira a sequencia e os paineis. Cada cor pode aparecer no maximo duas vezes e precisa de um painel para cada uso.", this);
            return;
        }

        ShuffleSequence();
        foreach (ControlPanel panel in panels) panel.ResetPanel();
        player.ResetPosition();
        step = 0;
        secondsLeft = roundSeconds;
        IsRunning = true;
        HasEnded = false;
        status = startMessage;
        Changed?.Invoke();
    }

    private bool SequenceIsValid()
    {
        foreach (PanelType type in sequence)
        {
            if (type == null) return false;

            int uses = 0;
            int available = 0;
            foreach (PanelType item in sequence) if (item == type) uses++;
            foreach (ControlPanel panel in panels)
            {
                if (panel == null) return false;
                if (panel.Type == type) available++;
            }
            if (uses > 2 || uses > available) return false;
        }
        return true;
    }

    private void ShuffleSequence()
    {
        PanelType[] previous = currentSequence;
        currentSequence = (PanelType[])sequence.Clone();

        for (int i = currentSequence.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            PanelType chosen = currentSequence[i];
            currentSequence[i] = currentSequence[j];
            currentSequence[j] = chosen;
        }

        bool same = previous != null && previous.Length == currentSequence.Length;
        for (int i = 0; same && i < currentSequence.Length; i++)
            if (currentSequence[i] != previous[i]) same = false;

        if (!same) return;
        for (int i = 1; i < currentSequence.Length; i++)
        {
            if (currentSequence[i] == currentSequence[0]) continue;
            PanelType first = currentSequence[0];
            currentSequence[0] = currentSequence[i];
            currentSequence[i] = first;
            break;
        }
    }

    public void TryActivate(ControlPanel panel)
    {
        if (!IsRunning || panel == null || panel.IsActivated) return;

        if (panel.Type != NextType)
        {
            End(wrongMessage);
            return;
        }
        panel.Activate();
        step++;
        secondsLeft = roundSeconds;
        status = correctMessage;
        if (step == StepCount)
        {
            End(winMessage);
            return;
        }

        Changed?.Invoke();
    }

    private void End(string finalMessage)
    {
        IsRunning = false;
        HasEnded = true;
        status = finalMessage;
        Changed?.Invoke();
    }
}
