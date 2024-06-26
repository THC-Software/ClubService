using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class EmailOutboxRepository(ReadStoreDbContext readStoreDbContext) : IEmailOutboxRepository
{
    public async Task Add(EmailMessage emailMessage)
    {
        await readStoreDbContext.EmailOutbox.AddAsync(emailMessage);
        await readStoreDbContext.SaveChangesAsync();
    }

    public async Task<List<EmailMessage>> GetAllEmails()
    {
        return await readStoreDbContext.EmailOutbox.OrderBy(x => x.Timestamp).ToListAsync();
    }

    public async Task RemoveEmails(List<EmailMessage> emails)
    {
        readStoreDbContext.EmailOutbox.RemoveRange(emails);
        await readStoreDbContext.SaveChangesAsync();
    }
}