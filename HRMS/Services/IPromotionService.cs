using HRMS.Models;

namespace HRMS.Service;

public interface IPromotionService
{
    IEnumerable<Promotion> GetAllPromotions();
    IEnumerable<Promotion> GetEmployeePromotions(int employeeId);
    void CreatePromotion(Promotion promotion);
    void ProcessPromotion(int employeeId, int newPositionId, decimal newSalary, string justification);
}