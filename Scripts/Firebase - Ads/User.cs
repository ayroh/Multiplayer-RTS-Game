using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "User")]
public class User : ScriptableObject
{
    public void SetData(string _email, string _fullName, string _nickname)
    {
        Email = _email;
        FullName = _fullName;
        Nickname = _nickname;
    }
    public void ResetData()
    {
        Email = "";
        FullName = "";
        Nickname = "";
    }
    public string Email;
    public string FullName;
    public string Nickname;
}
