using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownTrigger : MonoBehaviour
{
    private Animator pauseMenu;
    public bool gamePaused = false;

    private void Awake()
    {
        pauseMenu = GetComponent<Animator>();
    }

    public void PressPause()
    {
        if (gamePaused)
        {
            pauseMenu.SetBool("PauseButtonPressed", false);
            gamePaused = false;
        } 
        else
        {
            pauseMenu.SetBool("PauseButtonPressed", true);
            gamePaused = true;
        }
            
    }

    public void PressedButton ()
    {
        Debug.Log("Button's pressed.");
    }

    
}
