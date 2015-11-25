using UnityEngine;
using System.Collections;

public class ET_Utils 
{	
  	public static Vector3 GetPointOnXYPlane( Vector2 pos, float AtZ ) //Move to utilities
    {
        //Get point at Y on ray
        Ray ray = Camera.main.ScreenPointToRay(pos);
        Vector3 MouseOnXYPlane = ray.GetPoint( (0 + AtZ - ray.origin.z ) / ray.direction.z );
        return MouseOnXYPlane;
    }
	

	
	#region MATHS HELPERS
	public static Vector2 CartesianToPolar( Vector3 point )
	{
	    Vector2 polar;
	
	    //calc longitude
	    polar.y = Mathf.Atan2(point.x,point.z);
	
	    //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
	    float xzLen = new Vector2(point.x,point.z).magnitude; 
	    //atan2 does the magic
	    polar.x = Mathf.Atan2(-point.y,xzLen);
	
	    //convert to deg
	    polar *= Mathf.Rad2Deg;
	
	    return polar;
	}
	
	public static Vector3 CartesianToUnitSphere( Vector3 point )
	{
	    Vector2 polar;
	
	    //calc longitude
	    polar.y = Mathf.Atan2(point.x,point.z);
	
	    //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
	    float xzLen = new Vector2(point.x,point.z).magnitude; 
	    //atan2 does the magic
	    polar.x = Mathf.Atan2(-point.y,xzLen);
	
	    //convert to deg
	    polar *= Mathf.Rad2Deg;
	
	    Quaternion rotation = Quaternion.Euler( polar.x * 2, polar.y, 0);
		
		return  rotation * Vector3.up ;
	}


	public static Vector3 PolarToCartesian( Vector2 polar )
	{
	
	    //an origin vector, representing lat,lon of 0,0. 
	
	    Vector3 origin= new Vector3(0,0,1);
	    //build a quaternion using euler angles for lat,lon
	    Quaternion rotation = Quaternion.Euler(polar.x*2,polar.y,0);
	    //transform our reference vector by the rotation. Easy-peasy!
	    Vector3 point=rotation*origin;
	
	    return point;
	}
	
	public static Vector2 GetPointAroundCircle( float angle )
	{
		Vector2 pos = Vector3.zero;
		angle *= Mathf.Deg2Rad;
		pos.x = Mathf.Sin( angle );
		pos.y = Mathf.Cos( angle );
		
		return pos;
	}
	
	public static Vector2 GetPointAroundCircle( int pointIndex, int totalPointsInCircle )
	{
		float angleBetweenDivisions = 360f / (float)totalPointsInCircle;
		float angle = pointIndex * angleBetweenDivisions;
		
		return GetPointAroundCircle( angle );
	}
	
	public static Vector3 GetTransformPointAroundCircle( Transform t, int pointIndex, int totalPointsInCircle )
	{
		return t.TransformPoint( GetPointAroundCircle( pointIndex, totalPointsInCircle ) );
	}
#endregion
	
	
}
