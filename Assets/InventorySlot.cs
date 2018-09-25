using System.Net.Sockets;
using System.IO;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler {
    public Image slot_icon;
    public Button remove_button;
    public Image slot_amount;
    public Image slot_glow;
    public WorldItem item;

    public void OnDrag(PointerEventData eventData)
    {
        if (item)
        {
            slot_icon.transform.position = Input.mousePosition;
            SetSlotVisibility(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item)
        {
            slot_icon.transform.localPosition = Vector3.zero;
            SetSlotVisibility(true);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        foreach (InventorySlot slot in transform.parent.gameObject.GetComponentsInChildren<InventorySlot>()){
            // check if dragged into any slot
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.gameObject.transform as RectTransform, Input.mousePosition))
            {
                InventorySlot dragged_from = eventData.lastPress.GetComponentInParent<InventorySlot>();

                if (dragged_from.item && slot != dragged_from){
                    slot.AddToSlotAfterDragged(dragged_from.item);
                    dragged_from.RemoveItem();
                }
            }
        }
    }

    private void CreateNewItem(int amount){
        item = gameObject.AddComponent<WorldItem>();
        SetAmount(amount);
    }

    public void AddToSlot(WorldItem i){
        // if something of this type is already in this slot - just add amount
        if (item && item.item_name == i.item_name){
            SetAmount(item.amount + i.amount);
        }
        // otherwise - put new item into inventory
        else{
            CreateNewItem(i.amount);
            SetSlotIcon(i.GetSprite());
            item.icon = i.GetSprite();
        }
    }

    private void AddToSlotAfterDragged(WorldItem i){
        CreateNewItem(i.amount);
        SetSlotIcon(i.icon);
        item.icon = i.icon;
    }

    private void SetSlotIcon(Sprite s)
    {
        slot_icon.sprite = s;
        slot_icon.enabled = s;
        remove_button.interactable = s;
        remove_button.gameObject.SetActive(s);
    }

    private void SetAmount(int amount){
        SetAmountVisibility(amount > 1);
        slot_amount.GetComponentInChildren<Text>().text = amount.ToString();
        item.amount = amount;
    }

    private void SetSlotVisibility(bool v)
    {
        remove_button.interactable = v;
        slot_amount.enabled = v;
        slot_amount.GetComponentInChildren<Text>().enabled = v;
    }

    private void SetAmountVisibility(bool v){
        slot_amount.enabled = v;
        slot_amount.gameObject.SetActive(v);
        slot_amount.GetComponentInChildren<Text>().enabled = v;
    }

    private void ResetAmount(){
        SetAmount(1);
    }

    public void SelectItem(){
        // deselect all other slots
        //gameObject.GetComponents<InventorySlot>().slot_glow.enabled = false;

        //TcpClient tcpClient = new TcpClient();
        //tcpClient.Connect("35.180.106.179", 8999);
        //Console.WriteLine("you connected to the server!");

        //NetworkStream networkStream = tcpClient.GetStream();
        //StreamReader clientStreamReader = new StreamReader(networkStream);
        //StreamWriter clientStreamWriter = new StreamWriter(networkStream);

        //var client = new TcpClient("35.180.106.179", 8999);
        //var message = System.Text.Encoding.ASCII.GetBytes("Testing");
        //var stream = client.GetStream();
        //stream.Write(message, 0, message.Length); //sends bytes to server

        //var data = new byte[128];
        //int respLength = stream.Read(data, 0, data.Length); //gets next 128 bytes when sent to client
        //Console.Write("RECEIVED: "+System.Text.Encoding.ASCII.GetString(data));
        //stream.Close();
        //client.Close();

        Connect("35.180.106.179", "");

        // select this slot
        slot_glow.enabled = !slot_glow.enabled;
        
        gameObject.GetComponentInParent<PlayerInventory>().ToggleDropButton();
    }

    static void Connect(String server, String message)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 8999;
            TcpClient client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: "+responseData);

            // Close everything.
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }

    public void UseItem()
    {
        if (item != null)
        {
            //item.Use();
            DropItem();
        }
    }

    public void DropItem()
    {
        if (item != null)
        {
            playerMove p = gameObject.GetComponentInParent<playerMove>();
            Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);

            item.Drop(v, p.sprite.flipX, 1);
            if (item.amount <= 1)
            {
                ResetAmount();
                ClearSlot();
            }
            SetAmount(item.amount - 1);
        }
    }

    public void RemoveItem()
    {
        if (item){
            item.Destroy();
        }
        ResetAmount();
        ClearSlot();
    }

    public void ClearSlot()
    {
        Destroy(item);
        SetSlotIcon(null);
        slot_amount.enabled = false;
        slot_amount.gameObject.SetActive(false);
    }

    public bool IsEmpty()
    {
        return item == null;
    }
}