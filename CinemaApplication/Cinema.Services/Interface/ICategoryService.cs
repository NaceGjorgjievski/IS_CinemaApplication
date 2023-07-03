using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface ICategoryService
    {
        List<Category> GetAllProducts();
        Category GetDetailsForProduct(Guid? id);
        void CreateNewCategory(Category p);
        void UpdeteExistingCategory(Category p);
        void DeleteCategory(Guid id);
        
    }
}
