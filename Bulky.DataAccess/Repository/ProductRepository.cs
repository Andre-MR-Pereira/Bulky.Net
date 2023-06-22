using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private ApplicationDbContext _db;

		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Product product)
		{
			/*var objFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
			if(objFromDb != null)
			{
				objFromDb.Title = product.Title;
				objFromDb.Description = product.Description;
				objFromDb.ISBN = product.ISBN;
				objFromDb.Price = product.Price;
				objFromDb.Price50 = product.Price50;
				objFromDb.Price100 = product.Price100;
				objFromDb.CategoryId = product.CategoryId;
				objFromDb.Author = product.Author;
				objFromDb.ProductImages = product.ProductImages;
			}*/
			_db.Products.Update(product);
		}
	}
}
