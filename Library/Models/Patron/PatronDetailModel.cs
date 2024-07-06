using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Patron
{
    public class PatronDetailModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName  {
            get { return FirstName + "" + LastName; }

                }
        public int LibraryCardId { get; set; }
        public string Address { get; set; }
        public DateTime MemberSince { get; set; }
        public string  Telephone { get; set; }
        public string HomelibraryBranch { get; set; }
        public Decimal OverdueFees { get; set; }
        public IEnumerable <CheckOuts> AssetCheckOut { get; set; }
        public IEnumerable<CheckOutHistory> CheckOutHistory { get; set; }
        public IEnumerable<Holds> Holds { get; set; }
    }
}
