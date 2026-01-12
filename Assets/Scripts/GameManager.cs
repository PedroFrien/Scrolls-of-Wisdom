using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static reference to the single instance of the GameManager
    public static GameManager Instance { get; private set; }

    private bool CursorLocked = true;

    private FPController playerController;

    //private GameObject playerObject;

    // Other game manager variables and methods can go here
    private void Awake()
    {
        // If an instance already exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Set this object as the instance
        Instance = this;

        // Optional: Keep the GameManager alive when loading new scenes
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("GameManager instance created.");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<FPController>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        FindFirstObjectByType<LoopArea>().StartCoroutine("RestartLoop");
    }

    public void SetCursorActive(bool active)
    {
        if (active)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerController.LookEnabled = false;
            playerController.MovementEnabled = false;
        }

        if (!active)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerController.LookEnabled = true;
            playerController.MovementEnabled = true;
        }
        
    }
}
