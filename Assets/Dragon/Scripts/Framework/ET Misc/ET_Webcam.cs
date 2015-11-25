#if UNITY_STANDALONE_WIN

using UnityEngine;
using System.Collections;


/// <summary>
/// Webcam.
/// * Needs Load Save
/// </summary>
[RequireComponent (typeof ( ET_GUIWindow ))]
public class ET_Webcam : ET_ManagerBase
{
	static 			ET_Webcam m_Instance { get; set; }
	public static 	ET_Webcam Instance{ get { return m_Instance; } }
	
	
	WebCamTexture 		m_CamTex;
	public GUITexture 	m_GUITexture;
	string[] 			m_DeviceNames;
	public int 			m_SelectedDevice;
	int 				PrevIndex;

    bool                m_CamSelected = false;
	
	public Material 	m_Mat;		// Unlit texture. TODO: create automatically 
	
	void Awake()
	{
		m_Instance = this;
	}
	
	protected override void Start ()
	{
		base.Start();
		
		m_Mat = new Material (Shader.Find("Unlit/Texture"));
        m_AvCol = HSBColor.FromColor(Color.black);
		FindDevices();
	}
	
	void FindDevices()
	{
		m_DeviceNames = new string[ WebCamTexture.devices.Length ];

		for( int i = 0; i < m_DeviceNames.Length; i++ )
		{
			m_DeviceNames[ i ] = WebCamTexture.devices[i].name;
		}
	}
	
	void CheckSelectedCam()
	{
		if( m_SelectedDevice != PrevIndex )		// If the selected device index has been updated
		{
			SetSelectedCam( m_SelectedDevice );
		}
	}

	void SetSelectedCam( int index )
	{
		if( m_CamTex != null )	// Stop the existing cam tex if there is one
			m_CamTex.Stop();

        m_SelectedDevice = index;

		m_CamTex = new WebCamTexture( WebCamTexture.devices[ m_SelectedDevice ].name );	// Set the new cam tex
		m_CamTex.Play();																// Play the new cam tex
		
		m_Mat.SetTexture( "_MainTex", m_CamTex );
		
		if( m_GUITexture != null )														// If there is a GUItex set the texture
			m_GUITexture.texture = m_CamTex;
	}

	void Update ()
	{
	}

    void FixedUpdate()
    {
        if (m_RecordStats)
            FindAverageColor();
    }

    HSBColor m_AvCol;
    bool     m_RecordStats = false;

	public void FindAverageColor()
	{
		Color[] allPixels = m_CamTex.GetPixels();
		//Vector4 aggregateCol = Vector4.zero;
		Color aggregateCol = allPixels[ 0 ];
		
		for (int i = 1; i < allPixels.Length; i++) 
		{
			aggregateCol += allPixels[ i ];
		}
		
		aggregateCol /= allPixels.Length;

        m_AvCol = HSBColor.FromColor(aggregateCol);		
	}
	

	void DrawGUIWindow()
	{
        if (m_CamTex != null)
            GUILayout.Label(m_CamTex, GUILayout.Width(m_CamTex.width / 3), GUILayout.Height(m_CamTex.height / 3));

        GUILayout.Height(10);
                
        GUILayout.BeginVertical("box");
        GUILayout.Label("Select input");
        for (int i = 0; i < m_DeviceNames.Length; i++)
        {
            if (GUILayout.Button(m_DeviceNames[i], GUILayout.Height(20 ), GUILayout.Width(200)))
            {
                SetSelectedCam(i);
            }
        }
        GUILayout.EndVertical();

        GUILayout.Height(10);

        /*
        m_RecordStats = GUILayout.Toggle(m_RecordStats, "Record stats: ");
        GUILayout.Label("Av Hue: " + m_AvCol.h );
        GUILayout.Label("Av Sat: " + m_AvCol.s);
        GUILayout.Label("Av Bright: " + m_AvCol.b);
         * */
	}
}

#endif
