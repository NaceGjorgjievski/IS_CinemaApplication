using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public void CreateNewCategory(Category p)
        {
            this._categoryRepository.Insert(p);
        }

        public void DeleteCategory(Guid id)
        {
            var category = this.GetDetailsForProduct(id);
            this._categoryRepository.Delete(category);
        }

        public List<Category> GetAllProducts()
        {
            return this._categoryRepository.GetAll().ToList();
        }

        public Category GetDetailsForProduct(Guid? id)
        {
            return this._categoryRepository.Get(id);
        }

        public void UpdeteExistingCategory(Category p)
        {
            this._categoryRepository.Update(p);
        }
    }
}
