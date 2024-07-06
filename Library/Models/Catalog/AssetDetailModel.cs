using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Catalog
{
    public class AssetDetailModel
    {
        public int AssetId { get; set; }
        public string  Tittle { get; set; }
        public string AuthrOrDirector { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public string DeweyCallNumber { get; set; }
        public string  Status { get; set; }
        public decimal Cost { get; set; }
        public string PatronName { get; set; }
        public string CurrentLocation { get; set; }
        public string  ImageUrl { get; set; }
        public CheckOuts LatestCheckout { get; set; }
        public IEnumerable<CheckOutHistory> CheckOutHistory { get; set; }
        public IEnumerable<AssetHolds> CurrentHolds { get; set; }
    }

    public class AssetHolds
    {
        public string PatronName { get; set; }
        public string  HoldPlaced { get; set; }

    }
}
