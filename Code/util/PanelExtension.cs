namespace Sandbox.UI
{
	public static class PanelExtension
	{
		public static void PositionAtCrosshair( this Panel panel )
		{
			var localPlayer = Game.ActiveScene.GetAllComponents<ZombieHorde.HumanPlayer>()
				.FirstOrDefault( p => !p.IsProxy );
			PositionAtCrosshair( panel, localPlayer );
		}

		public static void PositionAtCrosshair( this Panel panel, ZombieHorde.HumanPlayer player )
		{
			if ( player is null || !player.IsValid() ) return;

			var eyePos = player.EyePos;
			var forward = player.EyeAngles.ToRotation().Forward;

			var tr = Game.ActiveScene.Trace.Ray( eyePos, eyePos + forward * 2000 )
				.IgnoreGameObjectHierarchy( player.GameObject )
				.Radius( 1.0f )
				.Run();

			panel.PositionAtWorld( tr.EndPosition );
		}

		public static void PositionAtWorld( this Panel panel, Vector3 pos )
		{
			var cam = panel.Scene?.Camera;
			if ( cam == null ) return;

			var screenPos = cam.PointToScreenPixels( pos );

			panel.Style.Left = Length.Fraction( screenPos.x );
			panel.Style.Top = Length.Fraction( screenPos.y );
			panel.Style.Dirty();
		}
	}
}
