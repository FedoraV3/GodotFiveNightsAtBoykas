using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FNAYB;

public partial class FNAFAI : Node
{
	[Signal] public delegate void springtrapMovedEventHandler();
	[Signal] public delegate void springtrapMovedInOfficeEventHandler(int stage);
	
	public int AILevel = 20;
	
	public enum SPRINGTRAP_POSITIONS
	{
		Cam01,
		Cam02,
		Cam03,
		Cam04,
		Cam05,
		Cam06,
		Cam07,
		Cam08,
		Cam09,
		Cam10,
		OfficeWindow,
		OfficeDoor
	}
	
	private Dictionary<SPRINGTRAP_POSITIONS, List<SPRINGTRAP_POSITIONS>> cameraGraph = new()
	{
		{ SPRINGTRAP_POSITIONS.Cam01, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam02, SPRINGTRAP_POSITIONS.Cam06, SPRINGTRAP_POSITIONS.OfficeDoor } },
		{ SPRINGTRAP_POSITIONS.Cam02, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam01, SPRINGTRAP_POSITIONS.Cam03, SPRINGTRAP_POSITIONS.Cam05, SPRINGTRAP_POSITIONS.OfficeWindow } },
		{ SPRINGTRAP_POSITIONS.Cam03, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam02, SPRINGTRAP_POSITIONS.Cam04 } },
		{ SPRINGTRAP_POSITIONS.Cam04, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam03, SPRINGTRAP_POSITIONS.Cam10 } },
		{ SPRINGTRAP_POSITIONS.Cam10, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam04, SPRINGTRAP_POSITIONS.Cam09 } },
		{ SPRINGTRAP_POSITIONS.Cam09, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam10, SPRINGTRAP_POSITIONS.Cam08 } },
		{ SPRINGTRAP_POSITIONS.Cam08, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam09, SPRINGTRAP_POSITIONS.Cam07 } },
		{ SPRINGTRAP_POSITIONS.Cam07, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam08, SPRINGTRAP_POSITIONS.Cam06 } },
		{ SPRINGTRAP_POSITIONS.Cam06, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam07, SPRINGTRAP_POSITIONS.Cam05, SPRINGTRAP_POSITIONS.Cam01 } },
		{ SPRINGTRAP_POSITIONS.Cam05, new List<SPRINGTRAP_POSITIONS> { SPRINGTRAP_POSITIONS.Cam06, SPRINGTRAP_POSITIONS.Cam02 } },
		// game has to handle these
		{ SPRINGTRAP_POSITIONS.OfficeWindow, new List<SPRINGTRAP_POSITIONS> {} },
		{ SPRINGTRAP_POSITIONS.OfficeDoor, new List<SPRINGTRAP_POSITIONS> {} }
	};
	
	public SPRINGTRAP_POSITIONS springTrapPos = SPRINGTRAP_POSITIONS.Cam09;
	public SPRINGTRAP_POSITIONS oldspringTrapPos = SPRINGTRAP_POSITIONS.Cam09;

	public bool springtrapIsInOffice = false;
	public bool springtrapCanMoveInOffice = false;

	public bool canPlayerMoveCamera = true;
	
	public int springtrapStage = 0;

	public bool playerIsDead = false;
		
	private Random rng = new Random();
	private Timer aiTimer =  new Timer();
	
	private void aiOnTimeout()
	{
		if (!springtrapIsInOffice)
		{
			List<SPRINGTRAP_POSITIONS> neighbors = cameraGraph[springTrapPos];
			List<SPRINGTRAP_POSITIONS> filtered = neighbors.Where(c => c != oldspringTrapPos).ToList();
			List<SPRINGTRAP_POSITIONS> choices = filtered.Count > 0 ? filtered : neighbors;
	
			oldspringTrapPos = springTrapPos;
			springTrapPos = choices[rng.Next(choices.Count)];

			switch (springTrapPos)
			{
				case SPRINGTRAP_POSITIONS.OfficeWindow:
					springtrapStage = 1;
					springtrapIsInOffice = true;
					EmitSignal(SignalName.springtrapMovedInOffice, 1);
					break;
				case SPRINGTRAP_POSITIONS.OfficeDoor:
					springtrapStage = 3;
					springtrapIsInOffice = true;
					EmitSignal(SignalName.springtrapMovedInOffice, 3);
					break;
				default:
					EmitSignal(SignalName.springtrapMoved);
					break;
			}
			// debug
			GetNode<Label>("/root/Office/Label").Text = $"My position: {springTrapPos}";
		}
		else
		{
			if (springtrapCanMoveInOffice)
			{
				springtrapStage += 1;
				if (springtrapStage == 4)
				{
					playerIsDead = true;
				}
				EmitSignal(SignalName.springtrapMovedInOffice, springtrapStage);
				
			}
			// debug
			GetNode<Label>("/root/Office/Label").Text = $"I'm in your office";	
		}
	}
	
	public override void _Ready()
	{
		//setup me plz
		AddChild(aiTimer);
		aiTimer.Start();
		aiTimer.Autostart = true;
		aiTimer.WaitTime = 5.0;
		aiTimer.OneShot = false;
		aiTimer.Timeout += aiOnTimeout;
	}

	public override void _Process(double delta)
	{
				
	}
}
