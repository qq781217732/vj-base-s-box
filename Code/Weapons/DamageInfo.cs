namespace ZombieHorde;

/// <summary>Helper to create Sandbox.DamageInfo with common presets.</summary>
public static class DamageInfoExt
{
	public static Sandbox.DamageInfo FromCustom( Vector3 sourcePosition, Vector3 force, float damage, string tag = "blast" )
	{
		var result = new Sandbox.DamageInfo
		{
			Position = sourcePosition,
			//Force = force, // Removed - Sandbox.DamageInfo no longer has Force
			Damage = damage,
		};
		result.Tags.Add( tag );
		return result;
	}

	public static Sandbox.DamageInfo FromBullet( Vector3 sourcePosition, Vector3 force, float damage )
	{
		return FromCustom( sourcePosition, force, damage, "bullet" );
	}

	public static Sandbox.DamageInfo Explosion( Vector3 sourcePosition, Vector3 force, float damage )
	{
		return FromCustom( sourcePosition, force, damage, "blast" );
	}
}
