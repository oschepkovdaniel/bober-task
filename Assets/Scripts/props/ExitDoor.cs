using UnityEngine;

public class ExitDoor : MonoBehaviour, InteractObject
{

    [Header("Params")]
    [SerializeField] private bool bRequireSpecificItem;
    [SerializeField] private string SpecificItemName;

    [Header("Prefabs")]
    [SerializeField] private GameObject WinUI;

    [Header("Audio")]
    [SerializeField] private AudioSource AudioOpen;
    [SerializeField] private AudioSource AudioLocked;

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
        
    }

    public void Interact()
    {
        if (bRequireSpecificItem)
        {
            if (playerController.GetCurrentItemName().Equals(SpecificItemName))
            {
                Enter();
            } else
            {
                AudioLocked.Play();
            }
        } else
        {
            Enter();
        }
    }

    private void Enter()
    {
        AudioOpen.Play();
        Instantiate(WinUI);
    }

    private void InitializePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }
}
