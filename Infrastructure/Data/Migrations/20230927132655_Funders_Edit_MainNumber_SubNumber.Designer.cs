﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230927132655_Funders_Edit_MainNumber_SubNumber")]
    partial class Funders_Edit_MainNumber_SubNumber
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Core.Entities.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("AdministrativeFees")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<decimal>("AdvancePayment")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<decimal>("BasicFundingAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<string>("ContractNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("FirstInstallmentBeginningDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FunderId")
                        .HasColumnType("int");

                    b.Property<int>("InstallmentsCount")
                        .HasPrecision(17, 2)
                        .HasColumnType("int");

                    b.Property<double>("InterestRatio")
                        .HasColumnType("float");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastInstallmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalFundingAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<decimal>("TotalInstallmentsAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.HasKey("Id");

                    b.HasIndex("ContractNumber")
                        .IsUnique();

                    b.HasIndex("FunderId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Core.Entities.Funder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactEmployee")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MainNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SubNumber")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.HasIndex("Name", "MainNumber", "SubNumber")
                        .IsUnique()
                        .HasFilter("[MainNumber] IS NOT NULL AND [SubNumber] IS NOT NULL");

                    b.ToTable("Funders");
                });

            modelBuilder.Entity("Core.Entities.InstallmentPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BankRefNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankStatement")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ContractId")
                        .HasColumnType("int");

                    b.Property<decimal>("InstallmentAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<double>("InstallmentNumber")
                        .HasColumnType("float");

                    b.Property<bool>("IsBank")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNet")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("OtherPaymentsAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<decimal>("PaymentAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("RemainingPaymentAmount")
                        .HasPrecision(17, 2)
                        .HasColumnType("decimal(17,2)");

                    b.Property<string>("TransferredBankAccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.ToTable("InstallmentPayments");
                });

            modelBuilder.Entity("Core.Entities.Contract", b =>
                {
                    b.HasOne("Core.Entities.Funder", "Funder")
                        .WithMany("Contracts")
                        .HasForeignKey("FunderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Funder");
                });

            modelBuilder.Entity("Core.Entities.InstallmentPayment", b =>
                {
                    b.HasOne("Core.Entities.Contract", "Contract")
                        .WithMany("InstallmentPayments")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Contract");
                });

            modelBuilder.Entity("Core.Entities.Contract", b =>
                {
                    b.Navigation("InstallmentPayments");
                });

            modelBuilder.Entity("Core.Entities.Funder", b =>
                {
                    b.Navigation("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}
