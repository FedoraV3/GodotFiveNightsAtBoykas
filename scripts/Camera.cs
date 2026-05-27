using FNAYB;
using Godot;

public partial class Camera : Node2D
{
	[Signal] public delegate void CameraOpenedEventHandler(bool val);
	
	// this should handle the entire camera plz zlp p zllpz pl zplz lp zz  zlz lp lp zpl zl zplp zplz 
	private bool _cameraOpen = false;
	public FNAFAI AI;
	
	public void toggleCamera()
	{
		if (AI.playerIsDead)
			return;
		
		var mainCamera = GetNode<Sprite2D>("MainCamera");
		var cameraPanel = GetNode<Sprite2D>("CameraPanel");
		var openCameraButton = GetNode<TextureButton>("OpenCameraButton");
		_cameraOpen = !_cameraOpen;
		
		if (_cameraOpen)
		{
			AI.canPlayerMoveCamera = false;
			cameraPanel.Visible = true;
			mainCamera.Visible = true;
			openCameraButton.SelfModulate = new Color(1.0f, 1.0f, 1.0f, 0.314f);
			AI.springtrapCanMoveInOffice = true;
		}
		else
		{
			AI.canPlayerMoveCamera = true;
			cameraPanel.Visible = false;
			mainCamera.Visible = false;
			openCameraButton.SelfModulate = new Color(1.0f, 1.0f, 1.0f);
			AI.springtrapCanMoveInOffice = false;
		}
	}
	
	public void onJumpscare()
	{
		if (_cameraOpen)
			toggleCamera();
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
