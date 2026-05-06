using Sandbox.UI;

namespace ZombieHorde
{
	public partial class ZomChatEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public ZomChatEntry()
		{
			Avatar = AddChild<Image>();
			NameLabel = AddChild<Label>();
			NameLabel.AddClass( "name" );
			Message = AddChild<Label>();
			Message.AddClass( "message" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( TimeSinceBorn > 12 )
			{
				Delete();
			}
		}
	}
}
