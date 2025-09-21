namespace TZAPI.Core
{
    public interface ICoreRepository
    {
        bool UpdateData(out string em);
        List<string> SearchData(string path);
    }
}
