using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Controller> players;
    public PawnSpider playerPawn;

    [SerializeField]
    private MainMenuController mainMenuController;


    private void Awake()
    {
        // Setup my Game Manager as a singleton
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Debug.LogWarning("Warning: Two Game Managers - destroying this one: " + gameObject.name);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Start in Main Menu
        mainMenuController.ShowMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
