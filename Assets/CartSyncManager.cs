using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartSyncManager : MonoBehaviour
{
    private Text _text;
	private Text _prevText;
	
	private CartSync _cartSync;
	
    void Awake()
    {
        _cartSync = GetComponent<CartSync>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_text != _prevText) {
			_cartSync.SetText(_text);
			_prevText = _text;
		}
    }
}
