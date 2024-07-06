using Library.Models.Catalog;
using Library.Models.Checkout;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        ILibraryAsset _asset;
        ICheckout _checkOuts;
        public CatalogController(ILibraryAsset asset, ICheckout checkOuts)
            {
            _asset = asset;
            _checkOuts = checkOuts;
            }
        public IActionResult Index()
        {
            var assetModel = _asset.GetAll();

            var ListingResult = assetModel
                 .Select(result => new AssetIndexListingModel
                 {
                     Id = result.Id,
                     ImageUrl = result.ImageUrl,
                     AuthorOrDirector = _asset.GetAuthorOrDirector(result.Id),
                     DeweyCallNumber = _asset.GetDeweyIndex(result.Id),
                     Tittle = result.Title,
                     Type = _asset.GetType(result.Id)
                 });

            var model =  new AssetIndexModel()
                {
                    Assets = ListingResult

                };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var asset = _asset.GetById(id);
            var currentHolds = _checkOuts.GetCurrentHolds(id)
                .Select(a => new AssetHolds
                {
                    HoldPlaced = _checkOuts.GetCurrentHoldPlaced(a.Id).ToString("d"),
                    PatronName = _checkOuts.GetCurrentHoldPatronName(a.Id)
                });
                
               

            var model = new AssetDetailModel
            {
                AssetId = id,
                Tittle = asset.Title,

                Type = _asset.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthrOrDirector = _asset.GetAuthorOrDirector(id),
                CurrentLocation = _asset.GetCurrentLocation(id).Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                CheckOutHistory = _checkOuts.GetCheckOutHistory(id),
                ISBN = _asset.GetIsbn(id),
                LatestCheckout = _checkOuts.GetLatestCheckOuts(id),
                PatronName = _checkOuts.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = _asset.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = asset.Id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkOuts.IscheckedOut(id)

            };

            return View(model);
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkOuts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceCheckOut(int assetId, int LibraryCardId)
        {
            _checkOuts.CheckOutItem(assetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = assetId});
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int LibraryCardId)
        {
            _checkOuts.PlaceHold(assetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult Hold(int Id)
        {
            var asset = _asset.GetById(Id);
            var model = new CheckoutModel
            {
                AssetId = asset.Id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkOuts.IscheckedOut(Id),
                HoldCount = _checkOuts.GetCurrentHolds(Id).Count()
            };
            return View(model);

        }

        public IActionResult CheckIn(int id)
        {
            _checkOuts.CheckInItem(id);
            return RedirectToAction("Detail", new {id = id});

        }

    }
}
