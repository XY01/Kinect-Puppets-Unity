using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KinectPuppet_GUI : MonoBehaviour 
{
    public Button m_Btn_NextPuppet;
    public Button m_Btn_PrevPuppet;
    public Text m_PuppetName;

    // Puppet editor
    public Text m_SelectedLimbName;
    public Text m_SelectedLimbIndex;

    public Button m_Btn_PrevLimb;
    public Button m_Btn_NextLimb;

    public Button m_Btn_Save;

    public Button m_Btn_SaveAs;
    public InputField m_SaveName;

    public Dropdown m_SelectedPuppet;


	// Use this for initialization
    public void Init( List< KinectPuppetProfile > profiles )
    {
        // Add all names to the drop down list
        m_SelectedPuppet.options.Clear();
        for (int i = 0; i < profiles.Count; i++)
        {
            m_SelectedPuppet.options.Add(new Dropdown.OptionData(profiles[i].m_Name));
        }

        // Add a listener to the dropdown chaneg value
        m_SelectedPuppet.onValueChanged.AddListener
        ( (int i) =>
            {
                KinectPuppet_Manager.Instance.m_Puppets[0].LoadProfile(KinectPuppet_Manager.Instance.m_PuppetProfiles[ m_SelectedPuppet.value ]);
            }
        );

       // m_SelectedPuppet.itemText.text = m_SelectedPuppet.options[m_SelectedPuppet.value].text;

        m_Btn_PrevLimb.onClick.AddListener
        ( () =>
                {
                    if (KinectPuppet_Manager.Instance.m_SelectedBodyPart != null)
                    {
                        KinectPuppet_Manager.Instance.m_Puppets[0].PrevPart(KinectPuppet_Manager.Instance.m_SelectedBodyPart);
                        UpdateSelectedLimb();
                    }
                }
        );

        m_Btn_NextLimb.onClick.AddListener
        (() =>
             {
                 if (KinectPuppet_Manager.Instance.m_SelectedBodyPart != null)
                 {
                     KinectPuppet_Manager.Instance.m_Puppets[0].NextPart(KinectPuppet_Manager.Instance.m_SelectedBodyPart);
                     UpdateSelectedLimb();
                 }
             }
        );
	}

    public void UpdateProfileList( List<KinectPuppetProfile> profiles )
    {
        // Add all names to the drop down list
        m_SelectedPuppet.options.Clear();

        for (int i = 0; i < profiles.Count; i++)
        {
            m_SelectedPuppet.options.Add(new Dropdown.OptionData(profiles[i].m_Name));
        }
    }

    public void UpdateSelectedLimb()
    {
        m_SelectedLimbName.text = KinectPuppet_Manager.Instance.m_SelectedBodyPart.m_BodyPart.ToString();
        m_SelectedLimbIndex.text = "" + KinectPuppet_Manager.Instance.m_SelectedBodyPart.name.ToString()[0];
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
