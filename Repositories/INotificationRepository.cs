namespace NotificationService.Repositories
{
    public interface INotificationRepository : IDisposable
    {
        Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
        
        public void Save();

        Task SaveAsync(CancellationToken cancellationToken = default);

        void DeleteInBackground();
    }
}