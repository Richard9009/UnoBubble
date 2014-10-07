using UnityEngine;
using System.Collections;

public class GameplayScene : MonoBehaviour {

	private int interval = 50;
	private int counter = 0;
	private Vector2 screen_size;
	private TextObject score_text;
	private TextObject remaining_bubbles_text;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = Util.FRAME_RATE;
		//Screen.SetResolution (640, 960, false);
		transform.position = new Vector3 (0, 0, 0);
		screen_size = Util.GameAreaSize ();
		transform.localScale = new Vector3(screen_size.x / renderer.bounds.size.x, screen_size.y / renderer.bounds.size.y, 1);

		GameObject left_wall = GameObject.Find ("left_wall");
		left_wall.transform.position = new Vector3 (-screen_size.x / 2 + left_wall.renderer.bounds.size.x / 2, 0, 0);
		left_wall.transform.localScale = new Vector3 (left_wall.transform.localScale.x, screen_size.y / left_wall.renderer.bounds.size.y, 1);
		(left_wall.collider2D as BoxCollider2D).size = new Vector2((left_wall.collider2D as BoxCollider2D).size.x, screen_size.y);
	
		GameObject right_wall = GameObject.Find ("right_wall");
		right_wall.transform.position = new Vector3 (screen_size.x / 2 - right_wall.renderer.bounds.size.x / 2, 0, 0);
		right_wall.transform.localScale = left_wall.transform.localScale;
		(right_wall.collider2D as BoxCollider2D).size = new Vector2((right_wall.collider2D as BoxCollider2D).size.x, screen_size.y);

		Vector2 fullscreen_size = Util.FullscreenSize ();
		GameObject top_panel = GameObject.Find ("top_panel");
		Vector2 original_size = top_panel.renderer.bounds.size;
		top_panel.transform.localScale = new Vector3 (fullscreen_size.x / original_size.x, Util.PANEL_HEIGHT * fullscreen_size.y / original_size.y, 1);
		top_panel.transform.position = new Vector3 (0, fullscreen_size.y / 2 - original_size.y / 2 * top_panel.transform.localScale.y, -2);

		GameObject bottom_panel = GameObject.Find ("bottom_panel");
		bottom_panel.transform.localScale = new Vector3 (fullscreen_size.x / original_size.x, Util.PANEL_HEIGHT * fullscreen_size.y / original_size.y, 1);
		bottom_panel.transform.position = new Vector3 (0, -fullscreen_size.y / 2 + original_size.y / 2 * bottom_panel.transform.localScale.y, -2);

		GameObject score_object = new GameObject ();
		score_text = score_object.AddComponent (typeof(TextObject)) as TextObject;
		score_text.init ("0", "Eras Bold ITC", 50, Color.white);
		score_object.transform.position = new Vector3 (-screen_size.x / 2 + 0.2f, top_panel.transform.position.y + 0.3f, -3);

		GameObject remaining_bubbles_object = new GameObject ();
		remaining_bubbles_text = remaining_bubbles_object.AddComponent (typeof(TextObject)) as TextObject;
		remaining_bubbles_text.init (GameManager.getInstance().RemainingBubbles.ToString(), "Eras Bold ITC", 50, Color.white);
		remaining_bubbles_text.setAlignment (TextAlignment.Center);
		remaining_bubbles_object.transform.position = new Vector3 (0, top_panel.transform.position.y + 0.3f, -3);

		//Block.create (new Vector3 (-1, 1.5f, -1));
		//Spike.create (new Vector3 (1, -1.5f, -1));
		Portal.create (new Vector3 (-3, 0, -1), new Vector3 (3, 2, -1));
		screen_size.x -= left_wall.renderer.bounds.size.x * 2;
	}

	// Update is called once per frame
	void Update () {
		counter++;
		GameManager gm = GameManager.getInstance ();
		score_text.setText (gm.TotalScore.ToString());
		if (counter == interval && gm.RemainingBubbles > 0) {
			counter = 0;
			gm.NewBubble.transform.position = new Vector3(Random.Range(-screen_size.x/2, screen_size.x/2), Util.FullscreenSize().y * 0.75f,-1.0f);
			remaining_bubbles_text.setText(gm.RemainingBubbles.ToString());
		}
	}
}


