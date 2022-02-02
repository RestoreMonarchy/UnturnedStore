﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Website.Shared.Enums;

namespace Website.Shared.Models.Database
{
    public class MProduct
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        [MaxLength(255)]
        public string GithubUrl { get; set; }
        [Required]
        public int ImageId { get; set; }
        [Range(0, 1000)]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }
        public int SellerId { get; set; }
        public int? AdminId { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsLoaderEnabled { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }

        public int TotalDownloadsCount { get; set; }
        public byte AverageRating { get; set; }
        public int RatingsCount { get; set; }

        public Seller Seller { get; set; }
        public UserInfo Customer { get; set; }

        public List<MProductTab> Tabs { get; set; }
        public List<MProductMedia> Medias { get; set; }
        public List<MProductReview> Reviews { get; set; }
        public List<MBranch> Branches { get; set; }

        public string GetDescription()
        {
            if (Description.Length > 100)
                return Description.Substring(0, 100).TrimEnd(' ') + "...";
            return Description;
        }

        public static MProduct FromProduct(MProduct product)
        {
            return new MProduct()
            {
                Id = product.Id,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
                GithubUrl = product.GithubUrl,
                Name = product.Name,
                ImageId = product.ImageId,
                SellerId = product.SellerId,
                Status = product.Status,
                AdminId = product.AdminId,
                IsLoaderEnabled = product.IsLoaderEnabled,
                IsEnabled = product.IsEnabled,
                LastUpdate = product.LastUpdate,
                CreateDate = product.CreateDate
            };
        }
    }
}
