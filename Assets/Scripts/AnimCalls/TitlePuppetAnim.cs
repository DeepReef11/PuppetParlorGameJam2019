using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePuppetAnim : MonoBehaviour
{
    private Animator puppetAnim;

    private void Awake()
    {
        puppetAnim = GetComponent<Animator>();
    }

    public void QuitButtonMouseOver()
    {
        puppetAnim.SetBool("MouseOverQuit", true);
    }

    public void QuitButtonMouseOff()
    {
        puppetAnim.SetBool("MouseOverQuit", false);
    }
}