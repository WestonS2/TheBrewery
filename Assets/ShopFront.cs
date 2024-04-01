using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopFront : MonoBehaviour
{
	public static ShopFront instance;
	
	public static GameObject currentCustomer;
	
	IDictionary<ItemData.ITEM, int> potionValue = new Dictionary<ItemData.ITEM, int>();
	
	[SerializeField] GameObject shopCamera;
	[SerializeField] GameObject shopUI;
	[SerializeField] Transform itemSpawnLocation;
	[SerializeField] TextMeshProUGUI npcDialogue;
	
	bool shopOpen;
	bool dialogueBoxOpen;
	
	void Start()
	{
		if(instance == null) instance = this;
		else Destroy(this.gameObject);
		
		shopCamera.SetActive(false);
		
		shopOpen = false;
		dialogueBoxOpen = false;
		
		potionValue.Add(ItemData.ITEM.HealthPotion, 6);
		potionValue.Add(ItemData.ITEM.FirePotion, 7);
		potionValue.Add(ItemData.ITEM.IcePotion, 7);
		potionValue.Add(ItemData.ITEM.GrowthPotion, 9);
		potionValue.Add(ItemData.ITEM.LuckPotion, 15);
	}
	
	void Update()
	{
		if(shopOpen)
		{
			if(Input.GetKeyDown(Controls.exitKey))
			{
				ToggleShopFront();
			}
			
			if(Input.GetMouseButtonDown(Controls.pickUpMouseKey))
			{
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(mouseRay.origin, mouseRay.direction, out RaycastHit hit, PlayerInteraction.instance.interactionDistance, LayerMask.GetMask("Shop")))
				{
					if(hit.collider.gameObject.tag == "NPC")
					{
						ToggleDialogue();
					}
				}
			}
			
			#region Spawn Potions For Testing
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.HealthPotion], itemSpawnLocation.position, itemSpawnLocation.rotation);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.FirePotion], itemSpawnLocation.position, itemSpawnLocation.rotation);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.IcePotion], itemSpawnLocation.position, itemSpawnLocation.rotation);
			}
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.GrowthPotion], itemSpawnLocation.position, itemSpawnLocation.rotation);
			}
			if(Input.GetKeyDown(KeyCode.Alpha5))
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.LuckPotion], itemSpawnLocation.position, itemSpawnLocation.rotation);
			}
			#endregion
		}
	}
	
	void ToggleDialogue()
	{
		dialogueBoxOpen = !dialogueBoxOpen;
		
		npcDialogue.gameObject.SetActive(dialogueBoxOpen);
		npcDialogue.SetText(currentCustomer.GetComponent<NPC>().dialogue.text);
	}
	
	public void ToggleShopFront()
	{
		shopOpen = !shopOpen;
		
		if(shopOpen)
		{
			GameManager.instance.PlayerState = GameManager.PLAYERSTATE.ShopMode;
			shopCamera.SetActive(true);
		}
		else
		{
			shopCamera.SetActive(false);
			GameManager.instance.PlayerState = GameManager.PLAYERSTATE.FreeRoam;
		}
	}
	
	public void ToggleInventory()
	{
		//Inventory script stuff here
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Item" && col.gameObject.GetComponent<ItemData>().Item != ItemData.ITEM.Coin)
		{
			StartCoroutine(Pay(currentCustomer.GetComponent<NPC>().Serve(col.gameObject.GetComponent<ItemData>().Item), col.gameObject.GetComponent<ItemData>().Item));
			Destroy(col.gameObject);
		}
	}
	
	IEnumerator Pay(bool fullPay, ItemData.ITEM spawnItem)
	{
		if(fullPay)
		{
			for(int i = 0; i < potionValue[spawnItem]; i++)
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.Coin], itemSpawnLocation.position, itemSpawnLocation.rotation);
				yield return new WaitForSeconds(0.15f);
			}
		}
		else
		{
			for(int i = 0; i < (potionValue[spawnItem] / 2); i++)
			{
				Instantiate(GameManager.instance.itemPrefabs[ItemData.ITEM.Coin], itemSpawnLocation.position, itemSpawnLocation.rotation);
				yield return new WaitForSeconds(0.15f);
			}
		}
		
		npcDialogue.gameObject.SetActive(false);
	}
}
