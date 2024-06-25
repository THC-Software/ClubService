using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class EmailOutboxRepository(ReadStoreDbContext readStoreDbContext) : IEmailOutboxRepository
{
    public async Task Add(EmailMessage emailMessage)
    {
        await readStoreDbContext.EmailOutbox.AddAsync(emailMessage);
        await readStoreDbContext.SaveChangesAsync();
    }
}