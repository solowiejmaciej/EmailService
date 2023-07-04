namespace NotificationService.Repositories
{
    public interface IRepository : IDisposable
    {
        public void Save();

        Task SaveAsync();

        void DeleteInBackground();
    }
}