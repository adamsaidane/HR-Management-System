using HRMS.Models;
using HRMS.Repositories;

namespace HRMS.Repositories.Implementation;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(HRMSDbContext context) : base(context) { }
}