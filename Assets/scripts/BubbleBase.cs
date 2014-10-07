using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class BubbleBase : MonoBehaviour {

	// Use this for initialization
	private const float MAX_VELOCITY = 1.5f;
	private Color bubble_color;
	private int number;
	private bool is_active = false;
	private GameObject text_object;

	void Start () {

	}

	public void Init(Color col, int num, float size, float falling_speed)
	{
		bubble_color = col;
		number = num;

		SpriteRenderer sprite = gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		sprite.sprite = Resources.Load<Sprite>("bubble");
		sprite.color = col;
		
		gameObject.AddComponent (typeof(Rigidbody2D));
		rigidbody2D.gravityScale = falling_speed;
		float angle = Random.Range (-Mathf.PI * 3/4, -Mathf.PI/4);
		rigidbody2D.AddForce (new Vector2 (Mathf.Cos (angle) * 50, Mathf.Sin (angle) * 50));

		gameObject.AddComponent(typeof(CircleCollider2D));
		collider2D.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("bouncy");

		float scale = size / renderer.bounds.size.x;
		transform.localScale = new Vector3 (scale, scale, 1);

		text_object = new GameObject ();
		(text_object.AddComponent (typeof(TextObject)) as TextObject).init (num.ToString (), "Eras Bold ITC", 75, col);
		text_object.transform.parent = transform;
	}

	public void OnCollisionEnter2D (Collision2D hit) {
		rigidbody2D.velocity *= 1.5f;
		if (rigidbody2D.velocity.magnitude > MAX_VELOCITY) {
			float sin = rigidbody2D.velocity.y / rigidbody2D.velocity.magnitude;
			rigidbody2D.velocity = new Vector2(MAX_VELOCITY * (1 - sin), MAX_VELOCITY * sin);
		}
	}

	public void BecomeActiveBubble()
	{
		float diameter = Util.FullscreenSize ().y * Util.PANEL_HEIGHT;
		Vector3 anchor_pos = new Vector3 (transform.position.x - 1.5f, transform.position.y + 1.5f, -5);
		
		float scale = diameter / renderer.bounds.size.y * transform.localScale.y;
		transform.localScale = new Vector3 (scale, scale, 1);
		HOTween.To(transform, 0.2f, new TweenParms().Prop("position", anchor_pos).
		           				Ease(EaseType.EaseOutQuad).Prop("rotation",new Vector3()));
		
		HOTween.To (transform, 0.3f, new TweenParms ().Prop ("position", GameManager.getInstance().ActiveBubblePos).
		            									Ease (EaseType.EaseInOutQuad).Delay (0.1f));
	}

	public void ReleaseAnimation()
	{
		HOTween.To (transform, 0.5f, new TweenParms ().Prop ("position", 
		                    new Vector3 (transform.position.x, transform.position.y - 5.0f, -4)).
		            		Ease (EaseType.EaseInCubic).Delay (0.2f));
		Destroy (gameObject, 0.5f);
	}

	public void Pop()
	{
		transform.rotation = new Quaternion();
		(renderer as SpriteRenderer).sprite = Resources.Load<Sprite>("bubble_pop");
		rigidbody2D.AddForce(new Vector2(0, 300.0f));
		rigidbody2D.gravityScale *= 500;
		
		Destroy (collider2D);
		Destroy(text_object);
	}
	
	public void handleTouch()
	{
		GameManager gm = GameManager.getInstance ();
		bool matched = gm.IsValidBubble (bubble_color, number);
		
		if (matched) {
			is_active = true;

			GameObject points = new GameObject ();
			(points.AddComponent (typeof(TextObject)) as TextObject).init (gm.Score.ToString (), "Eras Bold ITC", 50, bubble_color);
			points.transform.position = transform.position - new Vector3 (points.renderer.bounds.size.x / 2, 0, 0);

			HOTween.To (points.transform, 1.0f, new TweenParms ().Prop ("position", 
             new Vector3 (points.transform.position.x, points.transform.position.y + 1.0f, -3)));
			(points.GetComponent (typeof(TextObject)) as TextObject).fadeOut ();
			Destroy (points, 1.0f);

			Destroy (rigidbody2D);
			collider2D.isTrigger = true;
			BecomeActiveBubble ();
			gm.setAsActiveBubble (gameObject);

		} else Pop ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!is_active && transform.position.y + renderer.bounds.size.y/2 < -Util.GameAreaSize ().y / 2) {
			Destroy(gameObject);
		}

		if (!collider2D) return;
		if (Input.GetMouseButtonDown (0) == false) return;

		Vector3 obj_pos = transform.position;
		Vector3 touch_pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		float distance = Mathf.Sqrt (Mathf.Pow ((obj_pos.x - touch_pos.x), 2) + Mathf.Pow ((obj_pos.y - touch_pos.y), 2));
		if (distance < (collider2D as CircleCollider2D).radius) {
			if(is_active)GameManager.getInstance().ReleaseBubble();
			else handleTouch();
		}
	}
}
