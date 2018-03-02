using Auth.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Data.Mappers
{
    public class OrganisationAccountMap : IEntityTypeConfiguration<OrganisationAccount>
    {
        public void Configure(EntityTypeBuilder<OrganisationAccount> modelBuilder) {

            modelBuilder.ToTable("OrganisationAccount");

            modelBuilder.HasKey(t => t.Id);
            modelBuilder.Property<string>("Name").HasMaxLength(250);            
            modelBuilder.Property<string>("Region").HasMaxLength(50);
            modelBuilder.Property<string>("Data").HasMaxLength(500);
            modelBuilder.Property<DateTime>("AddedDate");
            modelBuilder.Property<string>("AddedBy").HasMaxLength(250);
            modelBuilder.Property<DateTime?>("ModifiedDate");
            modelBuilder.Property<string>("ModifiedBy");


            modelBuilder.HasMany(t => t.Users)
                .WithOne(t => t.OrganisationAccount)
                .HasForeignKey(t => t.OrganisationId).OnDelete(DeleteBehavior.Restrict);           
        }

    }
}
