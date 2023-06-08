using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Unity.Services.CloudSave;
public class CloudSaver
{
    private static ICloudSaveDataClient _client;

    public static async Task SaveData(string key, object value)
    {
        try
        {
            if (_client == null) _client = CloudSaveService.Instance.Data;

            var data = new Dictionary<string, object>{{key, value}};
            await _client.ForceSaveAsync(data);
        }
        catch(CloudSaveException e)
        {
            Debug.LogError(e);
            Debug.LogError($"Failed to save data with key {key}, value {value}");
        }
    }

    public static async void Test()
    {
        var data = new Dictionary<string, object>{{"key", "someValue"}};
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }
    
    public static async Task<string> GetData(string key)
    {
        if (_client == null) _client = CloudSaveService.Instance.Data;
        try
        {
            Dictionary<string, string> data = await _client.LoadAsync(new HashSet<string> { key });
            return data[key];
        }
        catch
        {
            return null;
        }
    }

    public static async Task<T> GetSerializedData<T>(string key) => DeserializeData<T>(await GetData(key));
    public static T DeserializeData<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized);
}
