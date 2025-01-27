using UnityEngine;

public class CardClickHandler : MonoBehaviour
{
    private GameManager gameManager;
    private Camera mainCamera;
    public Transform centerPosition;

    private Vector3[] playerPositions = new Vector3[4];

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize predefined player positions
        playerPositions[0] = new Vector3(Screen.width / 1.08f, Screen.height / 1.5f, mainCamera.nearClipPlane); // Center
        playerPositions[1] = new Vector3(Screen.width / 1.36f, Screen.height / 5.6f, mainCamera.nearClipPlane); // Left
        playerPositions[2] = new Vector3(Screen.width / 1.08f, Screen.height / 1.05f, mainCamera.nearClipPlane); // Right
        playerPositions[3] = new Vector3(Screen.width / 1.2f, Screen.height / 1.07f, mainCamera.nearClipPlane); // Top
    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    void OnMouseDown()
    {
        int playerIndex = transform.parent.name switch
        {
            "Player1" => 0,
            "Player2" => 1,
            "Player3" => 2,
            "Player4" => 3,
            _ => -1
        };

        if (playerIndex == -1)
        {
            Debug.LogError("Invalid player index.");
            return;
        }

        if (gameManager.IsCurrentPlayer(playerIndex))
        {
            int spriteIndex = MoveCardToPosition(playerIndex);
            gameManager.PlayerMoveCompleted(spriteIndex);
        }
        else
        {
            Debug.Log($"It's not Player {playerIndex + 1}'s turn");
        }
    }

    private int MoveCardToPosition(int playerIndex)
    {
        Vector3 targetScreenPosition = playerPositions[playerIndex];
        Vector3 targetWorldPosition = mainCamera.ScreenToWorldPoint(targetScreenPosition);

        transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, transform.position.z);
        transform.SetParent(centerPosition);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        int spriteIndex = gameManager.GetSpriteIndex(spriteRenderer.sprite);

        // Log the card's array (a, b, c, or d) based on its index
        string group = gameManager.GetCardGroup(spriteIndex);
        Debug.Log($"Player {playerIndex + 1} played card {spriteRenderer.sprite.name} (Index: {spriteIndex}) which belongs to group: {group}");

        Debug.Log($"Card {spriteRenderer.sprite.name} with index {spriteIndex} moved to position for Player {playerIndex + 1}");

        return spriteIndex;
    }
}
