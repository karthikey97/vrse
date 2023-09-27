using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;

public class CartSync : RealtimeComponent<CartSyncModel>
{
	private Text _text;
	private void Awake(){
		_text = GetComponent<Text>();
	}	
	
	public void updateTotal() {
		_text.text = model.total;
	}
	
	protected override void OnRealtimeModelReplaced(CartSyncModel previousModel, CartSyncModel currentModel){
		if(previousModel != null){
			previousModel.totalDidChange -= TotalDidChange;
		}
		
		if(currentModel != null){
			if(currentModel.isFreshModel){
				currentModel.total = _text.text;
			}
			
			updateTotal();
			
			currentModel.totalDidChange += TotalDidChange;
		}
	}
	
	private void TotalDidChange(CartSyncModel model, string total){
		updateTotal();
	}
	
	public void SetText(Text total){
		model.total = total.text;
	}
	
}