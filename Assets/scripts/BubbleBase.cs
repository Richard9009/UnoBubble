using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleBase : MonoBehaviour {

	private const int MAXIMUM_BUBBLES = 40;
	private static int num_of_bubbles = 0;

	public static BubbleBase create(Color col, int num, float size, float falling_speed)
	{
		if (num_of_bubbles == MAXIMUM_BUBBLES)
						return null;

		GameObject obj = new GameObject ();
		BubbleBase ret = obj.AddComponent (typeof(BubbleBase)) as BubbleBase;
		ret.Init (col, num, size, falling_speed);
		num_of_bubbles++;
		return ret;
	}

	private Color bubble_color;
	private int number;
	private bool is_active = false;
	private GameObject text_object;
	private GameObject reflection;
	private GameObject shadow;
	void Start () {
		reflection = new GameObject ();
		(reflection.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).sprite = Resources.Load<Sprite> ("bubble-reflection");
		reflection.transform.position = transform.position;
		reflection.name = "reflection";
		
		shadow = new GameObject ();
		(shadow.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).sprite = Resources.Load<Sprite> ("bubble-shadow");
		shadow.transform.position = transform.position;
		shadow.name = "shadow";
	}

	void OnDestroy() {
		num_of_bubbles--;
	}

	public void Init(Color col, int num, float size, float falling_speed)
	{
		bubble_color = col;
		number = num;

		XCEvent.AddListener ("SLOW DOWN", this, SlowDown);
		XCEvent.AddListener ("CHANGE COLOR", this, ChangeColor);
		XCEvent.AddListener ("FIESTA", this, fiesta);

		SpriteRenderer sprite = gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		sprite.sprite = Resources.Load<Sprite>("bubble");
		sprite.color = col;

		float scale = size / renderer.bounds.size.x;
		transform.localScale = new Vector3 (scale, scale, 1);

		text_object = new GameObject ();
		(text_object.AddComponent (typeof(TextObject)) as TextObject).init (num.ToString (), "dolphins", 100, col);
		text_object.transform.parent = transform;
		text_object.transform.position += new Vector3 (0, -0.1f, 0);

		gameObject.AddComponent(typeof(CircleCollider2D));
		collider2D.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("bouncy");

		gameObject.AddComponent (typeof(Rigidbody2D));
		rigidbody2D.gravityScale = falling_speed;
		float angle = Random.Range (-Mathf.PI * 3/4, -Mathf.PI/4);
		rigidbody2D.AddForce (new Vector2 (Mathf.Cos (angle) * 50, Mathf.Sin (angle) * 50));
		
	}

	public void SlowDown(Hashtable param)
	{
		if(rigidbody2D)
			rigidbody2D.velocity = Vector3.zero;
	}

	public void ChangeColor(Hashtable param)
	{
		Color col = (Color)param ["color"];
		bubble_color = col;
		XCAnimation.Create (text_object).ColorTo (col, 1.0f);
		XCAnimation.Create (gameObject).ColorTo (col, 1.0f);
	}

	public void fiesta(Hashtable param)
	{
		for (int i = 0; i < 4; i++) {
			BubbleBase bubble = BubbleBase.create(bubble_color, number, renderer.bounds.size.x, rigidbody2D.gravityScale);
			if(bubble == null) return;
			bubble.transform.localScale = transform.localScale;
			bubble.transform.position = transform.position;
		}
	}

	public void OnCollisionEnter2D (Collision2D hit) {
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * -1, rigidbody2D.velocity.y);
	}

	public void BecomeActiveBubble()
	{
		Debug.Log("become active bubble start");
		Camera.main.audio.PlayOneShot(Resources.Load<AudioClip>("sounds/bubble"));
		transform.rotation = Quaternion.identity;
		XCAnimation.Create(gameObject).MoveBy (new Vector3(-1.5f, 1.5f, -3), 0.2f).Link(
			XCAnimation.Create(gameObject).MoveTo (GameManager.getInstance ().ActiveBubblePos, 0.3f));
		Debug.Log("become active bubble end");
		
	}

	public void ReleaseAnimation()
	{
		Debug.Log("release animation start");
		XCAnimation.Create(gameObject).MoveBy(Vector3.down * 5, 0.5f);
		Destroy (gameObject, 0.6f);
		Debug.Log("release animation end");
	}

	public void Pop()
	{
			Camera.main.audio.PlayOneShot(Resources.Load<AudioClip>("sounds/bubble pop"));
		transform.rotation = Quaternion.identity;
		(renderer as SpriteRenderer).sprite = Resources.Load<Sprite>("bubble-pop");
		XCAnimation.Create(gameObject).FadeOut (1.0f);
		XCAnimation.Create(gameObject).ScaleTo (transform.localScale * 2, 1.0f);

		XCEvent.RemoveAllListeners (this);
		Destroy (gameObject, 1.0f);
		Destroy (reflection);
		Destroy (shadow);
		Destroy (collider2D);
		Destroy (rigidbody2D);
		Destroy (text_object);

		reflection = null;
		shadow = null;
	}

	public void handleTouch()
	{
		GameManager gm = GameManager.getInstance ();
		MatchStatus match = gm.IsValidBubble (bubble_color, number);
		
		if(match == MatchStatus.NOT_MATCH){
			Pop ();
			XCEvent.DispatchEvent("COMBO BREAK", null);
		}
		else {
			is_active = true;
			if(match == MatchStatus.SUPER_MATCH) {
				GameObject smatch = new GameObject ();
				(smatch.AddComponent (typeof(TextObject)) as TextObject).init ("SUPER MATCH\nCombo +5", "Eras Bold ITC", 30, Color.white);
				(smatch.GetComponent (typeof(TextObject)) as TextObject).setAlignment(TextAlignment.Center);
				smatch.transform.position = transform.position - new Vector3 (smatch.renderer.bounds.size.x / 2, -1.0f, 0);
				XCAnimation.Create(smatch).ScaleTo(Vector3.one * 1.5f, 2.0f);
				XCAnimation.Create(smatch).FadeOut(2.0f);
				Destroy(smatch, 2.1f);
				Camera.main.audio.PlayOneShot(Resources.Load<AudioClip>("sounds/super match"));
			}

			GameObject points = new GameObject ();
			(points.AddComponent (typeof(TextObject)) as TextObject).init (gm.Score.ToString(), "dolphins", 50, bubble_color);
			points.transform.position = transform.position - new Vector3 (points.renderer.bounds.size.x / 2, 0, 0);
			
			Destroy (points, 1.1f);
			Destroy (rigidbody2D);
			Destroy(reflection);
			Destroy(shadow);
			reflection = null;
			shadow = null;
			
			collider2D.isTrigger = true;
			BecomeActiveBubble ();
			gm.setAsActiveBubble (gameObject);

			XCEvent.DispatchEvent("GET BUBBLE", null);
			XCEvent.RemoveAllListeners(this);
			XCAnimation.Create(points).MoveBy(Vector3.up, 1.0f);
			XCAnimation.Create(points).FadeOut(1.0f);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (reflection) {
			reflection.transform.position = transform.position;
			reflection.transform.localScale = transform.localScale;
			shadow.transform.position = transform.position + new Vector3 (0.2f, -0.2f, 0);
			shadow.transform.localScale = transform.localScale;
		}
		if (!is_active && transform.position.y + renderer.bounds.size.y/2 < -Util.GameAreaSize ().y / 2) {
			XCEvent.RemoveAllListeners (this);
			Destroy(gameObject);
			Destroy(reflection);
			Destroy(shadow);
			reflection = null;
			shadow = null;
			return;
		}

		if (!collider2D) return;
		if (Input.GetMouseButtonDown (0) == false) return;

		Vector3 obj_pos = transform.position;
		Vector3 touch_pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		float distance = Mathf.Sqrt (Mathf.Pow (obj_pos.x - touch_pos.x, 2) + Mathf.Pow (obj_pos.y - touch_pos.y, 2));

		if (distance < (collider2D as CircleCollider2D).radius * transform.localScale.x) {
			Debug.Log("bubble clicked");
			//if (is_active)
				//XCEvent.DispatchEvent("COMBO BREAK", null);
			if(!is_active)
					handleTouch ();
		}
	}
}
