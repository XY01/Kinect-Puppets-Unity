using UnityEngine;
using System.Collections;

[RequireComponent(typeof( TForm_Follow ))]
public class Dragon_LimbSection : MonoBehaviour 
{
    public Transform m_LeftLimb;
    public Transform m_RightLimb;

    

    public void SetLeftLimbRot( float rot )
    {
        m_LeftLimb.SetLocalRotZ( rot );
    }

    public void SetRightLimbRot( float rot )
    {
        m_RightLimb.SetLocalRotZ( rot );
    }
}
