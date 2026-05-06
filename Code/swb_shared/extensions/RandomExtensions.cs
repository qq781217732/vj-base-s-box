using System;

namespace SWB.Shared;

public static class RandomExtensions
{
	public static float Float( this Random random ) => random.NextSingle();
	public static float NextFloat( this Random random ) => random.NextSingle();
	public static float NextFloat( this Random random, float maxValue ) => random.NextSingle() * maxValue;
	public static float NextFloat( this Random random, float minValue, float maxValue ) => random.NextSingle() * (maxValue - minValue) + minValue;
	public static int NextInt( this Random random, int maxValue ) => random.Next( maxValue );
	public static int NextInt( this Random random, int minValue, int maxValue ) => random.Next( minValue, maxValue );
}
