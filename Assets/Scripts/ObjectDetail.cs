using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ObjectDetail : MonoBehaviour
{
    private Image ObjectPic;
    public int ObjectNo;
    private Button ObjectButton;
    Sprite Mynow;
    // Start is called before the first frame update
    void Start()
    {
        ObjectPic = GetComponent<Image>();
        ObjectButton = GetComponent<Button>();
        ObjectButton.onClick.AddListener(OnBtnClick);
        Mynow = ObjectPic.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(Sprite Obj, int Number)
    {
        ObjectPic.sprite = Obj;
        ObjectNo = Number;
    }

    public void RemoveValue()
    {
        ObjectPic.sprite = Mynow;
        ObjectPic.fillCenter = false;
        ObjectNo = -1;
    }

    public void OnBtnClick()
    {
        print(ObjectNo);
        StartCoroutine(LevelManager.m_Instance.PlayerObject.GetComponent<FPSPlayer>().PlayerWeaponsComponent.SelectWeapon(ObjectNo));
    }
}
