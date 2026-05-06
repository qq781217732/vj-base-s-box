namespace ZombieHorde;

[Group( "ZombieHorde" )]
[Title( "Zom Debug Overlay" )]
public sealed class ZomDebugOverlay : Component
{
	bool showOverlay;

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "Home" ) )
			showOverlay = !showOverlay;

		if ( !showOverlay ) return;

		var zombies = Game.ActiveScene.GetAllComponents<BaseZombie>().ToList();
		var director = Game.ActiveScene.GetAllComponents<EncounterDirector>().FirstOrDefault();
		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().ToList();

		// Camera selection — match movement debug pattern
		var localPlayer = players.FirstOrDefault( p => !p.IsProxy && p.IsAlive );
		var cam = localPlayer is not null && localPlayer.IsFirstPerson
			? localPlayer.ViewModelCamera
			: localPlayer?.Camera ?? Scene.Camera;
		if ( cam is null ) return;
		var hud = cam.Hud;

		// 3D world text — each zombie
		foreach ( var z in zombies )
		{
			if ( !z.IsValid() || !z.IsAlive ) continue;

			var zoneName = director?.Zones
				.FirstOrDefault( zz => zz.IsPointInBounds( z.WorldPosition ) )?.ZoneName ?? "???";

			DebugOverlay.Text( z.WorldPosition + Vector3.Up * 48f,
				$"{z.Brain?.CurrentTask}\n{zoneName}",
				14f, TextFlag.Center, Color.Red, 0f, true );
		}

		// 2D panel — below movement debug, right side
		var x = Screen.Width - 420f;
		if ( x < 32f ) x = 32f;
		var y = 32f;
		var startLine = 10;
		var totalLines = 2 + (director?.Zones.Count ?? 0) + 1; // header + stats + blank + zones

		// Background rect
		var padX = 12f;
		var padY = 4f;
		hud.DrawRect( new Rect()
		{
			Position = new Vector2( x - padX, y + startLine * 16f - padY ),
			Size = new Vector2( 400f, totalLines * 16f + padY * 2 )
		}, Color.Black.WithAlpha( 0.5f ) );

		// Text
		var line = startLine;
		hud.DrawText( "=== ZOM DEBUG ===", 14f, Color.White, new Vector2( x, y + line++ * 16f ) );
		hud.DrawText( $"Zombies: {zombies.Count} | Pressure: {director?.GlobalPressure:F1}", 14f, Color.White, new Vector2( x, y + line++ * 16f ) );
		line++;

		if ( director is not null )
		{
			foreach ( var zone in director.Zones )
			{
				hud.DrawText(
					$"[{zone.State}] {zone.ZoneName}: {zone.CurrentEnemyCount}/{zone.AmbientBudget} alert={zone.AlertLevel:F1}",
					14f, Color.White.WithAlpha( 0.7f ), new Vector2( x, y + line++ * 16f ) );
			}
		}
	}
}
