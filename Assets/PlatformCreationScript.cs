using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;
using Newtonsoft.Json;
using Normal.Realtime;

public class PlatformCreationScript : MonoBehaviour
{

    [SerializeField] private string textURL = "http://127.0.0.1:5000";

    [System.Serializable]
    public class Product{
        public float h_pos { get; set; }
        public float height { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double rating { get; set; }
        public int shelf_id { get; set; }
        public float x_pos { get; set; }
        public float width { get; set; }
        public float depth {get; set; }
    }

    public class ProductList{
        public int prod_count { get; set; }
        public List<Product> product_list { get; set; }
    }

	private int counter = 0;
	private int count2 = -1;
	private float inch2mtr = 0.025f;
    
    // Start is called before the first frame update
    void Start(){
		StartCoroutine(Wait());
	}

    // Update is called once per frame
    void Update(){}
	
	private IEnumerator Wait(){
		Debug.Log("Starting wait");
		yield return new WaitForSeconds(5);
		Debug.Log("Wait completed");
		StartCoroutine(GetText());
	}

    private IEnumerator GetText(){
        using (UnityWebRequest request = UnityWebRequest.Get(textURL)){
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success){
                Debug.LogError(request.error);
            } 
			else {
                ProductList productList = JsonConvert.DeserializeObject<ProductList>(request.downloadHandler.text);
				Debug.Log("Success :D Found " + productList.prod_count.ToString() + " products");
                for (int i = 0; i < productList.prod_count; i++) {
                    Product product = productList.product_list[i];
					StartCoroutine(GetImage(product));
                    // Debug.Log("Product: " + product.name);
                }
            }
        }
    }

	private IEnumerator GetImage(Product product){
		count2 += 1;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textURL+"/"+count2.ToString())){
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success){
                Debug.LogError(request.error);
            } 
			else {
                // Debug.Log("Success retieving image");
				Texture productImage = ((DownloadHandlerTexture)request.downloadHandler).texture;
				createProduct(product, productImage);
            }
        }
    }

    void createProduct(Product product, Texture imgTexture) {
		bool exists = false;
		Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
		Vector3 itemRotation = new Vector3(0.0f, 0.0f, 0.0f);
		Quaternion itemQuaternion = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
		Vector3 itemPosition = new Vector3(0.0f, 0.0f, 0.0f);
		
		// Obtaining position of the product
		if (product.shelf_id==0){
			itemPosition = new Vector3(3.0f, product.h_pos*inch2mtr+0.5f, -product.x_pos*inch2mtr+1.5f);
			itemRotation = new Vector3(0.0f, 0.0f, 0.0f);
			itemQuaternion = Quaternion.Euler(itemRotation);
		}
		if (product.shelf_id==1){
			itemPosition = new Vector3(-product.x_pos*inch2mtr+2.5f, product.h_pos*inch2mtr+0.5f, 2.0f);
			itemRotation = new Vector3(0.0f, 270.0f, 0.0f);
			itemQuaternion = Quaternion.Euler(itemRotation);
		}
		if (product.shelf_id==2){
			itemPosition = new Vector3(-3.0f, product.h_pos*inch2mtr+0.5f, product.x_pos*inch2mtr-1.5f);
			itemRotation = new Vector3(0.0f, 180.0f, 0.0f);
			itemQuaternion = Quaternion.Euler(itemRotation);
		}
		if (product.shelf_id==3){
			itemPosition = new Vector3(product.x_pos*inch2mtr-2.5f, product.h_pos*inch2mtr+0.5f, -2.0f);
			itemRotation = new Vector3(0.0f, 90.0f, 0.0f);
			itemQuaternion = Quaternion.Euler(itemRotation);
		}
		
		ItemManager itemManager;
		GameObject cube = GameObject.Find(product.id);
		if (cube) {
			itemManager = cube.GetComponent<ItemManager>();
			exists = true;
		}
		else {
			cube = Realtime.Instantiate("product_prefab", itemPosition, itemQuaternion, ownedByClient: false, preventOwnershipTakeover: false);
			itemManager = cube.GetComponent<ItemManager>();
			itemManager.setName(product.id);
			scale = new Vector3(product.depth*inch2mtr, product.height*inch2mtr, product.width*inch2mtr);
			itemManager.attatchUI();
			itemManager.setScale(scale);
			
			// set product metadata
			itemManager.setPname(product.name);
			itemManager.setPrice(product.price);
			itemManager.setRating(product.rating);
		}
		
		itemManager.itemPosition = itemPosition;
		itemManager.itemRotation = itemQuaternion;
		
        cube.GetComponent<Renderer>().material.color = Color.white;
		cube.GetComponent<Renderer>().material.SetTexture("_BaseMap", imgTexture);
		cube.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);		
    }
}