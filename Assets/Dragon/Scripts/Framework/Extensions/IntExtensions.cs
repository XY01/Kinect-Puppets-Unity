using UnityEngine;
using System.Collections;

public static class IntExtensions
{	
	public static int WrapIntToRange( this int f, int min, int max )
	{		
		if( f < min )
			return max;
		else if( f > max )
			return min;
		else
			return f;
	}	
	
	public static void WrapIntToRangeRef( this int f, ref int i, int min, int max )
	{		
		if( i < min )
			i = max;
		else if( i > max )
			i = min;
	}	
	
	public static int LoopIncrement( int index, int min, int max )
	{
		index++;
		return WrapIntToRange(  index, min, max );
	}
	
	public static int LoopDecrement( int index, int min, int max )
	{
		index--;
		return WrapIntToRange(  index, min, max );
	}
	
}
