using UnityEngine;
using System.Collections;

public class DragonManager : MonoBehaviour {

    public Follow_Dragon[] m_Dragons;

   // OSCListener m_OSCListener;
    //OSCListener m_OSCListener;

    void Start()
    {
        m_Dragons = gameObject.GetComponentsInChildren<Follow_Dragon>() as Follow_Dragon[];
        SetToDragon(0);

       // m_OSCListener = new OSCListener("/midi/cc");
    }


    void Update()
    {
        /*
        if( m_OSCListener.Updated )
        {
            int index = (int)m_OSCListener.GetData(1);
            SetToDragon(index);
        }
        */
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) )
            SetToDragon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            SetToDragon(1);

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            SetToDragon(2);

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            SetToDragon(3);

        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            SetToDragon(4);

        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            SetToDragon(5);
    }

    void SetToDragon(int index)
    {
        index = Mathf.Clamp(index, 0, m_Dragons.Length - 1);

        for (int i = 0; i < m_Dragons.Length; i++)
        {
            m_Dragons[i].gameObject.SetActive(index == i);
        }
    }
}
