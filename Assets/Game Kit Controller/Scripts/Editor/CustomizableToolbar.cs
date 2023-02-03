using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class CustomizableToolbar : EditorWindow, IHasCustomMenu
{
	private const string TITLE = "GKC Toolbar";
	private static float WINDOW_HEIGHT = 40;
	private const float BUTTON_HEIGHT = 20;

	private CustomizableToolbarSettings m_settings;

	public static CustomizableToolbar Instance { get; private set; }

	[MenuItem ("Game Kit Controller/Show GKC Toolbar", false, 502)]
	private static void Init ()
	{
		var win = GetWindow<CustomizableToolbar> (TITLE);

		var pos = win.position;
		pos.height = WINDOW_HEIGHT;
		win.position = pos;

		var minSize = win.minSize;
		minSize.y = WINDOW_HEIGHT;
		win.minSize = minSize;

		var maxSize = win.maxSize;
		maxSize.y = WINDOW_HEIGHT;
		win.maxSize = maxSize;
	}

	private void OnGUI ()
	{
		EditorGUILayout.BeginVertical ();

		var list = m_settings.List.Where (c => c.IsValid);

		int numberOfColumns = m_settings.numberOfColumns;

		int currentIndex = 0;

		int numberOfElements = m_settings.List.Count;

		foreach (var n in list) {
			if (currentIndex == 0) {
				EditorGUILayout.BeginHorizontal ();
			}

			var commandName = n.CommandName;
			var buttonName = n.ButtonName;
			var useImage = n.useImage;
			var image = n.Image;
			var width = n.Width;
			var hint = n.hint;

			var content = useImage ? new GUIContent (image, hint) : new GUIContent (buttonName, hint);

			var options = 0 < width
					? new [] { GUILayout.Width (width), GUILayout.Height (BUTTON_HEIGHT) }
					: new [] {
				GUILayout.Width (EditorStyles.label.CalcSize (new GUIContent (buttonName)).x + 14),
				GUILayout.Height (BUTTON_HEIGHT)
			};
		
			if (GUILayout.Button (content, options)) {
				string commandCheck = commandName;

				commandCheck = commandCheck.Replace (" ", "");
		
				if (commandCheck != "") {
					EditorApplication.ExecuteMenuItem (commandName);
				}
			}

			currentIndex++;

			if (currentIndex % numberOfColumns == 0 && currentIndex != numberOfElements) {
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
			}

			if (currentIndex == numberOfElements) {
				EditorGUILayout.EndHorizontal ();

			}

		}

		EditorGUILayout.EndVertical ();
	}

	private void OnEnable ()
	{
		var mono = MonoScript.FromScriptableObject (this);
		var scriptPath = AssetDatabase.GetAssetPath (mono);
		var dir = Path.GetDirectoryName (scriptPath);
		var path = string.Format ("{0}/Settings.asset", dir);

		m_settings = AssetDatabase.LoadAssetAtPath<CustomizableToolbarSettings> (path);

		WINDOW_HEIGHT = m_settings.windowsHeight;

		if (WINDOW_HEIGHT == 0) {
			WINDOW_HEIGHT = 30;
		}

		Instance = this;
	}

	public static void updateToolBarValues ()
	{
		if (Instance != null) {
			Instance.Repaint ();
		} else {
			Init ();
		}
	}

	public void AddItemsToMenu (GenericMenu menu)
	{
		menu.AddItem
			(
			new GUIContent ("Settings"),
			false,
			() => EditorGUIUtility.PingObject (m_settings)
		);
	}
}
