
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AuthController : MonoBehaviour {

    //Login Input Fields
    public InputField loginEmailInput;
    public InputField loginPWInput;

    //Register Input Fields
    public InputField registerUserInput;
    public InputField registerPWInput;
    public InputField registerPWConfInput;
    public InputField registerEmailInput;

    //private login variables
    private string _loginEmailInput{ get { return loginEmailInput.text.ToString(); } }
    private string _loginPasswordInput { get { return loginPWInput.text.ToString(); } } //USE EncryptPassword(loginPWInput.text.toString()); instead when the DB is updated with the encrypted value and after error handling

    //private register variables
    private string _registerUserInput { get { return registerUserInput.text.ToString(); } }
    private string _registerPWInput { get { return registerPWInput.text.ToString();  } } //USE EncryptPassword(registerPWInput.text.toString()); instead when the DB is updated with the encrypted value and after error handling and compare
    private string _registerPWConfInput { get { return registerPWConfInput.text.ToString();  } } //USE EncryptPassword(registerPWConfInput.text.toString()); instead when the DB is updated with the encrypted value and after error handling and compare
    private string _registerEmailInput { get { return registerEmailInput.text.ToString();  } }

    //Display error messages here
    public Text Error_Text;

    /*
     * Registers User
     */
    public void RegisterUser()
    {
        /* TODO: Check for errors (build error manager class)
         * e.g. same password in the two inputs (create method checkPWs)
         * e.g. async check for username or email already exists
         * e.g. check for fields requirements 
         * e.g. create requirements for password and email and username
         */

        //Add values from Input to Dictionary
        IDictionary<string, string> keyValuePairs = new Dictionary<string, string>
        {
            { "email", _loginEmailInput },
            { "password", EncryptPassword(_loginPasswordInput) }
        };

        //Call to AwsApiManager POST
        gameObject.GetComponent<AwsApiManager>().Login(keyValuePairs);
    }

    /*
     * Logins User
     */
    public void LoginUser()
    {
        //Add values from Input to Dictionary
        IDictionary<string, string> keyValuePairs = new Dictionary<string, string>
        {
            { "email", _loginEmailInput },
            { "password", _loginPasswordInput }
        };
        Debug.Log("Email & Password" + _loginEmailInput + " " + _loginPasswordInput);
        //Calls to AwsApiManager POST
        gameObject.GetComponent<AwsApiManager>().Login(keyValuePairs);
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    void OnCustomAuthenticationFailed(string debugMessage)
    {
        Error_Text.text = debugMessage;
        Error_Text.gameObject.SetActive(true);
    }

    /*
     * Gives response of connection top left corner of screen
     */
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    /*
     * Encrypts password with PBKDF2 Algorithm
     */
    private string EncryptPassword(string password)
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

    /*
     * Checks if Register Passwords are the same (need to see if it's safer to encrypt them first)
     */
    private bool CheckPasswords(string _password, string _passwordConf)
    {
        if (_password == _passwordConf)
            return true;
        return false;
    }
    
}
