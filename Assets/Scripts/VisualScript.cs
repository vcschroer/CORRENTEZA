using UnityEngine;

public class VisualScript : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void OnJumpAnimation()
    {
        player.playingJumpAnim = true;
    }
}