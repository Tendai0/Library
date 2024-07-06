using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
   public interface ICheckout
    {
        IEnumerable<CheckOuts> GetAll();
        CheckOuts GetById(int checkoutId);
        CheckOuts GetLatestCheckOuts(int assetId);
        void Add(CheckOuts newcheckOuts);
        void CheckOutItem(int assetId, int LibraryCardId);
        void CheckInItem(int assetId);
        IEnumerable<CheckOutHistory> GetCheckOutHistory(int id);
        string GetCurrentCheckoutPatron(int assetId);

        void PlaceHold(int assetId, int LibraryCardId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        IEnumerable<Holds> GetCurrentHolds(int id);
        bool IscheckedOut(int id);

        void MarkLost(int assetId);
        void MarkFound(int assetId);

    }
}
