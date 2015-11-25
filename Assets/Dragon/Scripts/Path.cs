using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Path.
/// - A list of transforms that can be shortened, lengthened, and traversed
/// </summary>

public class Path : MonoBehaviour
{
	// A list of path nodes and a public property
	List< Path_Node > 			m_PathNodes = new List<Path_Node>();
	public List< Path_Node > 	PathNodes { get{ return m_PathNodes; } }

	// The length of the path
	float 						m_Length;
	public float 				Length { get{ return m_Length; } }

	// Flag for drawing the path gizmos in editor
	public bool 				m_DrawGizmos = false;


	// Adds a node at the end of the path
	public virtual void AddNodeAtEnd( Path_Node pathNode )
	{
		// parent the path node to this path
		pathNode.transform.parent = transform;

		// Add node to the list
		m_PathNodes.Add( pathNode );

		// Recalculate the length of the path
		CalculateLength();
	}

	// Adds a node at the end of the path based on an existing transform
	public virtual void AddNodeAtEnd( Transform t )
	{
		// Creates a new path node object
		Path_Node newNode = new GameObject("Node " + ( m_PathNodes.Count ) ).AddComponent< Path_Node >();

		// parent the path node to this path
		newNode.transform.parent = transform;

		// Copies the transform properties
		newNode.transform.CopyTransform( t );

		// Add node to the list
		m_PathNodes.Add( newNode );

		// Recalculate the length of the path
		CalculateLength();
	}

	// Adds a path node at the start of the path
	public virtual void AddNodeAtStart( Path_Node pathNode )
	{
		// Adds path node to list
		m_PathNodes.Insert( 0, pathNode );

		// Recalculate the length of the path
		CalculateLength();
	}

	// Removes path node at a particular index
	public void RemoveNode( int index )
	{
		// returns if the index is out of range or the list size is 0
		if( index > m_PathNodes.Count - 1 )  	return;
		if( m_PathNodes[ index ] == null ) 		return;

		// Removes node from list
		m_PathNodes.RemoveAt( index );
	}	
	
	// Gets the worlds position given a normalized lenght along the path
	public Vector3 GetPosAtNormLength( float normLength )
	{
		return GetPosAtLength( normLength * Length );
	}

	// TODO: Test, looks like the iterator shouldnt keep going once it is found
	public Vector3 GetPosAtLength( float length )
	{		
		// initialize return value
		Vector3 pos = Vector3.one;

		// clamp the length to the length of the path
		length = Mathf.Clamp( length, 0, m_Length );

		// Look through each node...
		for( int i = 0; i < m_PathNodes.Count; i++ )
		{
			// Until a node is larger than the length we are looking for
			if( m_PathNodes[ i ].m_RawLengthAlongPath > length )
			{
				// Store the length of the previous node
				float prevNodeLength = m_PathNodes[ i - 1 ].m_RawLengthAlongPath;

				// Store the length of this node
				float nextNodeLength = m_PathNodes[ i ].m_RawLengthAlongPath;

				// Find the difference between the desired length and the previous node length
				float lengthDifference = length - prevNodeLength;

				// Find the length difference between this node and the previous node
				float distanceBetweenNodes = nextNodeLength - prevNodeLength;	

				float interpolationAmount = Mathf.Lerp( 0, distanceBetweenNodes, lengthDifference / distanceBetweenNodes );

				pos = m_PathNodes[ i - 1 ].transform.position +
						( Vector3.Lerp( m_PathNodes[ i - 1 ].transform.position, m_PathNodes[ i ].transform.position, interpolationAmount ) );				
			}
		}
		
		return pos;
	}


	void ShortenToLength( float target )
	{
		float distanceToRemove = m_Length - target;			
		int nodeCountToRemove = 0;
		for( int i = 0; i < m_PathNodes.Count - 2; i++ )
		{
			float distanceBetweenNodes = Vector3.Distance( m_PathNodes[i].transform.position, m_PathNodes[i+1].transform.position );
			
			if( distanceBetweenNodes < distanceToRemove )
			{
				distanceToRemove -= distanceBetweenNodes;
				nodeCountToRemove++;
			}
			else
			{
				distanceToRemove -= MoveNodeToward( i, i + 1, distanceToRemove );
			}
		}
		
		for( int i = 0; i < nodeCountToRemove; i++ )
		{
			RemoveNode( 0 );
		}
		
		CalculateLength();
	}
	
	float MoveNodeToward( int node, int toward, float distanceToMove )
	{
		float distanceBetweenNodes = Vector3.Distance( m_PathNodes[ node ].transform.position, m_PathNodes[ toward ].transform.position );
		
		if( distanceBetweenNodes > distanceToMove )
		{
			m_PathNodes[ node ].transform.position =	Vector3.Lerp( m_PathNodes[ node ].transform.position , m_PathNodes[ toward ].transform.position , distanceToMove/distanceBetweenNodes );
			
			return distanceBetweenNodes - distanceToMove;
		}
		
		CalculateLength();
		
		return 0;
	}
	
	protected virtual void ShortenFromEndToLength( float targetLength )
	{
		
	}
	
	protected virtual void ShortenFromStartToLength( float targetLength )
	{
		
	}
	
	void CalculateLength()
	{
		float tempLength = 0;
		if( m_PathNodes.Count == 0 )
		{
			m_Length = 0; 
		}
		else
		{
			for( int i = 1; i < m_PathNodes.Count; i++ )
			{
				tempLength += Vector3.Distance( m_PathNodes[ i ].transform.position, m_PathNodes[ i - 1 ].transform.position );
				m_PathNodes[ i ].m_RawLengthAlongPath = tempLength;
			}
		}
		
		m_Length = tempLength;
	}
	
	public void Reset()
	{
		transform.DestroyAllChildren();
		m_PathNodes.Clear();
	}	
	
	public void Clear()
	{
		m_PathNodes.Clear();
	}	
	
	public void ProjectOnToPath( Path path, int nodeCount, float normStartLength,  float normEndLength )
	{
		if( nodeCount < 2 ) return;
		
		int nodeCountDifference = nodeCount - m_PathNodes.Count;
		
		if( nodeCountDifference > 0 )
		{
			for( int i = 0; i < nodeCountDifference; i++ )
			{
				AddNodeAtEnd( transform );
			}
		}
		else
		{
			for( int i = 0; i < Mathf.Abs( nodeCountDifference ); i++ )
			{
				m_PathNodes.RemoveAt(0);
			}
		}
		
		float normalizedRange = normEndLength - normStartLength;
		
		for( int i = 0; i < m_PathNodes.Count; i++ )
		{
			float normLength = normStartLength + ( ( (float)i / ( m_PathNodes.Count - 1 ) ) * normalizedRange );
			//float normLength = .5f;
			m_PathNodes[ i ].transform.position = path.GetPosAtNormLength( normLength );
		}
		
		CalculateLength();
	}
	
	public float test = .1f;
	void OnDrawGizmos()
	{
		if( m_DrawGizmos )
		{			
			Gizmos.color = Color.yellow;
			
			Gizmos.DrawSphere( GetPosAtNormLength(test) ,   .2f );
			
			for( int i = 1; i < m_PathNodes.Count; i++ )
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine( m_PathNodes[i].transform.position,  m_PathNodes[i - 1].transform.position ); 
				Gizmos.color = Color.gray;
				Gizmos.DrawWireSphere( m_PathNodes[i].transform.position,  m_PathNodes[i].transform.localScale.magnitude * .1f );
			}
		}
	}
	
	
}
