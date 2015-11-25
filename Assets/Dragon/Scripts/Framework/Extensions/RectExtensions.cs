using UnityEngine;
using System.Collections;

public static class RectExtensions
{	
	public static Rect Snap( this Rect r1, Rect r2, float threshold )
	{
		if( Mathf.Abs( r1.xMin - r2.xMin ) < threshold )
			r1.xMin = r2.xMin;

		return r1;
	}

	public static Rect Copy( this Rect r1 )
	{
		Rect newRect = new Rect();

		newRect.x = r1.x;
		newRect.y = r1.y;
		newRect.width = r1.width;
		newRect.height= r1.height;

		return newRect;
	}
	
	public static Rect Clamp( this Rect r1, float widthBounds, float heightBounds )
	{
		r1.x = Mathf.Clamp(r1.x, 0, widthBounds - r1.width );
		
		r1.y = Mathf.Clamp(r1.y, 0, heightBounds - r1.height );
		
		return r1;
	}
	
}
