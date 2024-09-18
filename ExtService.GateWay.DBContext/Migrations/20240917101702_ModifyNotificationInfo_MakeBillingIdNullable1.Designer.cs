﻿// <auto-generated />
using System;
using ExtService.GateWay.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    [DbContext(typeof(GateWayContext))]
    [Migration("20240917101702_ModifyNotificationInfo_MakeBillingIdNullable1")]
    partial class ModifyNotificationInfo_MakeBillingIdNullable1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Billing", b =>
                {
                    b.Property<Guid>("BillingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

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

                    b.ToTable("Billing", t =>
                        {
                            t.HasCheckConstraint("CK_RequestCount_GTE_Zero", "\"RequestCount\" >= 0");

                            t.HasCheckConstraint("CK_RequestLimit_Greater_Than_Zero", "\"RequestLimit\" > 0");

                            t.HasCheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                        });
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.BillingConfig", b =>
                {
                    b.Property<Guid>("BillingConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

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

                    b.ToTable("BillingConfig", t =>
                        {
                            t.HasCheckConstraint("CK_PeriodInDays_Greater_Than_Zero", "\"PeriodInDays\" > 0");

                            t.HasCheckConstraint("CK_RequestLimitPerPeriod_Greater_Than_Zero", "\"RequestLimitPerPeriod\" > 0");

                            t.HasCheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                        });
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Identification", b =>
                {
                    b.Property<Guid>("IdentificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

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

                    b.ToTable("Identification");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.MethodHeaders", b =>
                {
                    b.Property<Guid>("MethodHeaderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HeaderName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HeaderValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("MethodId")
                        .HasColumnType("uuid");

                    b.HasKey("MethodHeaderId");

                    b.HasIndex("MethodId");

                    b.ToTable("MethodHeaders");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.MethodInfo", b =>
                {
                    b.Property<Guid>("MethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("ApiBaseUri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ApiPrefix")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ApiTimeout")
                        .HasColumnType("integer");

                    b.Property<string>("MethodName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MethodPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MethodId");

                    b.ToTable("MethodInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.NotificationInfo", b =>
                {
                    b.Property<Guid>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("BillingConfigId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BillingId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NotificationLimitPercentage")
                        .HasColumnType("integer");

                    b.Property<string>("RecipientList")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("NotificationId");

                    b.HasIndex("BillingConfigId");

                    b.HasIndex("BillingId");

                    b.ToTable("NotificationInfo", t =>
                        {
                            t.HasCheckConstraint("CK_NotificationLimitPercentage_Range", "\"NotificationLimitPercentage\" > 0 AND \"NotificationLimitPercentage\" < 100");
                        });
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SubMethodInfo", b =>
                {
                    b.Property<Guid>("SubMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("MethodId")
                        .HasColumnType("uuid");

                    b.Property<string>("SubMethodName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SubMethodPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SubMethodId");

                    b.HasIndex("MethodId");

                    b.ToTable("SubMethodInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SystemInfo", b =>
                {
                    b.Property<Guid>("SystemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("SystemName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SystemId");

                    b.ToTable("SystemInfo");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.UserInfo", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

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

                    b.ToTable("UserInfo");
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

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.MethodHeaders", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.MethodInfo", "Method")
                        .WithMany("MethodHeaders")
                        .HasForeignKey("MethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Method");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.NotificationInfo", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.BillingConfig", "BillingConfig")
                        .WithMany("NotificationInfoSet")
                        .HasForeignKey("BillingConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExtService.GateWay.DBContext.DBModels.Billing", "Billing")
                        .WithMany("NotificationInfoSet")
                        .HasForeignKey("BillingId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Billing");

                    b.Navigation("BillingConfig");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SubMethodInfo", b =>
                {
                    b.HasOne("ExtService.GateWay.DBContext.DBModels.MethodInfo", "Method")
                        .WithMany("SubMethodInfoSet")
                        .HasForeignKey("MethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Method");
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

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.Billing", b =>
                {
                    b.Navigation("NotificationInfoSet");
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

                    b.Navigation("MethodHeaders");

                    b.Navigation("SubMethodInfoSet");
                });

            modelBuilder.Entity("ExtService.GateWay.DBContext.DBModels.SystemInfo", b =>
                {
                    b.Navigation("IdentificationSet");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
