namespace ZombieHorde;

/// <summary>
/// Prevents Zombies/Lootboxes from spawning OR enables spawning within direct sight of players.
/// </summary>
[Group( "ZombieHorde" )]
[Title( "Spawn Blocker" )]
public partial class HammerSpawnBlocker : Component
{
	/// <summary>
	/// Whether this entity is enabled or not.
	/// </summary>
	[Property]
	public bool Enabled { get; protected set; } = true;

	[Property, Title( "Spawn Type" )]
	public BlockType BlockType { get; set; } = BlockType.BlockSpawning;

	[Property]
	public bool AffectsCommonZombies { get; set; } = true;

	[Property]
	public bool AffectsSpecialZombies { get; set; } = true;

	[Property]
	public bool AffectsLootBoxes { get; set; } = false;

	protected override void OnStart()
	{
		SetupTags();

		// Set up collision via collider component (created by Hammer)
		var collider = Components.Get<Collider>();
		if ( collider != null )
		{
			collider.Enabled = true;
		}

		// Disable shadows on model renderer
		var renderer = Components.Get<ModelRenderer>();
		if ( renderer != null )
		{
			renderer.Enabled = false;
		}
	}

	public void SetupTags()
	{
		GameObject.Tags.RemoveAll();

		GameObject.Tags.Add( "trigger" );

		if ( BlockType == BlockType.BlockSpawning )
		{
			if ( AffectsCommonZombies )
				GameObject.Tags.Add( "BlockCommonZombieSpawn" );
			if ( AffectsSpecialZombies )
				GameObject.Tags.Add( "BlockSpecialZombieSpawn" );
			if ( AffectsLootBoxes )
				GameObject.Tags.Add( "BlockLootBoxSpawn" );
		}
		else if ( BlockType == BlockType.AllowSpawningRegardlessOfVision )
		{
			if ( AffectsCommonZombies )
				GameObject.Tags.Add( "AllowCommonZombieSpawn" );
			if ( AffectsSpecialZombies )
				GameObject.Tags.Add( "AllowSpecialZombieSpawn" );
			if ( AffectsLootBoxes )
				GameObject.Tags.Add( "AllowLootBoxSpawn" );
		}
	}

	/// <summary>
	/// Enables this trigger
	/// </summary>
	[Input]
	public void Enable()
	{
		Enabled = true;
	}

	/// <summary>
	/// Disables this trigger
	/// </summary>
	[Input]
	public void Disable()
	{
		Enabled = false;
	}
}

public enum BlockType
{
	BlockSpawning,
	AllowSpawningRegardlessOfVision
}

public enum AffectsType
{
	CommonZombies,
	SpecialZombies,
	LootBoxes
}
