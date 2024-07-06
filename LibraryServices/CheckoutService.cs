using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibtraryContext _context;
        public CheckoutService(LibtraryContext context)
        {
            _context = context;
        }
        public void Add(CheckOuts newcheckOuts)
        {
            _context.Add(newcheckOuts);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context
               .LibraryAssets
               .FirstOrDefault(l => l.Id == assetId);

            _context.Update(item);
            //Remove any existing checkouts items

            RemoveExistingCheckouts(assetId);

            //CloseExisting checkouthistory
            CloseExistingCheckoutsHistory(assetId, now);

            //Lookup for existing holds
            var currentholds = _context
                .Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);
            //if there are holds checkout the utem to the librarycard with earliest hold
            if (currentholds.Any())
            {
                CheckOutToEaliestHold(assetId, currentholds);
                return;
            }
            //Otherwise, update the item status to available 
            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();

        }

        private void CheckOutToEaliestHold(int assetId, IQueryable<Holds> currentholds)
        {
            var earliesthold = currentholds
                 .OrderBy(e => e.HoldPlaced)
                 .FirstOrDefault();

            var card = earliesthold.LibraryCard;

            _context.Remove(earliesthold);
            _context.SaveChanges();

            CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int LibraryCardId)
        {

            if (IscheckedOut(assetId))
            {
                return;
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(a=>a.Id ==assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var librarycard = _context.LibraryCards
                .Include(c => c.CheckOuts)
                .FirstOrDefault(c => c.Id == LibraryCardId);

            var now = DateTime.Now;

            var checkout = new CheckOuts
            {
                LibraryAsset = item,
                LibraryCard = librarycard,
                Since = now,
                Until = GetdefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var chechouthistory = new CheckOutHistory

            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = librarycard,
                
            };

            _context.Add(chechouthistory);
            _context.SaveChanges();
        }

        private DateTime GetdefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IscheckedOut(int assetId)
        {
            return _context.CheckOuts
                 .Where(l => l.LibraryAsset.Id == assetId).Any();
        }

        public IEnumerable<CheckOuts> GetAll()
        {
            return _context.CheckOuts;
        }

        public CheckOuts GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(a => a.Id == checkoutId);
        }

        public IEnumerable<CheckOutHistory> GetCheckOutHistory(int id)
        {
            return _context.CheckOutHistories
                .Include(h => h.LibraryAsset)
                .Include(h =>h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                 .Include(h => h.LibraryAsset)
                 .Include(l => l.LibraryCard)
                 .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var partron = _context.Patrons.
                Include(p => p.LibraryCard)
               .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return partron?.FirstName + "" + partron?.LastName;

        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(l => l.LibraryCard)
                .FirstOrDefault(ho => ho.Id == holdId).HoldPlaced;
        }

        public IEnumerable<Holds> GetCurrentHolds(int id)
        {
            return _context.Holds
                 .Include(h => h.LibraryAsset)
                 .Where(h => h.LibraryAsset.Id == id);
        }

        public CheckOuts GetLatestCheckOuts(int assetId)
        {
            return _context.CheckOuts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderBy(o => o.Since)
                .FirstOrDefault();
        }
        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
            
            UpdateAssetStatus(assetId, "Available");
            //remove checkouts
            RemoveExistingCheckouts(assetId);
            //remove checkouthistory
            CloseExistingCheckoutsHistory(assetId, now);
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string v)
        {
            var item = _context
               .LibraryAssets
               .FirstOrDefault(l => l.Id == assetId);

            _context.Update(item);
            item.Status = _context
               .Statuses
               .FirstOrDefault(statuses => statuses.Name == v);
        }

        private void CloseExistingCheckoutsHistory(int assetId, DateTime now)
        {
            var history = _context.CheckOutHistories
                 .FirstOrDefault(h => h.LibraryAsset.Id == assetId && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
                _context.SaveChanges();
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {

            var checkout = _context.CheckOuts
                .FirstOrDefault(c => c.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {

            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int LibraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(s=>s.Status)
                .FirstOrDefault(a => a.Id == assetId);

            var librarycard = _context.LibraryCards
                .FirstOrDefault(l => l.Id == LibraryCardId);
            if(asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Holds
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = librarycard
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetcheckoutById(assetId);

            if (checkout == null)
            {
                return "";
            }

            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(l => l.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + "" + patron.LastName;
               
        }

        private CheckOuts GetcheckoutById(int assetId)
        {
            return _context.CheckOuts
                .Include(c => c.LibraryAsset)
                .Include(l => l.LibraryCard)
                .FirstOrDefault(f => f.LibraryAsset.Id == assetId);
            
        }
    }
}
