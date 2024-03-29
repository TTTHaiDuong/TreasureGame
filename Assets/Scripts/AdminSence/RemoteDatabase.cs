using System;
using System.Data;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

public class RemoteDatabase : NetworkBehaviour
{
    public delegate void RemoteDatabaseConnect();
    private static RemoteDatabaseConnect IfSuccessful;

    public static RemoteDatabase ServerRemote;
    private static bool FlagStart;
    private static int CountClient;

    public static DataTable Result;

    private void Awake()
    {
        if (ServerRemote == null)
        {
            ServerRemote = this;
        }
    }

    private void Start()
    {
        if (this == ServerRemote) Scan();
    }

    private void Update()
    {
        if (FlagStart && !NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            if (CountClient <= 1) NetworkManager.Singleton.StartServer();
            else NetworkManager.Singleton.StartClient();
        }
    }

    public static void InvokeIfSuccessful(RemoteDatabaseConnect ifSuccessful)
    {
        IfSuccessful = ifSuccessful;
    }

    public static RemoteDatabase GetOwner()
    {
        RemoteDatabase[] remotes = FindObjectsOfType<RemoteDatabase>();
        foreach (RemoteDatabase remote in remotes)
            if (remote.IsOwner) return remote;
        return null;
    }

    public void ExecuteQuery(string query)
    {
        Result = null;
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Result = DBProvider.ExecuteQuery(query);
            IfSuccessful?.Invoke();
        }
        else GetOwner().DatabaseServerRpc(query);
    }

    public void ExecuteQuery(string query, string[] parameters, string[] values)
    {
        Result = null;
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Result = DBProvider.ExecuteQuery(query, parameters, values);
            IfSuccessful?.Invoke();
        }
        else
        {
            ArraySerializable<string> paraS = new(parameters);
            ArraySerializable<string> valS = new(values);
            GetOwner().DatabaseServerRpc(query, paraS, valS);
        }
    }

    [ServerRpc]
    private void DatabaseServerRpc(string query)
    {
        Debug.Log("Start ServerControls");
        DatabaseClientRpc(query);
    }

    [ServerRpc]
    private void DatabaseServerRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS)
    {
        DatabaseClientRpc(query, paraS, valS);
    }

    [ClientRpc]
    private void DatabaseClientRpc(string query)
    {
        Result = DBProvider.ExecuteQuery(query);
        IfSuccessful?.Invoke();
    }

    [ClientRpc]
    private void DatabaseClientRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS)
    {
        string[] parameters = paraS.Deserialize();
        string[] values = valS.Deserialize();
        Result = DBProvider.ExecuteQuery(query, parameters, values);
        IfSuccessful?.Invoke();
    }

    static Timer Timer;
    public static void Scan()
    {
        if (Timer == null || !Timer.IsRunning)
        {
            NetworkManager.Singleton.StartClient();
            Timer = new GameObject().AddComponent<Timer>();
            Timer.FinishListening(ClientOrServer);
            Timer.Play(2);
        }
    }

    private static void ClientOrServer(object obj)
    {
        RemoteDatabase[] remotes = FindObjectsOfType<RemoteDatabase>();
        NetworkManager.Singleton.Shutdown();

        CountClient = remotes.Length;
        FlagStart = true;
    }
}

//public class DataTableSerializable : INetworkSerializable
//{
//    public DataTableSerializable(DataTable table)
//    {
//        SerializedString = JsonConvert.SerializeObject(table);
//    }
//    public DataTableSerializable() { }

//    public string SerializedString;

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref SerializedString);
//    }

//    public DataTable Deserialize()
//    {
//        return JsonConvert.DeserializeObject<DataTable>(SerializedString);
//    }
//}

/// <summary>
/// Lớp Serialize DataTable
/// </summary>
public class DataTableSerializable : INetworkSerializable
{
    public DataTableSerializable(DataTable table)
    {
        if (table == null) throw new ArgumentNullException(nameof(table), "DataTable cannot be null.");
        SerializedString = JsonConvert.SerializeObject(table);
    }
    public DataTableSerializable() { }
    public DataTableSerializable(string serializedString)
    {
        SerializedString = serializedString;
    }

    public string SerializedString;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SerializedString);
    }

    public DataTable Deserialize()
    {
        return JsonConvert.DeserializeObject<DataTable>(SerializedString);
    }
}

public class ArraySerializable<T> : INetworkSerializable
{
    public ArraySerializable(T[] array)
    {
        if (array != null)
        {
            Wrapper wrapper = new() { Items = array.ToList() };
            SerializedString = JsonUtility.ToJson(wrapper);
        }
    }
    public ArraySerializable() { }

    [Serializable]
    private class Wrapper
    {
        public List<T> Items = new();
    }

    public string SerializedString;

    public void NetworkSerialize<T1>(BufferSerializer<T1> serializer) where T1 : IReaderWriter
    {
        serializer.SerializeValue(ref SerializedString);
    }

    public T[] Deserialize()
    {
        if (SerializedString == "") return new T[0];
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(SerializedString);
        return wrapper.Items.ToArray();
    }
}

