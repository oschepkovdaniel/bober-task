using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [Header("Widgets")]
    [SerializeField] private GameObject[] InventorySprites;
    [SerializeField] private GameObject[] Selectors;
    [SerializeField] private Image Crosshair;

    [Header("Params")]
    [SerializeField] private Color CrosshairNormalColor;
    [SerializeField] private Color CrosshairInteractColor;

    private GameObject player;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCrosshair();
        UpdateInventory();
    }

    private void InitializePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void UpdateInventory()
    {
        for (int i = 0; i < InventorySprites.Length; i++)
        {
            InventoryItem item = playerController.GetInventoryItem(i);
            if (item)
            {
                InventorySprites[i].SetActive(true);
                InventorySprites[i].GetComponent<Image>().sprite = item.GetSprite();
            } else
            {
                InventorySprites[i].SetActive(false);
            }

            if (playerController.GetCurrentItem() == i)
            {
                Selectors[i].SetActive(true);
            }
            else
            {
                Selectors[i].SetActive(false);
            }
        }
    }

    private void UpdateCrosshair()
    {
        if (playerController.GetHasInteract())
        {
            Crosshair.color = CrosshairInteractColor;
        } else
        {
            Crosshair.color = CrosshairNormalColor;
        }
    }
}
