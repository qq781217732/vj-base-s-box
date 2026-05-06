using Sandbox.UI;

public class InventoryIcon : Panel
{
	public GameObject TargetEnt;
	public Label Bullets;
	public Label BulletReserve;
	public Image Icon;
	public Label RarityBar;
	public Image Glyph;

	public InventoryIcon( int i, Panel parent )
	{
		Parent = parent;

		Glyph = AddChild<Image>();
		Glyph.AddClass( "glyph" );

		Bullets = AddChild<Label>();
		Bullets.Text = "?/?";
		Bullets.AddClass( "ammo-count" );

		BulletReserve = AddChild<Label>();
		BulletReserve.Text = "?/?";
		BulletReserve.AddClass( "ammo-reserve" );

		RarityBar = AddChild<Label>();
		RarityBar.AddClass( "right-bar" );

		Icon = AddChild<Image>();
		Icon.AddClass( "icon" );
	}

	public void Clear()
	{
		Bullets.Text = "";
		BulletReserve.Text = "";
		SetClass( "active", false );
		Icon.Texture = null;
	}
}
