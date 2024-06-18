using UnityEngine;

public class CupFiller : MonoBehaviour, BaseInteractable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    public void OnEnter(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }

        SetState(true);
        player.ProgressBar.Activate();

        if ((!player.Mug.IsFilled) || (!player.Mug.IsActive))
        {
            player.Mug.Activate();
        }
    }

    public void OnExit(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }

        SetState(false);
        player.ProgressBar.Deactivate();

        if (!player.Mug.IsFilled)
        {
            player.Mug.Deactivate();
        }
    }
    public void OnStay(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }
        player.ProgressBar.SetProgress(player.Mug.FillAmount);
    }
    public void OnInteractButton(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }

        player.Mug.Fill();
    }

    public void OnInteractButtonDown(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }
    }

    public void OnInteractButtonUp(Player player)
    {
        if (player.CurrentState != ControllerState.TopDown) { return; }
    }
    private void SetState(bool active)
    {
        meshRenderer.sharedMaterial = active ? activeMaterial : inactiveMaterial;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(GetComponent<BoxCollider>().bounds.center, GetComponent<BoxCollider>().bounds.size);
    }
}
