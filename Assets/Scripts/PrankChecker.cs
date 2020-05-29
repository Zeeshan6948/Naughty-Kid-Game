using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrankChecker : MonoBehaviour
{
    public int PrankObjectsCount;
    public GameObject PrankObject;
    public bool PrankSet;
    public bool PrankComplete;
    public GameObject PlacingObjects;
    public AudioClip PrankSound;
    public string ThiefAnim;
    int currentCount;
    int pickedCount;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void GotObject()
    {
        currentCount++;
        if(currentCount == PrankObjectsCount)
        {
            PlantingTrap();
        }
    }
    public void PickingObject()
    {
        pickedCount++;
        if (pickedCount == PrankObjectsCount)
        {
            LevelManager.m_Instance.CanvasObject.PlayerMessage("Put the Picked Object Infornt of Door to Plant Trap");
            PlacingObjects.SetActive(true);
        }
    }
    void PlantingTrap()
    {
        LevelManager.m_Instance.CanvasObject.PlayerMessage("Run Away Before Stranger Gets In");
        if (PrankObject != null)
            PrankObject.SetActive(true);
        PrankSet = true;
        Invoke("WaitAfter",6);
    }

    void WaitAfter()
    {
        LevelManager.m_Instance.StartThiefOperation();
    }

    public void PrankHappen()
    {
        PrankComplete = true;
        LevelManager.m_Instance.Theif.StopAllCoroutines();
        LevelManager.m_Instance.Theif.transform.GetChild(0).GetComponent<Animator>().SetInteger("AnimState",5);
        LevelManager.m_Instance.Theif.transform.GetChild(0).GetComponent<Animator>().SetTrigger(ThiefAnim);
        LevelManager.m_Instance.TheifSoundOut.clip = PrankSound;
        LevelManager.m_Instance.TheifSoundOut.Play();
        LevelManager.m_Instance.LevelCompleted();
    }
}
