using Godot;
using FNAYB;

public partial class Restartpanel : Node2D
{
	// todo: fix me plz the hardcode on the singleton
	private FNAFAI AI;
	
	private bool isPanelOpen = false;
	
	public void toggleRestartPanel()
	{
		var Panel = GetNode<Sprite2D>("Panel");
		var RestartButton = GetNode<TextureButton>("RestartButton");
		isPanelOpen =  !isPanelOpen;
		
		if (isPanelOpen)
		{
			// Color(1.0, 1.0, 1.0, 0.235)
			RestartButton.SelfModulate = new Color(1.0f, 1.0f, 1.0f, 0.235f);
			Panel.Visible = true;
			AI.canPlayerMoveCamera = false;
			AI.springtrapCanMoveInOffice = true;
		}
		else
		{
			// Color(1.0, 1.0, 1.0, 0.667)
			RestartButton.SelfModulate = new Color(1.0f, 1.0f, 1.0f, 0.667f);
			Panel.Visible = false;
			AI.canPlayerMoveCamera = true;
			AI.springtrapCanMoveInOffice = false;
		}
	}
	
	public void onJumpscare()
	{
		if (isPanelOpen)
			toggleRestartPanel();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AI = GetNode<FNAFAI>("/root/MainAI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
