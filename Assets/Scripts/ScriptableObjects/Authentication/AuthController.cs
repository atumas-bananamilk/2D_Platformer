using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class AuthController : MonoBehaviour {

    public InputField User_Input;
    public InputField Password_Input;
    public Text Error_Text;

    private string _username { get { return User_Input.text.ToString(); } }
    private string _password { get { return EncryptPassword(Password_Input.text.ToString()); } }
    
    //TODO: Create Register and Login Functions

    public static void RegisterUser()
    {

    }

    public void LoginUser()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        PhotonNetwork.AuthValues.AddAuthParameter("username", _username);
        PhotonNetwork.AuthValues.AddAuthParameter("password", _password);
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnJoinedLobby()
    {
        Debug.Log("Made it");
    }

    void OnCustomAuthenticationFailed(string debugMessage)
    {
        Error_Text.text = debugMessage;
        Error_Text.gameObject.SetActive(true);
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public string EncryptPassword(string password)
    {
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }
}
