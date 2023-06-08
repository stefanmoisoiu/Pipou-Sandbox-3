using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ConnectToUGS : MonoBehaviour
{
    public UnityEvent onConnected;
    private async void Start()
    {
#if UNITY_EDITOR
        if(ParrelSync.ClonesManager.IsClone()) await UnityServices.InitializeAsync(new InitializationOptions().SetProfile(ParrelSync.ClonesManager.CloneNameSuffix));
        else await UnityServices.InitializeAsync();
#endif
#if UNITY_STANDALONE
        await UnityServices.InitializeAsync();
#endif
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        onConnected.Invoke();
    }
}
