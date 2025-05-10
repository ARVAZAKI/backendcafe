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
                    LogoUrl = b.LogoUrl,
                    BannerUrl = b.BannerUrl
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
                    LogoUrl = b.LogoUrl,
                    BannerUrl = b.BannerUrl
                })
                .FirstOrDefaultAsync();

            if (branch == null)
                throw new Exception("Branch not found");

            return branch;
        }

        public async Task<BranchReadDTO> CreateBranchAsync(BranchCreateDTO branchDto)
        {
            if (!Uri.TryCreate(branchDto.LogoUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Logo URL must be a valid URL");
            if (branchDto.BannerUrl != null && !Uri.TryCreate(branchDto.BannerUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Banner URL must be a valid URL if provided");

            var branch = new Branch
            {
                BranchName = branchDto.BranchName,
                Address = branchDto.Address,
                LogoUrl = branchDto.LogoUrl,
                BannerUrl = branchDto.BannerUrl
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return new BranchReadDTO
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                LogoUrl = branch.LogoUrl,
                BannerUrl = branch.BannerUrl
            };
        }

        public async Task<BranchReadDTO> UpdateBranchAsync(int id, BranchUpdateDTO branchDto)
        {
            if (!Uri.TryCreate(branchDto.LogoUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Logo URL must be a valid URL");
            if (branchDto.BannerUrl != null && !Uri.TryCreate(branchDto.BannerUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Banner URL must be a valid URL if provided");

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                throw new Exception("Branch not found");

            branch.BranchName = branchDto.BranchName;
            branch.Address = branchDto.Address;
            branch.LogoUrl = branchDto.LogoUrl;
            branch.BannerUrl = branchDto.BannerUrl;

            await _context.SaveChangesAsync();
            return new BranchReadDTO
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                LogoUrl = branch.LogoUrl,
                BannerUrl = branch.BannerUrl
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