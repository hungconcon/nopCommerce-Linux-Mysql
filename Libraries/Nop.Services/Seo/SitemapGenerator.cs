using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Catalog;
using Nop.Services.Topics;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : BaseSitemapGenerator, ISitemapGenerator
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly CommonSettings _commonSettings;
        private readonly IWebHelper _webHelper;

        public SitemapGenerator(ICategoryService categoryService,
            IProductService productService, IManufacturerService manufacturerService,
            ITopicService topicService, CommonSettings commonSettings, IWebHelper webHelper)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._commonSettings = commonSettings;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        protected override void GenerateUrlNodes()
        {
            if (_commonSettings.SitemapIncludeCategories)
            {
                WriteCategories(0);
            }

            if (_commonSettings.SitemapIncludeManufacturers)
            {
                WriteManufacturers();
            }

            if (_commonSettings.SitemapIncludeProducts)
            {
                WriteProducts();
            }

            if (_commonSettings.SitemapIncludeTopics)
            {
                WriteTopics();
            }
        }

        private void WriteCategories(int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, false);
            foreach (var category in categories)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}c/{1}/{2}", _webHelper.GetStoreLocation(false), category.Id, category.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = category.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);

                WriteCategories(category.Id);
            }
        }

        private void WriteManufacturers()
        {
            var manufacturers = _manufacturerService.GetAllManufacturers(false);
            foreach (var manufacturer in manufacturers)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}m/{1}/{2}", _webHelper.GetStoreLocation(false), manufacturer.Id, manufacturer.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = manufacturer.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteProducts()
        {
            var products = _productService.GetAllProducts(false);
            foreach (var product in products)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}p/{1}/{2}", _webHelper.GetStoreLocation(false), product.Id, product.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = product.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteTopics()
        {
            var topics = _topicService.GetAllTopics().ToList().FindAll(t => t.IncludeInSitemap);
            foreach (var topic in topics)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}t/{1}", _webHelper.GetStoreLocation(false), topic.SystemName.ToLowerInvariant());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = DateTime.UtcNow;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }
    }
}
