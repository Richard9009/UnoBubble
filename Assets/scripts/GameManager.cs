using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class GameManager {

	// Use this for initialization
	private static GameManager instance = null;

	public int current_num = -1;
	public Color current_col = Color.white;
	public int combo_count = 1;

	public static GameManager getInstance()
	{
		if (instance == null)
			instance = new GameManager ();
		return instance;
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	public void setAsActiveBubble(GameObject bubble)
	{
		float diameter = Util.FullscreenSize ().y * Util.PANEL_HEIGHT;
		Vector3 end_pos = new Vector3 (-Util.GameAreaSize ().x / 2 + diameter/2,
		                              -Util.GameAreaSize ().y / 2 - diameter / 2, -5);
		Vector3 anchor_pos = new Vector3 (bubble.transform.position.x - 1.5f, bubble.transform.position.y + 1.5f, -5);

		float scale = diameter / bubble.renderer.bounds.size.y * bubble.transform.localScale.y;
		bubble.transform.localScale = new Vector3 (scale, scale, 1);
		HOTween.To(bubble.transform, 0.2f, new TweenParms().Prop("position", anchor_pos).Ease(EaseType.EaseOutQuad).
		           											Prop("rotation",new Vector3()));

		HOTween.To (bubble.transform, 0.3f, new TweenParms ().Prop ("position", end_pos).Ease (EaseType.EaseInOutQuad).Delay (0.1f));
			
		GameObject prev_active = GameObject.Find ("active_bubble");
		if (prev_active) {
			HOTween.To (prev_active.transform, 0.5f, new TweenParms ().Prop ("position", 
    	    			new Vector3 (prev_active.transform.position.x, prev_active.transform.position.y - 5.0f, -4)).
            			Ease (EaseType.EaseInCubic).Delay (0.2f));
			prev_active.name = "prev_bubble";
			MonoBehaviour.Destroy(prev_active, 0.5f);
		}

		bubble.name = "active_bubble";  
		combo_count++;
		
	}
		           
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
