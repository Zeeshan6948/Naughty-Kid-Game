using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyDailog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("PrivacyAccepted") == 1)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PrivacyAcceptedButton()
    {
        PlayerPrefs.SetInt("PrivacyAccepted", 1);
        Destroy(this.gameObject);
    }

    public void PrivacyPolicyBtn()
    {
        Application.OpenURL("http://www.redleos.com/privacy.html");
    }

    public void TermAndConditionBtn()
    {
        Application.OpenURL("http://www.redleos.com/terms.html");
    }
}
