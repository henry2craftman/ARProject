using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    [Header("Sign In Properties")]
    [SerializeField] GameObject signInPanel;
    [SerializeField] TMP_InputField signInEmailInput;
    [SerializeField] TMP_InputField signInPasswordInput;
    [SerializeField] Button signInBtn;
    [SerializeField] Button signUpBtn;
    [SerializeField] Button cancelBtn;

    [Header("Sign Up Properties")]
    [SerializeField] GameObject signUpPanel;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField confirmedPasswordInput;

    [Header("Verification Properties")]
    [SerializeField] GameObject verificationPanel;
    [SerializeField] TextMeshProUGUI verificationTxt;

    FirebaseAuth auth;
    FirebaseUser user;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAuthentication();
    }

    /// <summary>
    /// Firebase Authentication 초기화 함수
    /// </summary>
    private void InitializeAuthentication()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    /// <summary>
    /// 로그인 상태를 보여주는 함수
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    /// <summary>
    /// 기본 예제
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public void SignUp(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            SendVerificationEmail();

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    /// <summary>
    /// UI에 적용되는 함수
    /// </summary>
    public void SignUp()
    {
        signUpPanel.SetActive(true);
        signInPanel.SetActive(false);

        StartCoroutine(SignUpAsync(nameInput.text, emailInput.text, passwordInput.text, confirmedPasswordInput.text));

        IEnumerator SignUpAsync(string name, string email, string password, string confirmedPassword)
        {
            if(name == "")
            {
                Debug.Log("name is empty.");
            }
            else if(email == "")
            {
                Debug.Log("name is empty.");
            }
            else if(password != confirmedPassword)
            {
                Debug.Log("password does not match");
            }
            else
            {
                var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);

                yield return new WaitUntil(() => task.IsCompleted);

                if (task.Exception != null)
                {
                    Debug.Log(task.Exception);

                    FirebaseException exception = task.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)exception.ErrorCode;

                    string errorMsg = "";

                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            errorMsg += "Invalid Email";
                            break;
                        case AuthError.WeakPassword:
                            errorMsg += "Weak Password";
                            break;
                    }
                }
                else
                {
                    user = auth.CurrentUser;
                    var verificationTask = user.SendEmailVerificationAsync();

                    yield return new WaitUntil(() => verificationTask.IsCompleted);

                    yield return VerificationAsync(email);
                }
            }
        }
    }

    IEnumerator VerificationAsync(string email)
    {
        verificationPanel.SetActive(true);

        verificationTxt.text = string.Format("Please check out\n{0}\nfor verification.", email);

        while (true)
        {
            yield return new WaitForSeconds(1);

            var userReloaded = user.ReloadAsync();

            yield return new WaitUntil(() => userReloaded.IsCompleted);

            if (user.IsEmailVerified)
                break;

            Debug.Log("UserProfile을 업데이트 하는 중입니다.");
        }

        signUpPanel.SetActive(false);
        verificationPanel.SetActive(false);
        signInPanel.SetActive(true);
    }
        
    public void SignIn()
    {
        auth.SignInWithEmailAndPasswordAsync(signInEmailInput.text, signInPasswordInput.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            if (user.IsEmailVerified)
            {
                Debug.Log("로그인이 되었습니다.");
            }
            else
            {
                StartCoroutine(VerificationAsync(signInEmailInput.text));
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    /// <summary>
    /// VerificationEmail을 보내는 예시 함수
    /// </summary>
    public void SendVerificationEmail()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email sent successfully.");
            });
        }
    }
}
