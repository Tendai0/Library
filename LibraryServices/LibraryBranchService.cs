using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibtraryContext _context;
        public LibraryBranchService(LibtraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return GetAll()
                 .FirstOrDefault(b => b.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .Include(b => b.LibraryAssets);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches
                   .Include(a => a.LibraryAssets)
                   .FirstOrDefault(a => a.Id == branchId)
                   .LibraryAssets;

        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(h => h.LibraryBranch.Id == branchId);
            return  DataHelpers.HumanizeHours(hours);
            
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                 .Include(p => p.Patrons)
                 .FirstOrDefault(p =>p.Id == branchId).Patrons;
        }

        public bool IsBranchOpen(int Id)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hours = _context.BranchHours.Where(h => h.LibraryBranch.Id == Id);
            var dayhours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayWeek);

            return currentTimeHour < dayhours.CloseTime && currentTimeHour > dayhours.OpenTime;
        }
    }
}
