using UnityEngine;
using System.Collections;

public class OnClickSendMessage : MonoBehaviour {

    void OnMouseDown()
    {
        KinectPuppet_Manager.Instance.SetSelectedBodyPart(transform.parent.GetComponent<BodyPart>());
    }
}
