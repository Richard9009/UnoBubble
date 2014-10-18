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
		Vector2 fullscreen_size = Util.FullscreenSize ();
		float general_scale = screen_size.x / renderer.bounds.size.x;
		transform.localScale = new Vector3(general_scale, general_scale, 1);

		GameObject left_wall = GameObject.Find ("left_wall");
		left_wall.transform.localScale = new Vector3 (general_scale, general_scale, 1);
		left_wall.transform.position = new Vector3 (-screen_size.x / 2 + left_wall.renderer.bounds.size.x / 2, 0, -1);
	
		GameObject right_wall = GameObject.Find ("right_wall");
		right_wall.transform.localScale = left_wall.transform.localScale;
		right_wall.transform.position = new Vector3 (screen_size.x / 2 - right_wall.renderer.bounds.size.x / 2, 0, -1);

		GameObject top_left = GameObject.Find ("top left frame");
		top_left.transform.localScale = new Vector3 (general_scale, general_scale, 1);
		top_left.transform.position = new Vector3 (-fullscreen_size.x / 2 + top_left.renderer.bounds.size.x / 2,
		                                          fullscreen_size.y / 2 - top_left.renderer.bounds.size.y / 2, -3);

		GameObject top_panel = GameObject.Find ("top_panel");
		Vector2 original_size = top_panel.renderer.bounds.size;
		top_panel.transform.localScale = new Vector3 (general_scale, general_scale, 1);
		top_panel.transform.position = new Vector3 (fullscreen_size.x / 2 - original_size.x / 2 * top_panel.transform.localScale.x, fullscreen_size.y / 2 - original_size.y / 2 * top_panel.transform.localScale.y, -2);

		GameObject bottom_panel = GameObject.Find ("bottom_panel");
		bottom_panel.transform.localScale = new Vector3 (general_scale, general_scale, 1);
		bottom_panel.transform.position = new Vector3 (0, -fullscreen_size.y / 2 + original_size.y / 2 * bottom_panel.transform.localScale.y, -2);

		GameObject stage_bubble = GameObject.Find ("stage bubble");
		stage_bubble.transform.localScale = new Vector3 (general_scale, general_scale, 1);
		stage_bubble.transform.position = new Vector3 (fullscreen_size.x / 2 - stage_bubble.renderer.bounds.size.x / 2,
		                                              fullscreen_size.y / 2 - stage_bubble.renderer.bounds.size.y / 2, -3);

		GameObject score_object = new GameObject ();
		score_text = score_object.AddComponent (typeof(TextObject)) as TextObject;
		score_text.init ("0", "dolphins", 50, Color.white);
		score_object.transform.position = top_left.transform.position + new Vector3 (-1, -0.2f, -1);

		(GameObject.Find ("stage num text").GetComponent (typeof(TextMesh)) as TextMesh).text = GameManager.stage_num.ToString ();

		GameObject remaining_bubbles_object = new GameObject ();
		remaining_bubbles_text = remaining_bubbles_object.AddComponent (typeof(TextObject)) as TextObject;
		remaining_bubbles_text.init (GameManager.getInstance().RemainingBubbles.ToString(), "dolphins", 80, Color.grey);
		remaining_bubbles_text.setAlignment (TextAlignment.Center);
		remaining_bubbles_object.transform.position = new Vector3 (0, top_panel.transform.position.y + 0.6f, -4);

		ComboCounter.create ();
		//Block.create (new Vector3 (-1, -1.5f, -1));
		//Spike.create (new Vector3 (1, -1.5f, -1));
		//Portal.create (new Vector3 (-3, 0, -1), new Vector3 (3, 2, -1));
		screen_size.x -= left_wall.renderer.bounds.size.x * 2;
	}

	// Update is called once per frame
	void Update () {
		counter++;
		GameManager gm = GameManager.getInstance ();
		score_text.setText (gm.TotalScore.ToString());
		if (counter == interval && gm.RemainingBubbles > 0) {
			counter = 0;
			BubbleBase bubble = gm.NewBubble;
			if(bubble == null) return;
			bubble.transform.position = new Vector3(Random.Range(-screen_size.x/2, screen_size.x/2), Util.FullscreenSize().y * 0.75f,-1.0f);
			bubble.name = "bubble";
			remaining_bubbles_text.setText(gm.RemainingBubbles.ToString());

		}

		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}
}


