using System.Collections.Generic;
using System.Threading.Tasks;
using backendcafe.DTO;

namespace backendcafe.Services
{
    public interface IBranchService
    {
        Task<List<BranchReadDTO>> GetAllBranchesAsync();
        Task<BranchReadDTO> GetBranchByIdAsync(int id);
        Task<BranchReadDTO> CreateBranchAsync(BranchCreateDTO branchDto);
        Task<BranchReadDTO> UpdateBranchAsync(int id, BranchUpdateDTO branchDto);

        Task<bool> DeleteBranchAsync(BranchDeleteDTO branchDto);
    }
}