using FNAYB;
using Godot;

public partial class Camera : Node2D
{
	[Signal] public delegate void CameraOpenedEventHandler(bool val);
	
	// this should handle the entire camera plz zlp p zllpz pl zplz lp zz  zlz lp lp zpl zl zplp zplz 
	private bool _cameraOpen = false;
	public FNAFAI AI;

	[Export] private Sprite2D mainCamera;
	[Export] private Sprite2D cameraPanel;
	[Export] private TextureButton openCameraButton;
	
	[Export] public float PanLimit = 640f;
	[Export] public float CornerThreshold = 0.5f;

	private float _currentPan = 0f;
	
	[Export] private Sprite2D officeBackground;

	public void toggleCamera()
	{
		if (AI.playerIsDead)
			return;
		
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
		{
			cameraPanel.Visible = false;
			mainCamera.Visible = false;
			_cameraOpen = false;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AI = GetNode<FNAFAI>("/root/MainAI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float normalized = Mathf.Abs(officeBackground.Position.X) / PanLimit;
		GD.Print(normalized);
		
		openCameraButton.MouseFilter = normalized >= CornerThreshold 
			? Control.MouseFilterEnum.Stop
			: Control.MouseFilterEnum.Ignore;
		
		openCameraButton.Modulate = normalized >= CornerThreshold
			? new Color(1, 1, 1, 1)  
			: new Color(1, 1, 1, 0f);
	}
}
