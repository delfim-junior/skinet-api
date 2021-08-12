using Core.Entities;

namespace Core.Specifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productSpecParams)
            : base(x =>
                (string.IsNullOrEmpty(productSpecParams.Search) ||
                 x.Name.ToLower().Contains(productSpecParams.Search)) &&
                (!productSpecParams.BrandId.HasValue || x.ProductBrand.Id == productSpecParams.BrandId) &&
                (!productSpecParams.TypeId.HasValue || x.ProductType.Id == productSpecParams.TypeId))
        {
        }
    }
}