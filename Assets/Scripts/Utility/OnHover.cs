using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler
{
    public AudioManager AudioManager;
    public string audioName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(audioName != "")
            AudioManager.Play(audioName);
    }
}
