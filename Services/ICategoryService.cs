namespace backendcafe.Services
{
    public interface ICategoryService
    {
        Task<List<DTO.CategoryReadDTO>> GetCategoriesByBranchAsync(int branchId);
        Task<DTO.CategoryReadDTO> GetCategoryByIdAsync(int id);
        Task<DTO.CategoryReadDTO> CreateCategoryAsync(DTO.CategoryCreateDTO categoryDto);
        Task<DTO.CategoryReadDTO> UpdateCategoryAsync(DTO.CategoryUpdateDTO categoryDto);
        Task<bool> DeleteCategoryAsync(DTO.CategoryDeleteDTO categoryDto);
    }
}