using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour {
	public Sprite menu1;
	public Sprite menu2;
	public Sprite button1;
	public Sprite button2;
	private SpriteRenderer menuSpriteRenderer;
	private GameObject button;
	private SpriteRenderer buttonSpriteRenderer;

	// Use this for initialization
	void Start () {
		menuSpriteRenderer = GetComponent<SpriteRenderer>();
		menuSpriteRenderer.sprite = menu1;
		button = Camera.main.transform.Find("PauseMenu").transform.Find("PauseMenuButton").gameObject;
		buttonSpriteRenderer = button.GetComponent<SpriteRenderer>();
		buttonSpriteRenderer.sprite = button1;
	}

	// Update is called once per frame
	void OnMouseDown()
	{
		if(menuSpriteRenderer.sprite == menu1)
		{
			menuSpriteRenderer.sprite = menu2;
			buttonSpriteRenderer.sprite = button2;
		}
		else if(menuSpriteRenderer.sprite == menu2)
		{
			menuSpriteRenderer.sprite = menu1;
			buttonSpriteRenderer.sprite = button1;
		}
	}
}
