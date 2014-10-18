using UnityEngine;
using System.Collections;

public class BubbleBase : MonoBehaviour {

	// Use this for initialization
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
		Debug.Log("sprite created");

		float scale = size / renderer.bounds.size.x;
		transform.localScale = new Vector3 (scale, scale, 1);

		if (num < 10) {
			text_object = new GameObject ();
			(text_object.AddComponent (typeof(TextObject)) as TextObject).init (num.ToString (), "dolphins", 100, col);
			text_object.transform.parent = transform;
			text_object.transform.position += new Vector3 (0, -0.1f, 0);
		}
		Debug.Log("number created");

		gameObject.AddComponent(typeof(CircleCollider2D));
		collider2D.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("bouncy");
		Debug.Log("collider created");

		gameObject.AddComponent (typeof(Rigidbody2D));
		rigidbody2D.gravityScale = falling_speed;
		float angle = Random.Range (-Mathf.PI * 3/4, -Mathf.PI/4);
		rigidbody2D.AddForce (new Vector2 (Mathf.Cos (angle) * 50, Mathf.Sin (angle) * 50));
		Debug.Log("rigid body created");
		
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
		Debug.Log("fiesta start");
	
		for (int i = 0; i < 4; i++) {
			Debug.Log("creating bubble no." + i.ToString());
			GameObject bubble = new GameObject();
			(bubble.AddComponent(typeof(BubbleBase)) as BubbleBase).Init
				(bubble_color, number, renderer.bounds.size.x, rigidbody2D.gravityScale);
			bubble.transform.localScale = transform.localScale;
			bubble.transform.position = transform.position;
			
			//bubble.rigidbody2D.AddForce(new Vector2(Random.Range(-1.0f, 1.0f) * 50, Random.Range(-1.0f, 1.0f) * 50));
		}
		Debug.Log("fiesta end");
		
	}

	public void OnCollisionEnter2D (Collision2D hit) {
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * -1, rigidbody2D.velocity.y);
	}

	public void BecomeActiveBubble()
	{
		Debug.Log("become active bubble start");
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
		bool matched = gm.IsValidBubble (bubble_color, number);
		
		if (matched) {
			is_active = true;

			Debug.Log("handle touch start");
			GameObject points = new GameObject ();
			(points.AddComponent (typeof(TextObject)) as TextObject).init (gm.Score.ToString (), "dolphins", 50, bubble_color);
			points.transform.position = transform.position - new Vector3 (points.renderer.bounds.size.x / 2, 0, 0);
			Debug.Log("point text created");
			
			Destroy (points, 1.1f);
			Destroy (rigidbody2D);
			Destroy(reflection);
			Destroy(shadow);
			reflection = null;
			shadow = null;
			Debug.Log("components destroyed");
			
			collider2D.isTrigger = true;
			BecomeActiveBubble ();
			gm.setAsActiveBubble (gameObject);
			Debug.Log("has became active bubble");
			
			XCEvent.RemoveAllListeners(this);
			XCAnimation.Create(points).MoveBy(Vector3.up, 1.0f);
			XCAnimation.Create(points).FadeOut(1.0f);
			Debug.Log("handle touch end");
			
		} else Pop ();
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
			if (is_active)
					GameManager.getInstance ().ReleaseBubble ();
			else
					handleTouch ();
		}
	}
}
