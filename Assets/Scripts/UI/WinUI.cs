using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{

    [Header("Widgets")]
    [SerializeField] private Button ExitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeListeners();
        InitializeControl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void InitializeListeners()
    {
        ExitButton.onClick.AddListener(Exit);
    }

    private void InitializeControl()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
