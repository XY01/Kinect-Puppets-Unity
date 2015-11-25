using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
	
	public static Vector3 ScaleReturn( this Vector3 v1, Vector3 v2 )
	{
		Vector3 scaledVec = v1;
		scaledVec.Scale( v2 );
		return scaledVec;
	}
	
	public static Vector3 Divide( this Vector3 v1, Vector3 v2 )
	{
		Vector3 dividedVec = Vector3.zero;
		dividedVec.x = v1.x / v2.x;
		dividedVec.x = v1.y / v2.y;
		dividedVec.x = v1.z / v2.z;
		
		return dividedVec;
	}
	
	public static Vector3 RangedToNormalized( this Vector3 v, float min, float max )
	{
		//print( "input:   " + v.x + "   " + min + "   " + max );
		v.x = Utils.RangedToNormalized( v.x, min, max);
		
		//print( "result:   " + v.x + "   " + min + "   " + max );
		v.y = Utils.RangedToNormalized( v.y, min, max);
		v.z = Utils.RangedToNormalized( v.z, min, max);
		
		return v;
	}
	
	public static Vector3 RangedToUnitLegnth( this Vector3 v, float min, float max )
	{
		v.x = Utils.RangedToUnitLength( v.x, min, max);
		v.y = Utils.RangedToUnitLength( v.y, min, max);
		v.z = Utils.RangedToUnitLength( v.z, min, max);
		
		return v;
	}
	
	public static Vector3 NormalizedToRange( this Vector3 v, float min, float max )
	{
		v.x = Utils.NormalizedToRange( v.x, min, max);
		v.y = Utils.NormalizedToRange( v.y, min, max);
		v.z = Utils.NormalizedToRange( v.z, min, max);
		
		return v;
	}
	
	public static Vector3 Clamp( this Vector3 v, float min, float max )
	{
		v.x = Mathf.Clamp( v.x, min, max );
		v.y = Mathf.Clamp( v.y, min, max );
		v.z = Mathf.Clamp( v.z, min, max );
		
		return v;
	}
	
	public static Vector3 RestrictToRadius( this Vector3 v, Vector3 inputPos, float radius )
	{
		Vector3 restrictedPos = v;
		Vector3 direction = inputPos - v;
		restrictedPos = v + ( direction.normalized * radius );
		
		return restrictedPos;
	}
	
	public static Vector3 LerpVector3( this Vector3 v, Vector3 targVec, float t, float epsilon)
	{
		v.x = v.x.Lerp( targVec.x, t, epsilon );
		v.y = v.y.Lerp( targVec.y, t, epsilon );
		v.z = v.z.Lerp( targVec.z, t, epsilon );
		
		return v;
	}

	public static float SignedAngleBetweenVectors( this Vector3 A, Vector3 B )
	{
		float angle = 0;
		angle = Mathf.Atan2(A.x * B.y - A.y * B.x, A.x * B.x + A.y * B.y);
		return angle * Mathf.Rad2Deg;
	}

	public static float SignedAngleBetweenVectors( this Vector2 A, Vector2 B )
	{
		float angle = 0;
		angle = Mathf.Atan2(A.x * B.y - A.y * B.x, A.x * B.x + A.y * B.y);
		return angle * Mathf.Rad2Deg;
	}
	
	public static void Save( this Vector3 v, string uniqueID )
	{
		PlayerPrefsPlus.SetVector3( uniqueID, v );
	}
	
	public static void Load( this Vector3 v, string uniqueID )
	{
		v = PlayerPrefsPlus.GetVector3( uniqueID );
	}
}
