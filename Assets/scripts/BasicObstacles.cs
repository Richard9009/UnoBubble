using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class Block : MonoBehaviour {

	// Use this for initialization
	static public Block create(Vector3 pos)
	{
		GameObject obj = new GameObject ();
		obj.transform.position = pos;
		obj.name = "Block";
		return obj.AddComponent (typeof(Block)) as Block;
	}

	void Start () {
		SpriteRenderer sr = gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer;
		sr.sprite = Resources.Load<Sprite> ("block");
		sr.color = Color.red;
		transform.localScale = new Vector3 (0.2f, 0.2f, 1);

		gameObject.AddComponent (typeof(Rigidbody2D));
		rigidbody2D.isKinematic = true;
		gameObject.AddComponent (typeof(BoxCollider2D)); 
		collider2D.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("bouncy");
	}

	void OnCollisionEnter2D (Collision2D hit) {
		Rigidbody2D body = hit.gameObject.rigidbody2D;
		body.velocity = new Vector2(body.velocity.x * -1, body.velocity.y * -1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Spike : MonoBehaviour {

	static public Spike create(Vector3 pos)
	{
		GameObject obj = new GameObject ();
		obj.transform.position = pos;
		obj.transform.localScale = new Vector3 (0.3f, 0.3f, 1.0f);
		obj.name = "Spike";
		return obj.AddComponent (typeof(Spike)) as Spike;
	}

	void Start()
	{
		SpriteRenderer sr = gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer;
		sr.sprite = Resources.Load<Sprite> ("spike");
		sr.color = Color.blue;

		gameObject.AddComponent (typeof(CircleCollider2D));
		collider2D.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		(other.gameObject.GetComponent (typeof(BubbleBase)) as BubbleBase).Pop ();
	}
}

public class Portal : MonoBehaviour {

	static public Portal create(Vector3 pos_blue, Vector3 pos_red)
	{
		GameObject blue = new GameObject ();
		blue.transform.position = pos_blue;
		blue.transform.localScale = new Vector3 (0.3f, 0.5f, 1);
		blue.name = "PortalBlue";
		(blue.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).sprite = Resources.Load<Sprite> ("portal");
		Portal portalB = blue.AddComponent (typeof(Portal)) as Portal;

		GameObject red = new GameObject ();
		red.transform.position = pos_red;
		red.transform.localScale = new Vector3 (-0.3f, 0.5f, 1);
		red.name = "PortalRed";
		(red.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).sprite = Resources.Load<Sprite> ("portal");
		portalB.Pair = red.AddComponent (typeof(Portal)) as Portal;

		return portalB;
	}

	private Portal _pair = null;
	public Portal Pair { 
		set { 	_pair = value; 
				if(!value.Pair)
					value.Pair = this; } 
		get { return _pair; }
	}

	void Start()
	{
		gameObject.AddComponent(typeof(BoxCollider2D));
		collider2D.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		GameObject obj = other.gameObject;
		if (obj.name != "bubble") return;
		float original_scale = obj.transform.localScale.x;
		obj.collider2D.enabled = false; //disable collider to prevent infinite loop

		//show animation of bubble going and shrinking inside portal
		HOTween.To (obj.transform, 1.0f, new TweenParms ().Prop ("position", transform.position).
															Prop("localScale", Vector3.zero));

		StartCoroutine(_pair.AcceptTeleport (obj, original_scale));
	}

	public IEnumerator AcceptTeleport(GameObject obj, float ori_scale)
	{
		yield return new WaitForSeconds(1.0f);

		//teleport the bubble to the other side of the portal
		obj.transform.position = transform.position;

		//animation of the bubble grows back
		HOTween.To (obj.transform, 1.0f, new TweenParms ().Prop("localScale", new Vector3(ori_scale, ori_scale, 1)));

		//give bubble enough time to get out of the portal before activating collider back
		StartCoroutine (ActivateBack (obj, ori_scale));
	}

	public IEnumerator ActivateBack(GameObject obj, float ori_scale)
	{
		yield return new WaitForSeconds(1.0f);
		obj.collider2D.enabled = true;
		obj.transform.localScale = new Vector3 (ori_scale, ori_scale, 1);
	}

}