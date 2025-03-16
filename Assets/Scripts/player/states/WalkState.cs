using UnityEngine;

public class WalkState : PlayerState
{

    private PlayerController playerController;

    public WalkState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void EnterState()
    {
        playerController.SetWalking(false);
    }

    public void UpdateState()
    {
        if (playerController.GetPlayerInput().movement == Vector2.zero)
        {
            playerController.SetCurrentPlayerState(playerController.GetIdleState());
        }

        playerController.LookX(playerController.GetPlayerInput().mouse.x);
        playerController.LookY(playerController.GetPlayerInput().mouse.y);

        playerController.Move(playerController.gameObject.transform.right, playerController.GetPlayerInput().movement.x);
        playerController.Move(playerController.gameObject.transform.forward, playerController.GetPlayerInput().movement.y);

        playerController.ApplyGravity();

        playerController.CastForwardRay();
    }
}
