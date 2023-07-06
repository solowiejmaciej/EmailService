namespace NotificationService.Repositories
{
    public interface INotificationRepository : IDisposable
    {
        Task<int> SoftDeleteAsync(int id, string userId);

        void SoftDelete(int id);

        public void Save();

        Task SaveAsync();

        void DeleteInBackground();
    }
}