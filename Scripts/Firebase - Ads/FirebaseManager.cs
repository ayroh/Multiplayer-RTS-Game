using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System;
using UnityEngine.Serialization;
using static UnityEngine.UI.Button;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    private DependencyStatus dependencyStatus;
    private FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;
    private FirebaseDatabase database;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_InputField fullnameRegisterField;
    public TMP_Text warningRegisterText;
    [FormerlySerializedAs("OnVerificationSend")]
    [SerializeField]
    private ButtonClickedEvent m_onVerificationSend = new ButtonClickedEvent();

    //Saveload variables
    [Header("SaveLoadVariables")]
    [SerializeField]
    private UserSO saveData;
    [SerializeField]
    private User userData;
    [SerializeField]
    private CanonSO ClassicCannon,ElectricCannon,RifleCannon;
    [SerializeField]
    private DeckSO deck;
    [SerializeField]
    private ModifySO ModifyData;

    //Events
    [FormerlySerializedAs("OnAutoLogin")]
    [SerializeField]
    private ButtonClickedEvent m_onAutoLogin = new ButtonClickedEvent();

    [FormerlySerializedAs("OnLogin")]
    [SerializeField]
    private ButtonClickedEvent m_onLogin = new ButtonClickedEvent();

    //Singleton instance
    public static FirebaseManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        DontDestroyOnLoad(this);
        instance = this;

        //Check that all of the necessary dependencies for Firebase are present on the system
        StartCoroutine(FBCheckAndFixDependencies());

    }
    private void OnDestroy()
    {
        //Unregistering state changed for null references
        //Hata veriyor
        //firebaseAuth.StateChanged -= AuthStateChanged;

        //Clearing firebase items
        database = null;
        firebaseAuth = null;
        firebaseUser = null;
    }

    private void InitializeFirebase()
    {
        //Set the authentication instance object
        firebaseAuth = FirebaseAuth.DefaultInstance;

        //Registering firebase user login logout actions
        firebaseAuth.StateChanged += AuthStateChanged;
        AuthStateChanged(this,null);

        //Setting up database 
        database = FirebaseDatabase.DefaultInstance;
    }

    //Login and logout trigger. Controlls saving and loading data
    private void AuthStateChanged(object sender, EventArgs e)
    {
        if (firebaseAuth.CurrentUser != firebaseUser)
        {
            bool signedIn = firebaseUser != firebaseAuth.CurrentUser && firebaseAuth.CurrentUser != null;
            if (!signedIn && firebaseUser != null)
            {
                Debug.Log("Signed out " + firebaseUser.UserId);
                saveData.ResetData();
                userData.ResetData();
            }
            firebaseUser = firebaseAuth.CurrentUser;
            if (signedIn)
            {
                if (string.IsNullOrEmpty(firebaseUser.UserId))
                    print("anasýnýsðikym");
                else
                Debug.Log("Signed in " + firebaseUser.UserId);
                LoadSO(saveData);
                LoadSO(userData);
            }
        }
    }

    #region Button Functions
    //Function for login button
    public void LoginButton()
    {
        Login(emailLoginField.text, passwordLoginField.text);
    }

    //Function for the register button
    public void RegisterButton()
    {
        Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text, fullnameRegisterField.text);
    }

    //Function for logout button
    public void LogoutButton()
    {
        Logout();
    }

    //Function for Resend Email button
    public void ResendMailButton()
    {
        ResendMail();
    }
    #endregion

    #region Private Action Controllers

    //Resending Email verification !!Not in use right now
    private void ResendMail()
    {
        StartCoroutine(SendEmailVerification());
    }

    //Login operation
    private void Login(string _email, string _password)
    {
        StartCoroutine(LoginWithEmailAndPassword(_email,_password));
    }

    //Register operation
    private void Register(string _email, string _password, string _username, string _fullname)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            StartCoroutine(CreateUserWithEmailAndPassword(_email,_password,_username,_fullname));
        }
    }

    //Logout operation
    private void Logout()
    {
        firebaseAuth.SignOut();
    }

    #endregion

    #region Coroutines
    private IEnumerator FBCheckAndFixDependencies()
    {
        //Firebase connection set up

        var task = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled || task.IsFaulted);

        if (task.IsCanceled)
        {
            Debug.LogError("FBCheckAndFixDependencies was canceled.");
            warningLoginText.text = "FBCheckAndFixDependencies was canceled.";
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("FBCheckAndFixDependencies encountered an error: " + task.Exception);
            warningLoginText.text = "FBCheckAndFixDependencies encountered an error: " + task.Exception.InnerException.Message;
        }
        else
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
                if (firebaseUser != null)
                {
                    //If auto logged in
                    Task save = LoadSO(saveData);
                    Task user = LoadSO(userData);
                    //fail check yapýlabilir
                    yield return new WaitUntil(() => save.IsCompletedSuccessfully && user.IsCompletedSuccessfully);
                    m_onAutoLogin.Invoke();
                }
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        }
    }
    private IEnumerator LoginWithEmailAndPassword(string _email, string _password)
    {
        //Signing in
        var loginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(() => loginTask.IsCompleted || loginTask.IsCanceled || loginTask.IsFaulted);

        if (loginTask.IsCanceled)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            warningLoginText.text = "SignInWithEmailAndPasswordAsync was canceled.";
        }
        else if (loginTask.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + loginTask.Exception);
            warningLoginText.text = "SignInWithEmailAndPasswordAsync encountered an error: " + loginTask.Exception.InnerException.Message;
        }
        else
        {
            //Succesfully logged in

            firebaseUser = loginTask.Result;

            if (!firebaseUser.IsEmailVerified)
            {
                //If email not verified
                confirmLoginText.text = "";
                StartCoroutine(SendEmailVerification());
                warningLoginText.text = "Please verify your email";
                Logout();
            }
            else
            {
                //Email verified
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In: " + firebaseUser.Email;
                m_onLogin.Invoke();
            }
        }
    }
    private IEnumerator CreateUserWithEmailAndPassword(string _email, string _password, string _username, string _fullname)
    {
        //Creating user with informations
        var CreateUserTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => CreateUserTask.IsCompleted || CreateUserTask.IsCanceled || CreateUserTask.IsFaulted);

        if (CreateUserTask.IsCanceled)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            warningRegisterText.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
        }
        else if (CreateUserTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + CreateUserTask.Exception);
            warningRegisterText.text = "CreateUserWithEmailAndPasswordAsync encountered an error: " + CreateUserTask.Exception.InnerException.Message;
        }
        else
        {
            // Firebase user has been created.
            firebaseUser = CreateUserTask.Result;

            // Fill SO with new account informations
            userData.SetData(_email, _fullname, _username);

            // Creating new user information on database
            var userSaving = SaveSO(userData);

            yield return new WaitUntil(() => userSaving.IsCompleted || userSaving.IsCanceled || userSaving.IsFaulted);

            //TODO: Email gönderilmemesi durumuna karþý önlem alýnmasý gerek
            if (userSaving.IsCanceled)
            {
                Debug.LogError("Saving user data was canceled.");
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Saving user data was canceled. Account not created";
                yield break;
            }
            else if (userSaving.IsFaulted)
            {
                Debug.LogError("Saving user data encountered an error: " + userSaving.Exception);
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Account not created.Saving user data encountered an error: " + userSaving.Exception.InnerException.Message;
                yield break;
            }
            //TODO: veritabanýna kayýt fail olduðunda user için kaydý silmek gerekebilir. Transaction kullanmak mantýklý olabilir.


            // Creating new save file on database

            saveData.ResetData();
            var saveFileSaving = SaveSO(saveData);

            yield return new WaitUntil(() => saveFileSaving.IsCompleted || saveFileSaving.IsCanceled || saveFileSaving.IsFaulted);

            if (saveFileSaving.IsCanceled)
            {
                Debug.LogError("Saving user data was canceled.");
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Saving user data was canceled. Account not created";
                yield break;
            }
            else if (saveFileSaving.IsFaulted)
            {
                Debug.LogError("Saving user data encountered an error: " + saveFileSaving.Exception);
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Account not created.Saving user data encountered an error: " + saveFileSaving.Exception.InnerException.Message;
                yield break;
            }

            //Saving Cannon Informations to firebase for future loading operations.

            #region ClassicCannonSave

            ClassicCannon.ResetData();

            var ClassicCannonSave = SaveSO(ClassicCannon);

            yield return new WaitUntil(() => ClassicCannonSave.IsCompleted || ClassicCannonSave.IsCanceled || ClassicCannonSave.IsFaulted);

            if (ClassicCannonSave.IsCanceled)
            {
                Debug.LogError("Saving ClassicCannon data was canceled.");
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Saving ClassicCannon data was canceled. Account not created";
                yield break;
            }
            else if (ClassicCannonSave.IsFaulted)
            {
                Debug.LogError("Saving ClassicCannon data encountered an error: " + ClassicCannonSave.Exception);
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Account not created.Saving ClassicCannon data encountered an error: " + ClassicCannonSave.Exception.InnerException.Message;
                yield break;
            }

            #endregion

            #region ElectricCannonSave

            ElectricCannon.ResetData();

            var ElectricCannonSave = SaveSO(ElectricCannon);

            yield return new WaitUntil(() => ElectricCannonSave.IsCompleted || ElectricCannonSave.IsCanceled || ElectricCannonSave.IsFaulted);

            if (ElectricCannonSave.IsCanceled)
            {
                Debug.LogError("Saving ElectricCannon data was canceled.");
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Saving ElectricCannon data was canceled. Account not created";
                yield break;
            }
            else if (ElectricCannonSave.IsFaulted)
            {
                Debug.LogError("Saving ElectricCannon data encountered an error: " + ElectricCannonSave.Exception);
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Account not created.Saving ElectricCannon data encountered an error: " + ElectricCannonSave.Exception.InnerException.Message;
                yield break;
            }

            #endregion

            #region RifleCannonSave

            RifleCannon.ResetData();

            var RifleCannonSave = SaveSO(RifleCannon);

            yield return new WaitUntil(() => RifleCannonSave.IsCompleted || RifleCannonSave.IsCanceled || RifleCannonSave.IsFaulted);

            if (RifleCannonSave.IsCanceled)
            {
                Debug.LogError("Saving RifleCannon data was canceled.");
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Saving RifleCannon data was canceled. Account not created";
                yield break;
            }
            else if (RifleCannonSave.IsFaulted)
            {
                Debug.LogError("Saving RifleCannon data encountered an error: " + RifleCannonSave.Exception);
                //var deletingUser = DeleteUser();
                //yield return new WaitUntil(() => deletingUser.IsCompleted);
                warningRegisterText.text = "Account not created.Saving RifleCannon data encountered an error: " + RifleCannonSave.Exception.InnerException.Message;
                yield break;
            }

            #endregion

            // Sending email verification
            yield return StartCoroutine(SendEmailVerification());

        }
    }
    public IEnumerator SendEmailVerification()
    {
        //Sending email verification
        var SendMailTask = firebaseUser.SendEmailVerificationAsync();

        yield return new WaitUntil(predicate: () => SendMailTask.IsCompleted || SendMailTask.IsCanceled || SendMailTask.IsFaulted);

        //TODO: Email gönderilmemesi durumuna karþý önlem alýnmasý gerek
        if (SendMailTask.IsCanceled)
        {
            Debug.LogError("SendEmailVerificationAsync was canceled.");
            warningRegisterText.text = "SendEmailVerificationAsync was canceled.";

        }
        else if (SendMailTask.IsFaulted)
        {
            Debug.LogError("SendEmailVerificationAsync encountered an error: " + SendMailTask.Exception);
            warningRegisterText.text = "SendEmailVerificationAsync encountered an error: " + SendMailTask.Exception.Message;
        }
        else
        {
            //Email verification sent
            warningLoginText.text = "";
            confirmLoginText.text = "Verification mail send to: " + firebaseUser.Email;
            m_onVerificationSend.Invoke();
            Logout();
        }
    }
    #endregion

    #region Work in progress
    public IEnumerator DeleteUser()
    {
        //Çalýþmayan silme iþlemleri var. Devam edilecek.

        //await DeleteSO(userData);
        //await DeleteSO(saveData);
        //await DeleteSO(ElectricCannon);
        //await DeleteSO(RifleCannon);
        //await DeleteSO(deck);
        //StartCoroutine(DeleteSO(ClasicCannon));

        //var task = LoadSO(ModifyData);

        //yield return new WaitUntil(() => task.IsCompleted);

        StartCoroutine(DeleteSO(ModifyData));

        yield return new WaitForEndOfFrame();
        var task3 = firebaseUser.DeleteAsync();


    }
    public IEnumerator DeleteSO(ScriptableObject SO)
    {
        DatabaseReference SOReference = database.GetReference(SO.name).Child(firebaseUser.UserId);
        var task = SOReference.RemoveValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);
    }

    #endregion

    #region Public Save Operations

    public async Task SaveSO(ScriptableObject SO)
    {
        //Serializing SO
        string json = JsonUtility.ToJson(SO);
        //Getting database reference with so name
        DatabaseReference SOReference = database.GetReference(SO.name);
        //Saving data
        await SOReference.Child(firebaseUser.UserId).SetRawJsonValueAsync(json);
    }
    public async Task LoadSO(ScriptableObject SO)
    {
        //Getting database reference with so name
        DatabaseReference SOReference = database.GetReference(SO.name);
        //Loading data
        await SOReference.Child(firebaseUser.UserId).GetValueAsync()
            .ContinueWith(result =>
            {
                //Deserializing data
                if (result.IsCompleted) JsonUtility.FromJsonOverwrite(result.Result.GetRawJsonValue(), SO);
            });
    }

    #endregion

}
