using UnityEngine;

public class IdleState : PlayerState
{

    private PlayerController playerController;

    public IdleState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void EnterState()
    {
        playerController.SetWalking(true);
    }

    public void UpdateState()
    {
        if (playerController.GetPlayerInput().movement != Vector2.zero)
        {
            playerController.SetCurrentPlayerState(playerController.GetWalkState());
        }

        playerController.LookX(playerController.GetPlayerInput().mouse.x);
        playerController.LookY(playerController.GetPlayerInput().mouse.y);

        playerController.ApplyGravity();

        playerController.CastForwardRay();
    }
}
