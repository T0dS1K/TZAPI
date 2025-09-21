using System.Collections.Concurrent;

namespace TZAPI.Core
{
    public class CoreRepository : ICoreRepository
    {
        private readonly object LockUpdate = new();
        private ConcurrentDictionary<string, List<string>> DataStorage = new();

        public CoreRepository() { }

        public bool UpdateData(out string em)
        {
            try
            {
                var StringLines = File.ReadAllLines("AdvertisingPlatforms.txt");
                ConcurrentDictionary<string, List<string>> NewDataStorage = new();

                lock (LockUpdate)
                {
                    /////////////////////////////
                    
                    DataStorage = NewDataStorage;
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
    }
}
