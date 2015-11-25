using UnityEngine;
using System.Collections;

/// <summary>
//
// TODO:
// Deadzone/idle state 
// Perlin noise along body for breathing effect
// Setup multiple dragons
/// </summary>

public class Follow_Dragon : MonoBehaviour 
{
	public FollowTransform 	m_FollowTransform;
    public Transform        m_HandTransform;
    public Transform        m_MouseTransform;

    public ParticleSystem   m_PSys;

	public TForm_Follow	    m_HeadPrefab;
	public TForm_Follow	    m_SegmentPrefab;
	public TForm_Follow	    m_ArmPrefab;
	public TForm_Follow	    m_LegPrefab;
	public TForm_Follow	    m_TailPrefab;

    public Dragon_LimbSection m_ArmSectionPrefab;
    public Dragon_LimbSection m_LegSectionPrefab;
    public Dragon_LimbSection m_WingSectionPrefab;

    float m_ArmAngle;
    public Transform m_LShoulder;
    public Transform m_LHand;
    public Transform m_RShoulder;
    public Transform m_RHand;

    float m_LegAngle;
    public Transform m_LHip;
    public Transform m_LFoot;
    public Transform m_RHip;
    public Transform m_RFoot;

    public Texture2D m_HeadTex;
    public Texture2D m_WingTex;
    public Texture2D m_TailTex;

    public Material m_HeadMat;
    public Material m_WingMat;
    public Material m_TailMat;

	public int 			m_SegmentCount = 10;

	public float 		m_ArmPlacement = .25f;
    public float        m_WingPlacement = .3f;
	public float 		m_LegPlacement = .75f;

	TForm_Follow[] 		m_Segments;

	public float 		m_FollowDistance = .1f;

	public float 		m_Smoothing = 4;

	public AnimationCurve m_ScaleCurve;

    public Dragon_LimbSection m_Arms;
    public Dragon_LimbSection m_Legs;
    public Dragon_LimbSection m_Wings;


    float m_Speed;
    Vector3 m_PrevHeadPos;
    Vector3 m_CurrentHeadPos;

	// Use this for initialization
	void Start () 
	{
		transform.position = Vector3.zero;

        // Initialize the segment array
		m_Segments = new TForm_Follow[m_SegmentCount];

        // Get all the indexes of the positions at which the different body types will spawn
		int armIndex = (int)(   m_SegmentCount * m_ArmPlacement );
		int legIndex = (int)(   m_SegmentCount * m_LegPlacement );
        int wingIndex = (int)(  m_SegmentCount * m_WingPlacement);

        // If the arm and wing placement are at the same index, offset the wing index by 1
        if (m_ArmPlacement == m_WingPlacement)
            m_WingPlacement++;


		for (int i = 0; i < m_SegmentCount; i++)
		{
			TForm_Follow prefabToSpawn = m_SegmentPrefab;

			if( i == 0 )						prefabToSpawn = m_HeadPrefab;
            else if (i == armIndex)             prefabToSpawn = m_ArmSectionPrefab.GetComponent< TForm_Follow >();
            else if (i == legIndex)             prefabToSpawn = m_LegSectionPrefab.GetComponent< TForm_Follow >();
            else if (i == wingIndex)            prefabToSpawn = m_WingSectionPrefab.GetComponent<TForm_Follow>();
			else if( i == m_SegmentCount - 1 )	prefabToSpawn = m_TailPrefab;

            // Instantiate teh new segment
			TForm_Follow newFollowSegment = (TForm_Follow)Instantiate( prefabToSpawn );
			newFollowSegment.name = "Segment " + i;
			newFollowSegment.transform.parent = transform;

            float scaler = 1;

            if (i == armIndex)
            {
                m_Arms = newFollowSegment.GetComponent<Dragon_LimbSection>();
                scaler = m_ArmScaler;
            }
            else if (i == legIndex)
            {
                m_Legs = newFollowSegment.GetComponent<Dragon_LimbSection>();
                scaler = m_LegScaler;
            }
            else if (i == wingIndex)
            {
                scaler = m_WingScaler;
                m_Wings = newFollowSegment.GetComponent<Dragon_LimbSection>();
            }
            else if (i == 0)
            {
                scaler = m_HeadScaler;
            }
            
			float normValue = (float)i/(float)(m_SegmentCount - 1);

			float scale = m_ScaleCurve.Evaluate( normValue );


			//if( i == m_SegmentCount - 1 ) 

                newFollowSegment.transform.localScale = new Vector3(1 - normValue, scale * scaler, scale * scaler);
           

            if (i == 0)
            {
                newFollowSegment.m_FollowT = m_FollowTransform.transform;
                newFollowSegment.m_FollowDistance = 1f;
            }
            else
            {
                newFollowSegment.m_FollowT = m_Segments[i - 1].transform;
                newFollowSegment.m_FollowDistance = m_FollowDistance;
            }

			newFollowSegment.m_FollowDistance = m_FollowDistance;

			m_Segments[ i ] = newFollowSegment;
		}
	}

    void OnEnable()
    {
        m_HeadMat.SetTexture("_MainTex", m_HeadTex);
        m_WingMat.SetTexture("_MainTex", m_WingTex);
        m_TailMat.SetTexture("_MainTex", m_TailTex);

        m_PSys.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        m_PSys.gameObject.SetActive(false);
    }

	void Update () 
	{
		for (int i = 0; i < m_SegmentCount; i++)
		{
			float normValue = (float)i/(float)(m_SegmentCount - 1);

            if (i == 0)
            {
                m_Segments[i].m_Smoothing = 10f;
                m_Segments[i].m_FollowDistance = 1f;
            }
            else
            {
                m_Segments[i].m_FollowDistance = m_FollowDistance * m_ScaleCurve.Evaluate(normValue);
                m_Segments[i].m_Smoothing = m_Smoothing;
            }
		}

        Vector3 leftShoulderToArm = (m_LShoulder.position - m_LHand.position).normalized;
        float leftArmAngle = Vector3.Angle(Vector3.down, leftShoulderToArm);

        Vector3 rightShoulderToArm = (m_RShoulder.position - m_RHand.position).normalized;
        float rightArmAngle = Vector3.Angle(Vector3.down, rightShoulderToArm);


        m_Arms.SetLeftLimbRot(leftArmAngle - 110);
        m_Arms.SetRightLimbRot(rightArmAngle - 110);


        Vector3 leftHipToFoot = (m_LHip.position - m_LFoot.position).normalized;
        float leftLegAngle = Vector3.Angle(Vector3.down, leftHipToFoot);
        leftLegAngle *= 4;
        m_LeftLegAngle = Mathf.Lerp(m_LeftLegAngle, leftLegAngle, Time.deltaTime * 6);

        Vector3 rightHipToFoot = (m_RHip.position - m_RFoot.position).normalized;
        float rightLegAngle = Vector3.Angle(Vector3.down, rightHipToFoot);
        rightLegAngle *= 4;
        m_RightLegAngle = Mathf.Lerp(m_RightLegAngle, rightLegAngle, Time.deltaTime * 6);


        m_Legs.SetLeftLimbRot(m_LeftLegAngle + 90);
        m_Legs.SetRightLimbRot(m_RightLegAngle + 90);

        m_WingSeed += Time.deltaTime + ( Time.deltaTime * m_Speed * 6);

        float wingAngle = Mathf.Sin( m_WingSeed * m_WingSpeed).Scale(-1f, 1f, m_WingAngleRange.x, m_WingAngleRange.y);
        m_Wings.SetLeftLimbRot(wingAngle);
        m_Wings.SetRightLimbRot(wingAngle);     
 


        // Change head
        if( Input.GetKeyDown(KeyCode.Alpha1) )
        {
            //m_Segments[ i ].GetComponent
        }

	}

    public float m_HeadScaler = 1;
    public float m_LegScaler = 1;
    public float m_WingScaler = 1;
    public float m_ArmScaler = 1;

    float m_WingSeed = 0;

    void FixedUpdate()
    {
        m_CurrentHeadPos = m_Segments[0].transform.position;
        m_Speed = Mathf.Lerp( m_Speed, Vector3.Distance( m_CurrentHeadPos, m_PrevHeadPos ), Time.fixedDeltaTime * 10 );
        m_PrevHeadPos = m_CurrentHeadPos;
    }

    public float m_WingSpeed = 1;
    public Vector2 m_WingAngleRange;
    float m_LeftLegAngle;
    float m_RightLegAngle;
    float m_LeftArmAngle;
    float m_RightArmAngle;

    public void SwitchInputs()
    {
        if (m_FollowTransform.m_TransformToFollow == m_HandTransform) m_FollowTransform.m_TransformToFollow = m_MouseTransform;
        else if (m_FollowTransform.m_TransformToFollow == m_MouseTransform) m_FollowTransform.m_TransformToFollow = m_HandTransform;
    }

    public void PSysEmit( bool emit )
    {
        m_PSys.enableEmission = emit;
    }
}
