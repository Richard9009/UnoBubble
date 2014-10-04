using System;
using UnityEngine;

public class TextObject : MonoBehaviour
{
	private TextMesh mesh;
	private float alpha = 1.0f;
	private float fade_speed = 0.02f;
	private bool fading = false;

	public void init(string text, string font, int size, Color col)
	{
		gameObject.AddComponent (typeof(MeshRenderer));
		transform.parent = transform;
		renderer.material = Resources.Load(font + " Material") as Material;
		
		mesh = gameObject.AddComponent(typeof(TextMesh)) as TextMesh;
		mesh.text = text;
		mesh.color = col;
		mesh.fontSize = size;
		mesh.characterSize = 0.1f;
		mesh.font = Resources.Load (font) as Font;
		gameObject.transform.position = new Vector2(-gameObject.renderer.bounds.size.x / 2, gameObject.renderer.bounds.size.y / 2);
	}

	public void setAlignment(TextAlignment hAlignment)
	{
		mesh.alignment = hAlignment;
	}

	public void setText(string text)
	{
		mesh.text = text;
	}

	public void setColor(Color col)
	{
		mesh.color = col;
	}

	public void fadeOut()
	{
		fading = true;
	}

	void Update()
	{
		if (mesh.color.a > 0.0f && fading)
			mesh.color = new Color (mesh.color.r, mesh.color.g, mesh.color.b, alpha -= fade_speed);
	}
}

