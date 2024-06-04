﻿// <auto-generated />
using System;
using ExtService.GateWay.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    [DbContext(typeof(GateWayContext))]
    partial class GateWayContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Billing", b =>
                {
                    b.Property<Guid>("BillingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BillingConfigId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("IdentificationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MethodId")
                        .HasColumnType("uuid");

                    b.Property<int>("RequestCount")
                        .HasColumnType("integer");

                    b.Property<int>("RequestLimit")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BillingId");

                    b.HasAlternateKey("BillingConfigId", "StartDate", "EndDate");

                    b.HasIndex("IdentificationId");

                    b.HasIndex("MethodId");

                    b.ToTable("Billing", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.BillingConfig", b =>
                {
                    b.Property<Guid>("BillingConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("IdentificationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MethodId")
                        .HasColumnType("uuid");

                    b.Property<int>("PeriodInDays")
                        .HasColumnType("integer");

                    b.Property<int>("RequestLimitPerPeriod")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BillingConfigId");

                    b.HasIndex("IdentificationId");

                    b.HasIndex("MethodId");

                    b.ToTable("BillingConfig", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Identification", b =>
                {
                    b.Property<Guid>("IdentificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EnvName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uuid");

                    b.HasKey("IdentificationId");

                    b.HasIndex("SystemId");

                    b.ToTable("Identification", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.MethodInfo", b =>
                {
                    b.Property<Guid>("MethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("MethodName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MethodId");

                    b.ToTable("MethodInfo", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.NotificationInfo", b =>
                {
                    b.Property<Guid>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BillingConfigId")
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NotificationLimitPercentage")
                        .HasColumnType("integer");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uuid");

                    b.HasKey("NotificationId");

                    b.HasIndex("BillingConfigId");

                    b.HasIndex("SystemId");

                    b.ToTable("NotificationInfo", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SystemInfo", b =>
                {
                    b.Property<Guid>("SystemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("SystemName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SystemId");

                    b.ToTable("SystemInfo", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.UserInfo", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DomainName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId");

                    b.HasIndex("SystemId");

                    b.ToTable("UserInfo", (string)null);
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Billing", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.BillingConfig", "BillingConfig")
                        .WithMany("BillingRecords")
                        .HasForeignKey("BillingConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExtService.GateWay.DBContext.DBModels.Identification", "Identification")
                        .WithMany("BillingSet")
                        .HasForeignKey("IdentificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExtService.GateWay.DBContext.DBModels.MethodInfo", "Method")
                        .WithMany("BillingSet")
                        .HasForeignKey("MethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BillingConfig");

                    b.Navigation("Identification");

                    b.Navigation("Method");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.BillingConfig", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.Identification", "Identification")
                        .WithMany("BillingConfigSet")
                        .HasForeignKey("IdentificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExtService.GateWay.DBContext.DBModels.MethodInfo", "Method")
                        .WithMany("BillingConfigSet")
                        .HasForeignKey("MethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Identification");

                    b.Navigation("Method");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Identification", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.SystemInfo", "SystemInfo")
                        .WithMany("IdentificationSet")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SystemInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.NotificationInfo", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.BillingConfig", "BillingConfig")
                        .WithMany("NotificationInfoSet")
                        .HasForeignKey("BillingConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExtService.GateWay.DBContext.DBModels.SystemInfo", "SystemInfo")
                        .WithMany("NotificationInfoSet")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BillingConfig");

                    b.Navigation("SystemInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.UserInfo", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.SystemInfo", "SystemInfo")
                        .WithMany("Users")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SystemInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.BillingConfig", b =>
                {
                    b.Navigation("BillingRecords");

                    b.Navigation("NotificationInfoSet");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Identification", b =>
                {
                    b.Navigation("BillingConfigSet");

                    b.Navigation("BillingSet");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.MethodInfo", b =>
                {
                    b.Navigation("BillingConfigSet");

                    b.Navigation("BillingSet");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SystemInfo", b =>
                {
                    b.Navigation("IdentificationSet");

                    b.Navigation("NotificationInfoSet");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
