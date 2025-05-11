using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backendcafe.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BranchReadDTO>> GetAllBranchesAsync()
        {
            return await _context.Branches
                .Select(b => new BranchReadDTO
                {
                    Id = b.Id,
                    BranchName = b.BranchName,
                    Address = b.Address,
                })
                .ToListAsync();
        }

        public async Task<BranchReadDTO> GetBranchByIdAsync(int id)
        {
            var branch = await _context.Branches
                .Where(b => b.Id == id)
                .Select(b => new BranchReadDTO
                {
                    Id = b.Id,
                    BranchName = b.BranchName,
                    Address = b.Address,
                })
                .FirstOrDefaultAsync();

            if (branch == null)
                throw new Exception("Branch not found");

            return branch;
        }

        public async Task<BranchReadDTO> CreateBranchAsync(BranchCreateDTO branchDto)
        {
           
            var branch = new Branch
            {
                BranchName = branchDto.BranchName,
                Address = branchDto.Address,
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return new BranchReadDTO
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
            };
        }

        public async Task<BranchReadDTO> UpdateBranchAsync(int id, BranchUpdateDTO branchDto)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                throw new Exception("Branch not found");

            branch.BranchName = branchDto.BranchName;
            branch.Address = branchDto.Address;
           
            await _context.SaveChangesAsync();
            return new BranchReadDTO
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                
            };
        }


        public async Task<bool> DeleteBranchAsync(BranchDeleteDTO branchDto)
        {
            var branch = await _context.Branches.FindAsync(branchDto.Id);
            if (branch == null)
                return false;

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}