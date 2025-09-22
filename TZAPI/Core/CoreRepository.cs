using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace TZAPI.Core
{
    public class CoreRepository : ICoreRepository
    {
        private readonly object LockUpdate = new();
        private ConcurrentDictionary<string, List<string>> DataStorage = new();

        public CoreRepository()
        {
            UpdateData(out string em);
            Console.WriteLine(em);
        }

        public bool UpdateData(out string em)
        {
            try
            {
                var StringLines = File.ReadAllLines("AdvertisingPlatforms.txt");

                lock (LockUpdate)
                {
                    DataStorage = ReadingData(StringLines);
                }

                em = "Ok";
                return true;
            }
            catch (FileNotFoundException)
            {
                em = "FileNotFound";
            }
            catch (Exception ex)
            {
                em = $"Crytical error {ex.Message}";
            }

            return false;
        }

        public List<string> SearchData(string path)
        {
            List<string> AdvertisingPlatforms = new();

            if (!string.IsNullOrEmpty(path) && DataStorage.ContainsKey(path))
            {
                AdvertisingPlatforms = DataStorage[path];
            }

            return AdvertisingPlatforms;
        }

        private ConcurrentDictionary<string, List<string>> ReadingData(string[] StringLines)
        {
            var sb = new StringBuilder();
            ConcurrentDictionary<string, List<string>> NewDataStorage = new();

            foreach (var Line in StringLines)
            {
                if (string.IsNullOrWhiteSpace(Line)) continue;
                var Split = Line.Split(':');
                if (Split.Length != 2) continue;

                var Platform = Split[0].Trim();
                var KeysList = Split[1].Trim().Split(',').Select(p => p.Trim()).ToList();

                foreach (var Key in KeysList)
                {
                    NewDataStorage.AddOrUpdate(
                        Key,
                        Z => new List<string> { Platform },
                        (Z, DataStorage) =>
                        {
                            if (!DataStorage.Contains(Platform))
                            {
                                DataStorage.Add(Platform);
                            }
                            return DataStorage;
                        });
                }
            }

            foreach (var Key in NewDataStorage.Keys)
            {
                sb.Clear();
                var Split = Key.Split('/', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < Split.Length - 1; i++)
                {
                    sb.Append('/');
                    sb.Append(Split[i]);
                    string path = sb.ToString();

                    if (NewDataStorage.TryGetValue(path, out var DataStorage))
                    {
                        NewDataStorage[Key] = NewDataStorage[Key].Union(DataStorage).ToList();
                    }
                }
            }

            return NewDataStorage;
        }
    }
}
