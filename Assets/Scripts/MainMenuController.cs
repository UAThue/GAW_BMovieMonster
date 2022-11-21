using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject menuState;
    [SerializeField]
    private GameObject gameplayState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameplay()
    {
        menuState.SetActive(false);
        gameplayState.SetActive(true);
    }
    public void ShowMainMenu()
    {
        menuState.SetActive(true);
        gameplayState.SetActive(false);
    }

}
