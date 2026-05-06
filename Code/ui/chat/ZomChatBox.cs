using Sandbox.UI;
using SWB.Shared;
using System;

namespace ZombieHorde;

public partial class ZomChatBox : Panel
{
	static ZomChatBox Current;

	public Panel Canvas { get; protected set; }
	public TextEntry Input { get; protected set; }

	public ZomChatBox()
	{
		Current = this;

		StyleSheet.Load( "/resource/styles/_zomchatbox.scss" );

		Canvas = AddChild<Panel>();
		Canvas.AddClass( "chat_canvas" );

		Input = AddChild<TextEntry>();
		Input.AddEventListener( "onsubmit", () => Submit() );
		Input.AddEventListener( "onblur", () => Close() );
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;
	}

	void Open()
	{
		AddClass( "open" );
		Input.Focus();
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();
	}

	void Submit()
	{
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Sandbox.Input.Pressed( InputButtonHelper.Chat ) )
		{
			Open();
		}
	}

	public void AddEntry( string name, string message, string avatar, string color = null, string lobbyState = null )
	{
		var e = Canvas.AddChild<ZomChatEntry>();

		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
		if ( color != null )
			e.NameLabel.Style.FontColor = color;

		if ( lobbyState == "ready" || lobbyState == "staging" )
		{
			e.SetClass( "is-lobby", true );
		}
	}

	[ConCmd( "client" )]
	public static void AddChatEntry( string name, string message, string avatar = null, string color = null, string lobbyState = null )
	{
		Current?.AddEntry( name, message, avatar, color, lobbyState );

		if ( !Networking.IsHost )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ConCmd( "client" )]
	public static void AddInformation( string message, string avatar = null )
	{
		Current?.AddEntry( message, null, avatar );
	}

	[ConCmd( "server" )]
	public static void Say( string message )
	{
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		var players = Game.ActiveScene.GetAllComponents<HumanPlayer>().ToList();
		if ( players.Count == 0 ) return;

		// Find the calling player via connection
		HumanPlayer player = null;
		// ConsoleSystem.Caller removed in new API
			player = players.FirstOrDefault();
			//player = callerPlayer; // removed

		var color = "#7DFF8A";
		if ( player != null )
		{
			if ( !player.IsAlive )
			{
				if ( player.Health <= 0 )
				{
					color = "#90A4A6";
				}
				else
				{
					color = "#FF0000";
					if ( player.Health / player.MaxHealth <= .8f ) color = "#BD0000";
					if ( player.Health / player.MaxHealth <= .5f ) color = "#9C0000";
					if ( player.Health / player.MaxHealth <= .2f ) color = "#800000";
				}
			}
			else
			{
				if ( player.Health / player.MaxHealth <= .8f ) color = "#FFFF8E";
				if ( player.Health / player.MaxHealth <= .5f ) color = "#FFC68B";
				if ( player.Health / player.MaxHealth <= .2f ) color = "#FF8588";
			}
		}

		Log.Info( $"{player?.DisplayName ?? "Unknown"}: {message}" );
		AddChatEntry( player?.DisplayName ?? "Unknown", message, $"avatar:{player?.Network?.Owner?.SteamId}", color );
	}
}
