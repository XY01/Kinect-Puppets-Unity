using UnityEngine;
using System.Collections;

public class LabeledSlider
{
	public string 	m_TextfieldString = "lol";
	float 			m_Value = .5f;
	string 			m_Label = "Label";
	float 			m_MinRange = 0;
	float 			m_MaxRange = 1;
	string 			m_AdditionalString;
    bool            m_ClampToRange = false;
    bool            m_SendToOSC = false;
    string          m_OSCaddress = "/address";
    float           m_PreviousValue = .5f;


    LabeledSliderType m_SliderType = LabeledSliderType.Float;

    public enum LabeledSliderType
    {
        Float,
        Int
    }

	public LabeledSlider( string label, float min, float max, float initialVal )
	{
		m_Label = label;
		m_MinRange = min;
		m_MaxRange = max;
		m_Value = initialVal;
	}


    public LabeledSlider(string label, float min, float max, float initialVal, bool clampToRange)
    {
        m_Label = label;
        m_MinRange = min;
        m_MaxRange = max;
        m_Value = initialVal;
        m_ClampToRange = clampToRange;
    }

    public LabeledSlider(string label, float min, float max, float initialVal, LabeledSliderType type)
    {
        m_Label = label;
        m_MinRange = min;
        m_MaxRange = max;
        m_Value = initialVal;
        m_SliderType = type;
    }


    public LabeledSlider(string label, float min, float max, float initialVal, bool clampToRange, LabeledSliderType type)
    {
        m_Label = label;
        m_MinRange = min;
        m_MaxRange = max;
        m_Value = initialVal;
        m_ClampToRange = clampToRange;
        m_SliderType = type;
    }

    public LabeledSlider( string label, float min, float max, float initialVal, bool sendToOSC, string oscAddress )
    {
        m_Label = label;
        m_MinRange = min;
        m_MaxRange = max;
        m_Value = initialVal;
        m_SendToOSC = sendToOSC;
        m_OSCaddress = oscAddress;
    }

	public LabeledSlider( string label, float min, float max, float initialVal, bool sendToOSC, string oscAddress, LabeledSliderType type )
	{
		m_Label = label;
		m_MinRange = min;
		m_MaxRange = max;
		m_Value = initialVal;
		m_SendToOSC = sendToOSC;
		m_OSCaddress = oscAddress;
		m_SliderType = type;
	}



	public float Draw( float val, string additionalString )		
	{
		m_AdditionalString = additionalString;
		return Draw( val );
	}

	public float Draw( float val )		
	{
        if (m_ClampToRange)
        {
            m_Value = Mathf.Clamp(val, m_MinRange, m_MaxRange);
        }
        else
        {
            m_Value = val;
        }
        
        GUILayout.BeginHorizontal( );
		{
			GUILayout.Label( m_Label + m_AdditionalString );	

			m_Value = GUILayout.HorizontalSlider( m_Value, m_MinRange, m_MaxRange, GUILayout.Width( 100 ) );
			
			GUI.SetNextControlName(m_Label);

			m_TextfieldString = GUILayout.TextField( m_TextfieldString, GUILayout.Width( 40 ) );


			if( GUI.GetNameOfFocusedControl() == m_Label ) 
			{
				if( Event.current.isKey && Event.current.keyCode == KeyCode.Return )
				{
	                if (!float.TryParse( m_TextfieldString, out m_Value) )
					{
	                    m_Value = m_MinRange;
	                    if (m_SliderType == LabeledSliderType.Float)
	                    {
	                        m_TextfieldString = m_MinRange.ToDoubleDecimalString();
	                    }
	                    else
	                    {
	                        m_TextfieldString = m_Value.ToString("00");
	                    }
					}

					m_TextfieldString = m_Value.ToString("00");
				}
			}
			else
			{
                if (m_SliderType == LabeledSliderType.Float)
                {
                    m_TextfieldString = m_Value.ToDoubleDecimalString();
                }
                else
                {
                    m_TextfieldString = m_Value.ToString("00");
                }
			}

		}
		GUILayout.EndHorizontal();

        if ( m_SendToOSC )
            SendOSC();

		return m_Value;
	}

    public void SendOSC ()
    {
        if (m_Value != m_PreviousValue && m_OSCaddress != null)
            OSCHandler.Instance.SendOSCMessage(m_OSCaddress, m_Value);

        m_PreviousValue = m_Value;
    }

}
