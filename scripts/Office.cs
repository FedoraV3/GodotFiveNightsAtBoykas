using Godot;
using System;
using FNAYB;

// min -360
// max 360

/* Springtrap stage in office:
	0 = at your window
	1 = ran to the corner (point of no return)
	2 = staring at you
	3 = if in cameras or opened restart panel kill you
*/

// 2.10 jumpscare end

public partial class Office : Camera2D
{
	[Signal] public delegate void SpringtrapJumpscareEventHandler();
	
	[Export] public float PanSpeed      = 1200f;
	[Export] public float EdgeZone      = 0.20f;
	[Export] public float SmoothFactor  = 10f;
	
	[Export] public float BackgroundHalfWidth = 1000f;
	
	[Export] public float PanLimit = 640f;
	
	private float _targetX;
	private Vector2 _screenSize;

	private FNAFAI AI;
	
	[Export] private Sprite2D BoykaPeeking;
	[Export] private Sprite2D BoykaJumpscare;
	[Export] private Sprite2D Boyka;
	[Export] private Sprite2D OfficeBackground;
	
	// This script should handle springtrap while he is in the office whilst another one will handle his positions in the cameras using events they will communicate with each other whether springtrap is in the cameras or office
	public async void _springTrapMovedInOffice(int stage)
	{
		try
		{
			switch (stage)
			{
				case 0:
					Boyka.Visible = false;
					BoykaPeeking.Visible = false;
					break;
				case 1:
					Boyka.Visible = true;
					break;
				case 2:
					Tween runningTween = GetTree().CreateTween();
					runningTween.TweenProperty(Boyka, "offset",
						new Vector2(-1000.0f, 0.0f), 0.3);
					break;
				case 3:
					BoykaPeeking.Visible = true;
					break;
				case 4:
					// Force the player camera to move
					EmitSignal(SignalName.SpringtrapJumpscare);
					AI.canPlayerMoveCamera = false;
					BoykaPeeking.Visible = false;
					BoykaJumpscare.Visible = true;

					Tween jumpscareTween = GetTree().CreateTween();
					jumpscareTween.TweenProperty(OfficeBackground, "position",
						new Vector2(360f, 0f), 0.5);
					_targetX = -360f;
					
					// play jumpscare sound then wait 2s and then reload the scene
					var jumpscare = GetNode<AudioStreamPlayer>("JumpscareSound");

					jumpscare.Play();

					await ToSignal(GetTree().CreateTimer(4.1), SceneTreeTimer.SignalName.Timeout);

					jumpscare.Stop();
					break;
			}
		}
		catch (Exception e)
		{
			GD.Print("ERR ------------------------------------------------------------------------------");
												GD.PrintErr(e.Message);
												GD.Print(e.StackTrace);
												GD.Print(e.Source);
			GD.Print("ERR ------------------------------------------------------------------------------");
		}
	}
	
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_targetX = 0f;
		Input.MouseMode = Input.MouseModeEnum.Confined;
		
		AI = GetNode<FNAFAI>("/root/MainAI");
		AI.springtrapMovedInOffice += _springTrapMovedInOffice;
	}

	public override void _Process(double delta)
	{
		if (AI.canPlayerMoveCamera && !AI.playerIsDead)
		{
			float screenW = GetViewportRect().Size.X;
			float mouseX = GetViewport().GetMousePosition().X;
			float leftEdge = screenW * EdgeZone;
			float rightEdge = screenW * (1f - EdgeZone);

			if (mouseX < leftEdge)
			{
				float strength = 1f - (mouseX / leftEdge);
				_targetX -= PanSpeed * strength * (float)delta;
			}
			else if (mouseX > rightEdge)
			{
				float strength = (mouseX - rightEdge) / (screenW - rightEdge);
				_targetX += PanSpeed * strength * (float)delta;
			}

			_targetX = Mathf.Clamp(_targetX, -360f, 360f);
		}

		var bg =  OfficeBackground;
		bg.Position = new Vector2(
			Mathf.Lerp(bg.Position.X, -_targetX, SmoothFactor * (float)delta),
			bg.Position.Y
		);
	}
}
