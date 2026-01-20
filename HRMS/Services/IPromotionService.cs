using HRMS.Models;
using HRMS.ViewModels;

namespace HRMS.Service;

public interface IPromotionService
{
    Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
    Task<IEnumerable<Promotion>> GetEmployeePromotionsAsync(int employeeId);
    Task CreatePromotionAsync(Promotion promotion);
    Task ProcessPromotionAsync(int employeeId, int newPositionId, decimal newSalary, string justification);
    Task<PromotionFormViewModel> GetPromotionFormViewModelAsync();
}