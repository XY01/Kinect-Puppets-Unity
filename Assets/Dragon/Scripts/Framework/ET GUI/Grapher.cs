using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grapher : MonoBehaviour
{
	 // Static singleton property
    static Grapher m_Instance { get; set; }
 
    // Static singleton property
    public static Grapher Instance
    {
        get 
		{			
			return m_Instance; 
		}
    }
	
	Material m_Mat;
	public Color m_Col;
	
	float[] m_FloatsRed = new float[ 200 ];
	float[] m_FloatsBlue = new float[ 200 ];
	float[] m_FloatsYellow = new float[ 200 ];
	
	int m_MaxLength = 10;
	
	public Vector2 m_SizeInPixels = new Vector2( 400, 200 );
	Vector2 m_SizeInScreenSpace = new Vector2( .1f, .05f );
	
	public Vector2 m_Offset;
	
	public bool m_Draw = true;
	
	void Awake()
	{
		m_Instance = this;
		
		 m_Mat = new Material(
			"Shader \"Lines/Wireframe\" { Properties { _Color (\"Main Color\", Color) = (0,0,0,1) } SubShader { Pass { " + 
			"ZWrite off " + 
			"ZTest LEqual " +
			"Blend SrcAlpha OneMinusSrcAlpha " + 
			"Lighting Off " +
			"Offset -1, -1 " +
			"Color[_Color] }}}");
	}
	
	public void AddFloatRed( float val )
	{
		m_SizeInScreenSpace.x = m_SizeInPixels.x / Screen.width;
		m_SizeInScreenSpace.y = m_SizeInPixels.y / Screen.height;
		
		for( int p = m_FloatsRed.Length-1; p > 0; p-- )
			m_FloatsRed[ p ] = m_FloatsRed[ p-1 ];
		
		m_FloatsRed[ 0 ] = val;
	}
	
	public void AddFloatBlue( float val )
	{
		m_SizeInScreenSpace.x = m_SizeInPixels.x / Screen.width;
		m_SizeInScreenSpace.y = m_SizeInPixels.y / Screen.height;
		
		for( int p = m_FloatsBlue.Length-1; p > 0; p-- )
			m_FloatsBlue[ p ] = m_FloatsBlue[ p-1 ];
		
		m_FloatsBlue[ 0 ] = val;
	}
	
	public void AddFloatYellow( float val )
	{
		m_SizeInScreenSpace.x = m_SizeInPixels.x / Screen.width;
		m_SizeInScreenSpace.y = m_SizeInPixels.y / Screen.height;
		
		for( int p = m_FloatsYellow.Length-1; p > 0; p-- )
			m_FloatsYellow[ p ] = m_FloatsYellow[ p-1 ];
		
		m_FloatsYellow[ 0 ] = val;
	}
	
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Home ) )
			m_Draw = !m_Draw;
		
		/*
		m_Floats.Add( Random.Range( 0f, 1f ) );
		
		if( m_Floats.Count >= m_MaxLength - 1 )
		{
			int extraNodes = m_Floats.Count - m_MaxLength;
			print ( extraNodes );
			for( int i = 0; i < extraNodes; i++ )
			{
				print ("                " +  m_Floats.Count );
				m_Floats.Remove( 0 );
				print ( m_Floats.Count );
			}				
		}				
		*/
		
	}
	
	public bool m_LogGraph = true;
	void OnRenderObject()
	{   
		if( !m_Draw ) return;
		
	    GL.PushMatrix();
		GL.LoadOrtho ();
		
	    GL.Begin(GL.LINES);
		m_Mat.SetPass(0);
	    GL.Color( Color.black );
		
		float dist = 1f / m_FloatsRed.Length;
		
		GL.Vertex( new Vector2( (  0 * m_SizeInScreenSpace.x ) + m_Offset.x , (1 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		GL.Vertex( new Vector2( (  1 * m_SizeInScreenSpace.x ) + m_Offset.x , (1 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		
		GL.Vertex( new Vector2( (  0 * m_SizeInScreenSpace.x ) + m_Offset.x , (0 * m_SizeInScreenSpace.y) + m_Offset.y ) );
		GL.Vertex( new Vector2( (  1 * m_SizeInScreenSpace.x ) + m_Offset.x , (0 * m_SizeInScreenSpace.y) + m_Offset.y ) );
	
		GL.Color( Color.red );
	    for(int i = 0; i < m_FloatsRed.Length - 1; i++ )
		{
			float normalizedPos = ( dist * i );
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
	        GL.Vertex( new Vector2( (  normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsRed[i] * m_SizeInScreenSpace.y) + m_Offset.y ) );
			
			normalizedPos =  dist * ( i + 1 ) ;
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
			GL.Vertex( new Vector2( ( normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsRed[i + 1] * m_SizeInScreenSpace.y) + m_Offset.y ) );
	    }
		
		GL.Color( Color.blue );
	    for(int i = 0; i < m_FloatsBlue.Length - 1; i++ )
		{
			float normalizedPos = ( dist * i );
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
	        GL.Vertex( new Vector2( (  normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsBlue[i] * m_SizeInScreenSpace.y) + m_Offset.y ) );
			
			normalizedPos =  dist * ( i + 1 ) ;
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
			GL.Vertex( new Vector2( ( normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsBlue[i + 1] * m_SizeInScreenSpace.y) + m_Offset.y ) );
	    }
		
		GL.Color( Color.yellow );
	    for(int i = 0; i < m_FloatsYellow.Length - 1; i++ )
		{
			float normalizedPos = ( dist * i );
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
	        GL.Vertex( new Vector2( (  normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsYellow[i] * m_SizeInScreenSpace.y) + m_Offset.y ) );
			
			normalizedPos =  dist * ( i + 1 ) ;
			if( m_LogGraph )
			{
				normalizedPos = Mathf.Sqrt( normalizedPos );
			}
			
			GL.Vertex( new Vector2( ( normalizedPos * m_SizeInScreenSpace.x ) + m_Offset.x , (m_FloatsYellow[i + 1] * m_SizeInScreenSpace.y) + m_Offset.y ) );
	    }
	
	    GL.End();
	    GL.PopMatrix();
		
	}
	
	
}
