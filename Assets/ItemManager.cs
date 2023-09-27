using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Normal.Realtime;

public class ItemManager : MonoBehaviour
{
	// public Vector3 itemScale { get; set; }
	public Vector3 itemPosition { get; set; }
	public Quaternion itemRotation { get; set; }
	// public Vector3 itemRotation { get; set; }
    // public string id { get; set; }
    // public string product_name { get; set; }
    // public double price { get; set; }
    // public double rating { get; set; }
	public GameObject ui { get; set; }
	Vector3 zeroVector = new Vector3(0.0f,0.0f,0.0f);
	private RealtimeTransform _realtimeTransform;
	private XRGrabInteractable _xrInteractable;
	private ItemSync _itemSync;
	bool wasHeld = false;
	int i=0;
	int x=0;
	bool userWasOn = false;
    bool attached = false;
	bool wasItemPlaced=false;
    GameObject rig;
    Vector3 uiPos;
	double total = 0.0;
	bool priceAdded = false;

	
	void Awake(){
		_itemSync = GetComponent<ItemSync>();
	}
	
	void Start(){
        GameObject[] objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for(int i = 0; i<objects.Length; i++){
            if(objects[i].name == "XR Rig"){
                rig = objects[i];
            }
        }
		if (!attached) attatchUI();
		updateName();
		updateScale();
		_realtimeTransform = GetComponent<RealtimeTransform>();
		_xrInteractable = gameObject.GetComponent<XRGrabInteractable>();
	}
	
    // Update is called once per frame
    void Update(){
		bool isHeld = _xrInteractable.isSelected;
		if ((_realtimeTransform.ownerID != -1) && (!_realtimeTransform.isOwnedLocally)) {
			_xrInteractable.interactionLayers = InteractionLayerMask.GetMask("Nothing");
			//Debug.Log(_xrInteractable.interactionLayerMask.value);
		}
		else {
			_xrInteractable.interactionLayers = InteractionLayerMask.GetMask("Default");
			//Debug.Log(_xrInteractable.interactionLayerMask.value);
		}
		if (isHeld) {
			// Debug.Log(_realtimeTransform.ownerID);
			// Debug.Log(gameObject.name);
			_realtimeTransform.RequestOwnership();
			wasHeld = true;
			updateUserUI();
			priceAdded = false;
		}
		else{
			if (wasHeld) 
			{
				// _realtimeTransform.ClearOwnership();
				wasHeld = false;
			}
			if (i%600==300){
				Rigidbody rb = gameObject.GetComponent<Rigidbody>();
				rb.velocity = zeroVector;
				rb.angularVelocity = zeroVector;
				placeItem();
				wasItemPlaced = true;
			}
			if (wasItemPlaced && (i>=500)){
				Rigidbody rb = gameObject.GetComponent<Rigidbody>();
				if (rb.position == itemPosition)
				{
					_realtimeTransform.ClearOwnership();
					wasItemPlaced = false;
				}
			}
			if(userWasOn){
                rig.transform.Find("Camera Offset").Find("Main Camera").Find("userUI").gameObject.SetActive(false);
                userWasOn = false;
            }
		}
		bool isHovered = _xrInteractable.isHovered;
		if (!isHeld && isHovered){
			gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
			updateItemUI();
		}
		else{
			gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
			ui.SetActive(false);
		}
		_itemSync.updateVisible();
		i=(i+1)%600;
    }
	
	public void placeItem(){
		// _realtimeTransform.RequestOwnership();
		if (_realtimeTransform.isOwnedLocally) { // If no owner
			Rigidbody rb = gameObject.GetComponent<Rigidbody>();
			rb.position = itemPosition;
			rb.rotation = itemRotation;
			ui.transform.localPosition = new Vector3 (-0.75f, 0.0f, 0.0f);
			_itemSync.setVisible(true);
			// _realtimeTransform.ClearOwnership();
			// rb.ResetCenterOfMass();
			// if(attached)
            	// ui.transform.position = uiPos;
			// else{
				// uiPos = ui.transform.position;
				// attached = true;
			// }
		}
	}
	
	public void updateName() {_itemSync.updateName();}
	public void updateScale() {
		ui.transform.parent = null;
		_itemSync.updateScale();
		ui.transform.SetParent(gameObject.transform,true);
		ui.transform.localPosition = new Vector3 (-0.75f, 0.0f, 0.0f);
	}
	public void setName(string name)	{_itemSync.setName(name);}
	public void setScale(Vector3 scale)	{
		ui.transform.parent = null;
		_itemSync.setScale(scale);
		ui.transform.SetParent(gameObject.transform,true);
	}
	public void setPname(string  pname) {_itemSync.setPname(pname);}
	public void setPrice(double price)	{_itemSync.setPrice(price);}
	public void setRating(double rating){_itemSync.setRating(rating);}
	public string getPname() {return _itemSync.getPname();}
	public double getPrice() {return _itemSync.getPrice();}
	public double getRating() {return _itemSync.getRating();}

	public void attatchUI(){
        GameObject infoUI = Instantiate(Resources.Load("itemUI", typeof(GameObject))) as GameObject;
        infoUI.transform.SetParent(gameObject.transform,false);
        ui = infoUI;
		attached = true;
	}
    public void updateItemUI(){
        ui.SetActive(true);
        Text ratingText = ui.transform.Find("Rating").Find("Text").GetComponent<Text>();
        ratingText.text = getRating().ToString();
        Text priceText = ui.transform.Find("Price").Find("Text").GetComponent<Text>();
        priceText.text = "$"+getPrice().ToString();
    }
    public void updateUserUI(){
        GameObject userUI = rig.transform.Find("Camera Offset").Find("Main Camera").Find("userUI").gameObject;
        userUI.transform.position = new Vector3(userUI.transform.position.x,rig.transform.Find("Camera Offset").Find("Main Camera").transform.position.y/1.5f,userUI.transform.position.z);
        userUI.SetActive(true);
        Text nameText = userUI.transform.Find("Name").Find("Text").GetComponent<Text>();
        nameText.text = getPname();
        Text ratingText = userUI.transform.Find("Rating").Find("Text").GetComponent<Text>();
        ratingText.text = getRating().ToString();
        Text priceText = userUI.transform.Find("Price").Find("Text").GetComponent<Text>();
        priceText.text = "$"+getPrice().ToString();
        userWasOn = true;
    }

	private void OnTriggerEnter(Collider other)
    {
		Debug.Log("fn entred");
		Debug.Log(other.name);
		Collider hitcol = other.GetComponent<Collider>();
		Debug.Log(hitcol.transform.Find("cartUI"));
		// GameObject hit = hitcol.transform.parent.gameObject;
		if(!priceAdded && !_xrInteractable.isSelected && hitcol.name == "CART"){
			Debug.Log("in cond");
			Debug.Log(getPrice());
			// total += getPrice();
			Text totalText = hitcol.transform.Find("cartUI").Find("Total").Find("Text").GetComponent<Text>();
			string val  = totalText.text.Remove(0,8);
			double tval = Convert.ToDouble(val);
			double totval = tval+getPrice();
			totalText.text = "Total: $"+totval.ToString();
			priceAdded = true;
			_itemSync.setVisible(false);
		}
    }
}
