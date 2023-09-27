using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ItemSync : RealtimeComponent<ItemDetails>
{
	public void setVisible(bool visible){
		model.visible = visible;
		GetComponent<Renderer>().enabled = model.visible;
	}
	
	public void updateVisible() {
		GetComponent<Renderer>().enabled = model.visible;
	}
	
	public void setName(string name) {
		model.name = name;
        gameObject.name = model.name;
    }
	
	public void updateName() {
        gameObject.name = model.name;
    }
	
	public void setScale(Vector3 scale) {
		model.scale = scale;
		gameObject.transform.localScale = model.scale;
	}
	
	public void updateScale() {
		gameObject.transform.localScale = model.scale;
	}
	
	public void setPrice(double price) { model.price = price; }
	public void setRating(double rating) { model.rating = rating; }
	public void setPname(string pname) { model.pname = pname; }
	public double getPrice() { return model.price; }
	public double getRating() { return model.rating; }
	public string getPname() { return model.pname; }
}