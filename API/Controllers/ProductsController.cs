using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using BrunoZell.ModelBinding;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MPesa;
using Environment = MPesa.Environment;

namespace API.Controllers
{
    public class RequestExample
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPayments _payments;

        public ProductsController(IGenericRepository<Product> productRepository,
            IGenericRepository<ProductBrand> productBrandRepository,
            IGenericRepository<ProductType> productTypeRepository, IMapper mapper, IConfiguration configuration,
            IPayments payments)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
            _configuration = configuration;
            _payments = payments;
        }

        // [HttpGet]
        // public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
        // {
        //     var products = await _productRepository.ListAllAsync();
        //     return Ok(products);
        // }

        /*
        [HttpPost]
        public async Task<ActionResult<object>> Upload([ModelBinder(BinderType = typeof(JsonModelBinder))]
            RequestExample command,
            [FromForm] IList<IFormFile> files)
        {
            return Ok();
        }*/


        [HttpGet]
        public async Task<ActionResult<Pagination<ProductsDto>>> GetProducts(
            [FromQuery] ProductSpecParams productSpecParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productSpecParams);

            var totalItems = await _productRepository.CountAsync(countSpec);

            var products = await _productRepository.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsDto>>(products);

            return Ok(new Pagination<ProductsDto>(productSpecParams.PageIndex, productSpecParams.PageSize, totalItems,
                data));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductsDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepository.GetEntityWithSpec(spec);

            return _mapper.Map<Product, ProductsDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            return Ok(await _productTypeRepository.ListAllAsync());
        }

        [HttpPost("pay")]
        public async Task<ActionResult<PaymentRequest>> PayBasket([FromBody] PaymentRequest paymentReq)
        {
            var response = await _payments.DoC2B(paymentReq);
            return response.IsSuccessfully ? paymentReq : Ok(new ApiException(int.Parse(response.Code), response.Description));
        }
    }
}