﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int itemId,wearId;
    public int price;

    public Text priceText;
    public Button buyButton,equipButton,unequipButton;

    public GameObject  itemPrefab;

    public bool HasItem()
    {
        //0:Daha satin alinmamis
        //1:satin alinmiş giyilmemiş
        //2:hem satın alınmış hem de giyilmiş
        bool hasItem=PlayerPrefs.GetInt("item"+itemId.ToString())!=0;
        return hasItem;
    }
    public bool IsEquipped()
    {

        bool equippedItem=PlayerPrefs.GetInt("item"+itemId.ToString())==2;
        return equippedItem;
    }

    public void InitializeItem()
    {
        priceText.text=price.ToString();
        if(HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if(IsEquipped())
            {
                EquipItem();
            }
            else{
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }
    public void BuyItem()
    {
        if(!HasItem())
        {
            int money=PlayerPrefs.GetInt("money");
            if(money>=price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip,0.1f);
                LevelController.Current.GiveMoneyPlayer(-price);
                PlayerPrefs.SetInt("item"+itemId.ToString(),1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);

            }
        }
    }
    public void EquipItem()
    {
        UnEquipItem();
        MarketController.Current.equippedItems[wearId]=Instantiate(itemPrefab,PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();
        MarketController.Current.equippedItems[wearId].itemId=itemId;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item"+itemId.ToString(),2);

    }
    public void UnEquipItem()
    {
        Item equippedItem=MarketController.Current.equippedItems[wearId];
        if(equippedItem !=null)
        {
            MarketItem marketItem=MarketController.Current.items[equippedItem.itemId];
            PlayerPrefs.SetInt("item"+marketItem.itemId,1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);

        }
    }
    public void EquipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equippedItemAudioClip,0.1f);
        EquipItem();
    }
    public void UnEquipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.UnEquipItemAudioClip,0.1f);
        UnEquipItem();
    }
}
