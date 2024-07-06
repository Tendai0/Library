using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class PatronService : IPatron
    {

        private LibtraryContext _context;

        public PatronService(LibtraryContext context)
        {
            _context = context;
        }

        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return GetAll() 
                .FirstOrDefault(patron => patron.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
               .Include(patron => patron.LibraryCard)
               .Include(patron => patron.HomeLibraryBranch);
        }

        public IEnumerable<CheckOutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = GetLCardId(patronId);

            return _context.CheckOutHistories
                .Include(check => check.LibraryCard)
                .Include(check => check.LibraryAsset)
                .Where(check => check.LibraryCard.Id == cardId)
                .OrderByDescending(check => check.CheckedOut);
        }

        public IEnumerable<CheckOuts> GetCheckOuts(int patronId)
        {
            var cardId = GetLCardId(patronId);

            return _context.CheckOuts
                .Include(cho => cho.LibraryCard)
                .Include(cho => cho.LibraryAsset)
                .Where(cho => cho.LibraryCard.Id == cardId);

        }

        private int GetLCardId(int patronId)
        {
            return _context.Patrons
              .Include(patron => patron.LibraryCard)
              .FirstOrDefault(patron => patron.Id == patronId)
              .LibraryCard.Id;
        }

        public IEnumerable<Holds> GetHolds(int patronId)
        {
            var cardId = GetLCardId(patronId);
            return _context.Holds
                .Include(h => h.LibraryCard)
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryCard.Id == cardId)
                .OrderByDescending(h => h.HoldPlaced);
        }
    }
}
