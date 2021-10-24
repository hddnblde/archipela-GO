using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataManager
{
    #region Public Methods
    public static void Save(object data, string path)
    {
        using (var stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
        }
    }

    public static bool DataExists(string path) =>
        File.Exists(path);

    public static T Load<T>(string path)
    {
        string directory = GetDirectoryOfPath(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        object data = Load(path);

        if (data == null || !(data is T))
            return default;

        else
            return (T)data;
    }
    #endregion


    #region Internal Methods
    private static string GetDirectoryOfPath(string path) =>
        Path.GetDirectoryName(path);

    private static object Load(string path)
    {
        if (!DataExists(path))
            return null;

        using (var stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
    #endregion
}