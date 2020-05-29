using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ObjectsTypes
{Checker, FlourSack, Pulley, Rope, Bucket, NailBox, Oilbox}

public class Detector : MonoBehaviour
{
    public ObjectsTypes myType;
    GameObject DestroyOne;
    private void OnTriggerEnter(Collider other)
    {
        if (myType == ObjectsTypes.Checker && other.tag == "Usable")
        {
            switch ((int)other.GetComponent<Detector>().myType)
            {
                case 1:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
                case 2:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
                case 3:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
                case 4:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
                case 5:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
                case 6:
                    LevelManager.m_Instance.CurrentPrank.GotObject();
                    DestroyOne = other.gameObject;
                    Invoke("DestoryObject", 1);
                    break;
            }
        }
        if (myType == ObjectsTypes.Checker && other.gameObject.layer == 13)
        {
            if (LevelManager.m_Instance.CurrentPrank.PrankSet)
            {
                LevelManager.m_Instance.CurrentPrank.PrankHappen();
                DestroyOne = this.gameObject;
                DestoryObject();
            }
            else
            {
                LevelManager.m_Instance.LevelFailed();
                DestroyOne = this.gameObject;
                DestoryObject();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    void DestoryObject()
    {
        Destroy(DestroyOne);
    }
}
