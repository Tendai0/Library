using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibtraryContext _context;
        public LibraryAssetService(LibtraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                           .Include(asset => asset.Status)
                           .Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id).Any();
            var isVideo = _context.LibraryAssets.OfType<Video>().Where(v => v.Id == id).Any();

            return isBook ? _context.Books.FirstOrDefault(b => b.Id == id).Author
                : _context.Videos.FirstOrDefault(v => v.Id == id).Director ?? "Unknown";
        }

        public LibraryAsset GetById(int id)
        {
            return _context.LibraryAssets
                           .Include(asset => asset.Status)
                           .Include(asset => asset.Location)
                           .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(asset => asset.Id == id).Location;
                           
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(b => b.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            else return "";
        }

        public string GetTittle(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(a => a.Id == id).Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id);
            return book.Any() ? "Book" : "Video";
        }
    }
}
