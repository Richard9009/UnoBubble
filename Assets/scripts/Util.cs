using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {

	public const float PANEL_HEIGHT = 0.125f;
	public const bool ON_MOBILE = false;

	// Use this for initialization
	public struct BubbleColor {
		static public Color RED = new Color(1.0f, 0.5f, 0.5f);
		static public Color GREEN = new Color(0.2f, 0.8f, 0.2f);
		static public Color BLUE = new Color(0.2f, 0.6f, 1.0f);
		static public Color YELLOW = new Color(0.8f, 0.8f, 0.0f);

		public static Color getRandom()
		{
			Color[] colors = { RED, GREEN, BLUE, YELLOW };
			return colors[Random.Range(0, 4)];
		}
	};

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static float UnitToPixels()
	{
		return (Screen.height/2) / Camera.main.orthographicSize;
	}

	public static Vector2 GameAreaSize()
	{
		Vector2 screen_size = Util.FullscreenSize ();
		return new Vector2(screen_size.x, screen_size.y * (1.0f - PANEL_HEIGHT * 2));
	}

	public static Vector2 FullscreenSize()
	{
		return new Vector2 (Screen.width / Util.UnitToPixels (), Screen.height / Util.UnitToPixels ());
	}
}
