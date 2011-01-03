//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class ProductAttributeService : IProductAttributeService
    {
        #region Constants
        private const string PRODUCTATTRIBUTES_ALL_KEY = "Nop.productattribute.all";
        private const string PRODUCTATTRIBUTES_BY_ID_KEY = "Nop.productattribute.id-{0}";
        private const string PRODUCTVARIANTATTRIBUTES_ALL_KEY = "Nop.productvariantattribute.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTES_BY_ID_KEY = "Nop.productvariantattribute.id-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY = "Nop.productvariantattributevalue.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY = "Nop.productvariantattributevalue.id-{0}";
        private const string PRODUCTATTRIBUTES_PATTERN_KEY = "Nop.productattribute.";
        private const string PRODUCTVARIANTATTRIBUTES_PATTERN_KEY = "Nop.productvariantattribute.";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY = "Nop.productvariantattributevalue.";
        #endregion

        #region Fields

        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductVariantAttribute> _productVariantAttributeRepository;
        private readonly IRepository<ProductVariantAttributeCombination> _productVariantAttributeCombinationRepository;
        private readonly IRepository<ProductVariantAttributeValue> _productVariantAttributeValueRepository;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productAttributeRepository">Product attribute repository</param>
        /// <param name="productVariantAttributeRepository">Product variant attribute mapping repository</param>
        /// <param name="productVariantAttributeCombinationRepository">Product variant attribute combination repository</param>
        /// <param name="productVariantAttributeValueRepository">Product variant attribute value repository</param>
        public ProductAttributeService(ICacheManager cacheManager,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductVariantAttribute> productVariantAttributeRepository,
            IRepository<ProductVariantAttributeCombination> productVariantAttributeCombinationRepository,
            IRepository<ProductVariantAttributeValue> productVariantAttributeValueRepository
            )
        {
            this._cacheManager = cacheManager;
            this._productAttributeRepository = productAttributeRepository;
            this._productVariantAttributeRepository = productVariantAttributeRepository;
            this._productVariantAttributeCombinationRepository = productVariantAttributeCombinationRepository;
            this._productVariantAttributeValueRepository = productVariantAttributeValueRepository;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public void DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                return;

            _productAttributeRepository.Delete(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <returns>Product attribute collection</returns>
        public IList<ProductAttribute> GetAllProductAttributes()
        {
            string key = PRODUCTATTRIBUTES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _productAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = query.ToList();
                return productAttributes;
            });
        }

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        public ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTES_BY_ID_KEY, productAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var pa = _productAttributeRepository.GetById(productAttributeId);
                return pa;
            });
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Insert(productAttribute);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Update(productAttribute);
            
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        #endregion

        #region Product variant attributes mappings (ProductVariantAttribute)

        /// <summary>
        /// Deletes a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute mapping</param>
        public void DeleteProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                return;

            _productVariantAttributeRepository.Delete(productVariantAttribute);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets product variant attribute mappings by product identifier
        /// </summary>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public IList<ProductVariantAttribute> GetProductVariantAttributesByProductVariantId(int productVariantId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTES_ALL_KEY, productVariantId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _productVariantAttributeRepository.Table
                            orderby pva.DisplayOrder
                            where pva.ProductVariantId == productVariantId
                            select pva;
                var productVariantAttributes = query.ToList();
                return productVariantAttributes;
            });
        }

        /// <summary>
        /// Gets a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping</returns>
        public ProductVariantAttribute GetProductVariantAttributeById(int productVariantAttributeId)
        {
            if (productVariantAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTES_BY_ID_KEY, productVariantAttributeId);
            return _cacheManager.Get(key, () =>
            {
                return _productVariantAttributeRepository.GetById(productVariantAttributeId);
            });
        }

        /// <summary>
        /// Inserts a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public void InsertProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Insert(productVariantAttribute);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public void UpdateProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Update(productVariantAttribute);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        #endregion

        #region Product variant attribute values (ProductVariantAttributeValue)

        /// <summary>
        /// Deletes a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">Product variant attribute value</param>
        public void DeleteProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                return;

            _productVariantAttributeValueRepository.Delete(productVariantAttributeValue);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets product variant attribute values by product identifier
        /// </summary>
        /// <param name="productVariantAttributeId">The product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public IList<ProductVariantAttributeValue> GetProductVariantAttributeValues(int productVariantAttributeId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY, productVariantAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pvav in _productVariantAttributeValueRepository.Table
                            orderby pvav.DisplayOrder
                            where pvav.ProductVariantAttributeId == productVariantAttributeId
                            select pvav;
                var productVariantAttributeValues = query.ToList();
                return productVariantAttributeValues;
            });
        }

        /// <summary>
        /// Gets a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Product variant attribute value</returns>
        public ProductVariantAttributeValue GetProductVariantAttributeValueById(int productVariantAttributeValueId)
        {
            if (productVariantAttributeValueId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY, productVariantAttributeValueId);
            return _cacheManager.Get(key, () =>
            {
                var pvav = _productVariantAttributeValueRepository.GetById(productVariantAttributeValueId);
                return pvav;
            });
        }

        /// <summary>
        /// Inserts a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public void InsertProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Insert(productVariantAttributeValue);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public void UpdateProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Update(productVariantAttributeValue);

            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
        }

        #endregion

        #region Product variant attribute compinations (ProductVariantAttributeCombination)

        /// <summary>
        /// Deletes a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public void DeleteProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                return;

            _productVariantAttributeCombinationRepository.Delete(combination);
        }

        /// <summary>
        /// Gets all product variant attribute combinations
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant attribute combination collection</returns>
        public IList<ProductVariantAttributeCombination> GetAllProductVariantAttributeCombinations(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<ProductVariantAttributeCombination>();

            var query = from pvac in _productVariantAttributeCombinationRepository.Table
                        orderby pvac.Id
                        where pvac.ProductVariantId == productVariantId
                        select pvac;
            var combinations = query.ToList();
            return combinations;
        }

        /// <summary>
        /// Gets a product variant attribute combination
        /// </summary>
        /// <param name="productVariantAttributeCombinationId">Product variant attribute combination identifier</param>
        /// <returns>Product variant attribute combination</returns>
        public ProductVariantAttributeCombination GetProductVariantAttributeCombinationById(int productVariantAttributeCombinationId)
        {
            if (productVariantAttributeCombinationId == 0)
                return null;
            
            var combination = _productVariantAttributeCombinationRepository.GetById(productVariantAttributeCombinationId);
            return combination;
        }

        /// <summary>
        /// Inserts a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public void InsertProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productVariantAttributeCombinationRepository.Insert(combination);
        }

        /// <summary>
        /// Updates a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public void UpdateProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productVariantAttributeCombinationRepository.Update(combination);
        }

        #endregion

        #endregion
    }
}
