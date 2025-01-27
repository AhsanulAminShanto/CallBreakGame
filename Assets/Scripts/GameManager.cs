using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Deck deck;  // Reference to the Deck script
    public List<SpriteRenderer> p1Cards;  // Player 1's cards
    public List<SpriteRenderer> p2Cards;  // Player 2's cards
    public List<SpriteRenderer> p3Cards;  // Player 3's cards
    public List<SpriteRenderer> p4Cards;  // Player 4's cards
    private List<Sprite> dealtCards = new List<Sprite>();  // List to track dealt cards
    private int currentPlayerIndex = 0;  // Track whose turn it is
    private List<List<SpriteRenderer>> players;  // List of players' card lists

    // Arrays to store card numbers for each player
    private int[] player1Cards = new int[13];
    private int[] player2Cards = new int[13];
    private int[] player3Cards = new int[13];
    private int[] player4Cards = new int[13];

    // Arrays that represent the groups for the cards (a, b, c, d)
    private int[] a = new int[13] { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48 };
    private int[] b = new int[13] { 1, 5, 9, 13, 17, 21, 25, 29, 33, 37, 41, 45, 49 };
    private int[] c = new int[13] { 2, 6, 10, 14, 18, 22, 26, 30, 34, 38, 42, 46, 50 };
    private int[] d = new int[13] { 3, 7, 11, 15, 19, 23, 27, 31, 35, 39, 43, 47, 51 };

    private int[] fullDeck = {
        5, 6, 8, 7, 9, 12, 10, 11, 16, 15, 13, 14, 20, 19, 17, 18,
        24, 23, 21, 22, 28, 27, 25, 26, 32, 31, 29, 30, 38, 35, 33, 34,
        42, 41, 39, 40, 46, 45, 43, 44, 50, 49, 47, 48, 54, 53, 51, 52,
        1, 2, 4, 3
    };

    private int[] lastPlayedCardIndices = new int[4];

    void Start()
    {
        players = new List<List<SpriteRenderer>> { p1Cards, p2Cards, p3Cards, p4Cards };
        DealDeck();
        StartTurn();
    }

    void Update()
    {
        // Handle any necessary updates here
    }

    public void DealDeck()
    {
        for (int i = 0; i < players.Count; i++)
        {
            DealCards(players[i], i + 1);
        }

        // Log the final arrays for each player after dealing
        Debug.Log("Player 1 final cards: " + string.Join(", ", player1Cards));
        Debug.Log("Player 2 final cards: " + string.Join(", ", player2Cards));
        Debug.Log("Player 3 final cards: " + string.Join(", ", player3Cards));
        Debug.Log("Player 4 final cards: " + string.Join(", ", player4Cards));
    }

    public void DealCards(List<SpriteRenderer> player, int playerNumber)
    {
        int[] cardNumbers = new int[13]; // Temporary array for the current player

        int i = 0;
        while (i < 13)
        {
            int randomValue = Random.Range(0, deck.card.Count);

            if (!dealtCards.Contains(deck.card[randomValue]))
            {
                player[i].sprite = deck.card[randomValue];
                player[i].gameObject.AddComponent<BoxCollider2D>();
                player[i].gameObject.AddComponent<CardClickHandler>();
                player[i].gameObject.GetComponent<CardClickHandler>().SetGameManager(this);
                dealtCards.Add(deck.card[randomValue]);

                // Log the card dealt to the player
                int spriteIndex = GetSpriteIndex(deck.card[randomValue]);

                // Insert the card number into the appropriate player's array
                cardNumbers[i] = spriteIndex;

                i++;
            }
        }

        // Assign the temporary array to the corresponding player's array
        switch (playerNumber)
        {
            case 1:
                player1Cards = cardNumbers;
                break;
            case 2:
                player2Cards = cardNumbers;
                break;
            case 3:
                player3Cards = cardNumbers;
                break;
            case 4:
                player4Cards = cardNumbers;
                break;
        }

        // Optional: Log the card numbers array for the player
        Debug.Log($"Player {playerNumber} card numbers: {string.Join(", ", cardNumbers)}");
    }

    public int GetSpriteIndex(Sprite sprite)
    {
        if (deck.card.Contains(sprite))
        {
            return deck.card.IndexOf(sprite);
        }
        return -1; // Return -1 if the sprite is not found
    }

    public bool IsCurrentPlayer(int playerIndex)
    {
        return currentPlayerIndex == playerIndex;
    }

    public void EndTurn()
    {
        if (currentPlayerIndex == 3) // After Player 4 has played, check for the winner
        {
            CheckForWinner();
        }
        else
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            StartTurn();
        }
    }

    public void PlayerMoveCompleted(int lastPlayedCardIndex)
    {
        UpdateLastPlayedCardIndex(currentPlayerIndex, lastPlayedCardIndex);
        string lastCardGroup = GetCardGroup(lastPlayedCardIndex);
        SetActiveCardsForGroup(lastCardGroup);
        EndTurn();
    }

    public void UpdateLastPlayedCardIndex(int playerIndex, int cardIndex)
    {
        lastPlayedCardIndices[playerIndex] = cardIndex;
    }

    private int GetLastPlayedCardIndex(int playerIndex)
    {
        return lastPlayedCardIndices[playerIndex];
    }

    void StartTurn()
    {
        Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn");
    }

    void SetActiveCardsForGroup(string group)
    {
        // Define the indices for the group
        int[] groupIndices = group switch
        {
            "a" => a,
            "b" => b,
            "c" => c,
            "d" => d,
            _ => new int[0]
        };

        // Iterate through all players and their cards to deactivate or activate based on group
        foreach (var player in players)
        {
            for (int i = 0; i < player.Count; i++)
            {
                SpriteRenderer cardRenderer = player[i];
                int cardIndex = GetSpriteIndex(cardRenderer.sprite);

                // Activate card if it's in the current group, otherwise deactivate
                if (ArrayContains(groupIndices, cardIndex))
                {
                    cardRenderer.gameObject.SetActive(true);
                }
                else
                {
                    cardRenderer.gameObject.SetActive(false);
                }
            }
        }
    }

    private bool ArrayContains(int[] array, int value)
    {
        foreach (int item in array)
        {
            if (item == value)
            {
                return true;
            }
        }
        return false;
    }

    public string GetCardGroup(int cardIndex)
    {
        // Match the card index to one of the groups (a, b, c, d)
        if (System.Array.Exists(a, element => element == cardIndex))
        {
            return "a";
        }
        else if (System.Array.Exists(b, element => element == cardIndex))
        {
            return "b";
        }
        else if (System.Array.Exists(c, element => element == cardIndex))
        {
            return "c";
        }
        else if (System.Array.Exists(d, element => element == cardIndex))
        {
            return "d";
        }
        return "unknown";
    }

    private void CheckForWinner()
    {
        int highestIndex = lastPlayedCardIndices[0];
        int winnerIndex = 0;
        for (int i = 1; i < lastPlayedCardIndices.Length; i++)
        {
            if (lastPlayedCardIndices[i] > highestIndex)
            {
                highestIndex = lastPlayedCardIndices[i];
                winnerIndex = i;
            }
        }

        // Announce the winner
        Debug.Log($"Player {winnerIndex + 1} is the winner of this round with card index {highestIndex}.");

         // Remove the last played card from each player's list
    for (int i = 0; i < players.Count; i++)
    {
        players[i].RemoveAt(players[i].Count - 1);
    }

    // Reset the visibility of all remaining cards
    ResetCardsVisibility();

    // Start the next round or end the game if necessary
    currentPlayerIndex = 0;  // Reset to Player 1 or the next starting player
    StartTurn();
    }
    void ResetCardsVisibility()
{
    foreach (var player in players)
    {
        foreach (var cardRenderer in player)
        {
            cardRenderer.gameObject.SetActive(true);
        }
    }
}
}
