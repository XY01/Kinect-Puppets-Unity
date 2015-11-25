using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Graph : MonoBehaviour
{
	Material 		m_LineMat;
	Material 		m_DebugMat;
	public Color 	m_Col;
	public float[] 	m_Values = new float[ 200 ];
	int 			m_MaxLength = 10;
	
	public Vector2 	m_SizeInPixels = new Vector2( 400, 200 );
	public Vector2 	m_SizeInScreenSpace = new Vector2( .1f, .05f );
	public Vector2 	m_Offset;

	public Vector2 	m_Range = new Vector2( 0, 1 );
	

	
	public bool 	m_Draw = true;
	
	void Awake()
	{		
		 m_LineMat = new Material(
			"Shader \"Lines/Wireframe\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass { " + 
			"ZWrite off " + 
			"ZTest LEqual " +
			"Blend SrcAlpha OneMinusSrcAlpha " + 
			"Lighting Off " +
			"Offset -1, -1 " +
			"Color[_Color] }}}");

		m_DebugMat = new Material(
			"Shader \"Lines/Wireframe\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,.5) } SubShader { Pass { " + 
			"ZWrite off " + 
			"ZTest LEqual " +
			"Blend SrcAlpha OneMinusSrcAlpha " + 
			"Lighting Off " +
			"Offset -1, -1 " +
			"Color[_Color] }}}");

		//m_SizeInScreenSpace.x = m_SizeInPixels.x / Screen.width;
		//m_SizeInScreenSpace.y = m_SizeInPixels.y / Screen.height;
	}
	
	public void AddValue( float val )
	{
		m_SizeInScreenSpace.x = m_SizeInPixels.x / Screen.width;
		m_SizeInScreenSpace.y = m_SizeInPixels.y / Screen.height;
		
		for( int p = m_Values.Length-1; p > 0; p-- )
			m_Values[ p ] = m_Values[ p-1 ];
		
		m_Values[ 0 ] = val;
	}

	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Home ) )
			m_Draw = !m_Draw;		

		m_Offset = Camera.main.WorldToViewportPoint( transform.position );
	}
	
	public bool m_LogGraph = true;
	void OnRenderObject()
	{   
		if( !m_Draw ) return;
		
	    GL.PushMatrix();
		GL.LoadOrtho ();
		
	    GL.Begin(GL.LINES);
		m_LineMat.SetPass(0);
	    GL.Color( Color.white );
		
		float dist = 1f / m_Values.Length;
		
		GL.Vertex( new Vector2( ( 0 * m_SizeInScreenSpace.x ) + m_Offset.x , (1 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		GL.Vertex( new Vector2( ( 1 * m_SizeInScreenSpace.x ) + m_Offset.x , (1 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		
		GL.Vertex( new Vector2( ( 0 * m_SizeInScreenSpace.x ) + m_Offset.x , (0 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		GL.Vertex( new Vector2( ( 1 * m_SizeInScreenSpace.x ) + m_Offset.x , (0 * m_SizeInScreenSpace.y) + m_Offset.y ) );
	
		GL.Color( Color.red );

		m_DebugMat.SetPass(0);

	    for(int i = 0; i < m_Values.Length - 1; i++ )
		{
			float normalizedPos = ( dist * i );
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
	        GL.Vertex( new Vector2( (  normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , ( m_Values[i].ScaleTo01( m_Range.x, m_Range.y ) * m_SizeInScreenSpace.y) + m_Offset.y ) );
			
			normalizedPos =  dist * ( i + 1 ) ;
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
			GL.Vertex( new Vector2( ( normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (  m_Values[i + 1].ScaleTo01( m_Range.x, m_Range.y ) * m_SizeInScreenSpace.y) + m_Offset.y ) );
	    }

	    GL.End();
	    GL.PopMatrix();
	}
}
