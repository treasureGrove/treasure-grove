using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class JoystickData{
	
	//Enum with values representing either the Horizontal or Vertical axis on the different sticks available
	public enum AxisTypes
	{
		LEFT_HORIZONTAL,
		LEFT_VERTICAL,
		RIGHT_HORIZONTAL,
		RIGHT_VERTICAL,
		D_HORIZONTAL,
		D_VERTICAL
	}


	// Enum representing the different buttons on the joystick, using the Xbox 360 controller as its base. In other words, the
	// "A" button is the bottom face button, which is the "X" button one the Play Station Joystick.
	public enum ButtonTypes
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		LeftBumper = 4,
		RightBumper = 5,
		Back = 6,
		Start = 7,
		LeftStickClick = 8,
		RightStickClick = 9,
		LeftDPadX = 10,
		RightDPadX = 11,
		TopDPadY = 12,
		BottomDPadY = 13,
		LeftTrigger = 14,
		RightTrigger = 15
	}

	//The default joystick configuration for the default register
	public static Dictionary<ButtonTypes, int> joystick_default = new Dictionary<ButtonTypes, int>()
	{
		{ ButtonTypes.A, 0 },
		{ ButtonTypes.B, 1 },
		{ ButtonTypes.X, 2 },
		{ ButtonTypes.Y, 3 },
		{ ButtonTypes.LeftBumper, 4 },
		{ ButtonTypes.RightBumper, 5 },
		{ ButtonTypes.Back, 6 },
		{ ButtonTypes.Start, 7 },
		{ ButtonTypes.LeftStickClick, 8 },
		{ ButtonTypes.RightStickClick, 9 },
		{ ButtonTypes.LeftDPadX, 10 },
		{ ButtonTypes.RightDPadX, 11 },
		{ ButtonTypes.TopDPadY, 12 },
		{ ButtonTypes.BottomDPadY, 13 },
		{ ButtonTypes.LeftTrigger, 14 },
		{ ButtonTypes.RightTrigger, 15 }
	};
		
	// The register for the "default" platform. If a joystick name or button type cannot be found in a platform specific register, this register will be used instead.
	public static Dictionary<string, Dictionary<ButtonTypes, int>> register_default = new Dictionary<string, Dictionary<ButtonTypes, int>>()
	{
		{ "default", JoystickData.joystick_default }
	};
		
	// The register used on Windows platforms.
	public static Dictionary<string, Dictionary<ButtonTypes, int>> register_windows = new Dictionary<string, Dictionary<ButtonTypes, int>>()
	{
		{ "default", JoystickData.joystick_default }
	};
		
	// The register used on OSX platforms
	public static Dictionary<string, Dictionary<ButtonTypes, int>> register_osx = new Dictionary<string, Dictionary<ButtonTypes, int>>()
	{

	};
		
	//  The register used on Linux platforms
	public static Dictionary<string, Dictionary<ButtonTypes, int>> register_linux = new Dictionary<string, Dictionary<ButtonTypes, int>>()
	{

	};
}
