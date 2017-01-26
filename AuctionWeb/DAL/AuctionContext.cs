using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using AuctionWeb.Models;
using System.Data.Entity.Migrations;

namespace AuctionWeb.DAL
{
    public class AuctionContext: DbContext
    {
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<AuctionOffer> AuctionOffera { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Auction>()
                .HasOptional(a => a.Gallery)
                .WithRequired(g => g.Auction);

            modelBuilder.Entity<Image>()
                    .HasRequired<Gallery>(g => g.Gallery) 
                    .WithMany(g => g.Images);

            modelBuilder.Entity<AuctionOffer>()
                   .HasRequired<Image>(g => g.Image) 
                   .WithMany(g => g.AuctionOffers);

            //modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}