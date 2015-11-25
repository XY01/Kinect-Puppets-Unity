using UnityEngine;
using System.Collections;

public static class ColorExtensions
{
	public static Color SetColourWithoutAlpha( this Color col, Color newCol )
	{
		col.r = newCol.r;
		col.g = newCol.g;
		col.b = newCol.b;

		return col;
	}

	public static Color SetAlpha( this Color col, float alpha )
	{
		col.a = alpha;
		return col;
	}

	public static Color SetHue( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		temphsbCol.h = val;

		return temphsbCol.ToColor();
	}

	public static Color SetSaturation( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		temphsbCol.s = val;
		
		return temphsbCol.ToColor();
	}

	public static Color SetBright( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		temphsbCol.b = val;
		
		return temphsbCol.ToColor();
	}

	public static float GetHue( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		return temphsbCol.h;
	}
	
	public static float GetSaturation( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		return temphsbCol.s;
	}
	
	public static float GetBright( this Color col, float val )
	{
		HSBColor temphsbCol = HSBColor.FromColor( col );
		return temphsbCol.b;
	}

	public static Color LerpTriple( Color col1, Color col2, Color col3, float value )
	{
		if( value < .5f )
		{
			return Color.Lerp( col1, col2, value * 2 );
		}
		else
		{
			return Color.Lerp( col2, col3, (value - .5f) * 2 );
		}
	}
}
