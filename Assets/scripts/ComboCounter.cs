using UnityEngine;
using System.Collections;

public class ComboCounter : MonoBehaviour {

	public static ComboCounter create()
	{
		return (new GameObject ()).AddComponent (typeof(ComboCounter)) as ComboCounter;
	}

	private Vector3 IMAGE_SCALE = new Vector3(0.7f, 0.7f, 0.7f);
	private XCAnimation anim = null;
	private TextObject combo_text;
	private int wait_frames = 0;

	private float GetComboTime()
	{
		switch (GameManager.getInstance ().ComboLevel) {
			case ComboName.NONE: return 5.0f;
			case ComboName.COMBO: return 4.5f;
			case ComboName.BRAVO: return 4.0f;
			case ComboName.AWESOME: return 3.5f;
			case ComboName.SUPER: return 3.0f;
			case ComboName.PERFECT: return 3.0f;
			default: return 3.0f;
		}
	}
	// Use this for initialization
	void Start () {
		XCEvent.AddListener ("GET BUBBLE", this, UpdateComboCount);
		XCEvent.AddListener ("COMBO BREAK", this, ComboBreak);
		XCEvent.AddListener ("COMBO CALLED", this, UpdateComboLevel);

		(gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).
			sprite = null;

		GameObject tObj = new GameObject ();
		combo_text = tObj.AddComponent (typeof(TextObject)) as TextObject;
		combo_text.init ("", "dolphins", 90, Color.white);
		tObj.transform.parent = transform;
		tObj.transform.position += Vector3.down;

		transform.localScale = IMAGE_SCALE;
		transform.position = new Vector3 (Util.GameAreaSize().x / 2 - 2.0f, Util.GameAreaSize().y / 2, -5);
	}

	public void UpdateComboCount(Hashtable param)
	{
		if (anim) Destroy (anim);

		combo_text.setText ("x" + GameManager.getInstance ().ComboCount);
		transform.localScale = IMAGE_SCALE;
		anim = XCAnimation.Create (gameObject).ScaleTo (Vector3.zero, GetComboTime ());
		wait_frames = Mathf.CeilToInt(GetComboTime () * Util.FRAME_RATE);
		combo_text.setVisible (true);
	}

	public void UpdateComboLevel(Hashtable param)
	{
		(gameObject.renderer as SpriteRenderer).
			sprite = Resources.Load<Sprite> (GameManager.getInstance().ComboLevel.ToString());
	}

	public void ComboBreak(Hashtable param)
	{
		if (anim) Destroy (anim);
		anim = null;
		transform.localScale = IMAGE_SCALE;
		combo_text.setText ("");
		(gameObject.renderer as SpriteRenderer).sprite = null;
		GameManager.getInstance ().ReleaseBubble ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.getInstance ().ComboCount > 0) {
			wait_frames--;
			if(wait_frames == 0)
				ComboBreak(null);
		}
	}
}
