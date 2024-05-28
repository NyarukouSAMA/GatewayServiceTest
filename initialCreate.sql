CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "MethodInfo" (
    "MethodId" uuid NOT NULL,
    "MethodName" text NOT NULL,
    CONSTRAINT "PK_MethodInfo" PRIMARY KEY ("MethodId")
);

CREATE TABLE "SystemInfo" (
    "SystemId" uuid NOT NULL,
    "SystemName" text NOT NULL,
    CONSTRAINT "PK_SystemInfo" PRIMARY KEY ("SystemId")
);

CREATE TABLE "Identification" (
    "IdentificationId" uuid NOT NULL,
    "ClientId" text NOT NULL,
    "EnvName" text NOT NULL,
    "SystemId" uuid NOT NULL,
    CONSTRAINT "PK_Identification" PRIMARY KEY ("IdentificationId"),
    CONSTRAINT "FK_Identification_SystemInfo_SystemId" FOREIGN KEY ("SystemId") REFERENCES "SystemInfo" ("SystemId") ON DELETE CASCADE
);

CREATE TABLE "UserInfo" (
    "UserId" uuid NOT NULL,
    "DomainName" text NOT NULL,
    "FirstName" text NOT NULL,
    "LastName" text NOT NULL,
    "Email" text NOT NULL,
    "State" integer NOT NULL,
    "SystemId" uuid NOT NULL,
    CONSTRAINT "PK_UserInfo" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_UserInfo_SystemInfo_SystemId" FOREIGN KEY ("SystemId") REFERENCES "SystemInfo" ("SystemId") ON DELETE CASCADE
);

CREATE TABLE "Billing" (
    "BillingId" uuid NOT NULL,
    "RequestLimit" integer NOT NULL,
    "RequestCount" integer NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "IdentificationId" uuid NOT NULL,
    "MethodId" uuid NOT NULL,
    CONSTRAINT "PK_Billing" PRIMARY KEY ("BillingId"),
    CONSTRAINT "FK_Billing_Identification_IdentificationId" FOREIGN KEY ("IdentificationId") REFERENCES "Identification" ("IdentificationId") ON DELETE CASCADE,
    CONSTRAINT "FK_Billing_MethodInfo_MethodId" FOREIGN KEY ("MethodId") REFERENCES "MethodInfo" ("MethodId") ON DELETE CASCADE
);

CREATE TABLE "NotificationInfo" (
    "NotificationId" uuid NOT NULL,
    "NotificationLimitPercentage" integer NOT NULL,
    "Message" text NOT NULL,
    "SystemId" uuid NOT NULL,
    "BillingId" uuid NOT NULL,
    CONSTRAINT "PK_NotificationInfo" PRIMARY KEY ("NotificationId"),
    CONSTRAINT "FK_NotificationInfo_Billing_BillingId" FOREIGN KEY ("BillingId") REFERENCES "Billing" ("BillingId") ON DELETE CASCADE,
    CONSTRAINT "FK_NotificationInfo_SystemInfo_SystemId" FOREIGN KEY ("SystemId") REFERENCES "SystemInfo" ("SystemId") ON DELETE CASCADE
);

CREATE INDEX "IX_Billing_IdentificationId" ON "Billing" ("IdentificationId");

CREATE INDEX "IX_Billing_MethodId" ON "Billing" ("MethodId");

CREATE INDEX "IX_Identification_SystemId" ON "Identification" ("SystemId");

CREATE INDEX "IX_NotificationInfo_BillingId" ON "NotificationInfo" ("BillingId");

CREATE INDEX "IX_NotificationInfo_SystemId" ON "NotificationInfo" ("SystemId");

CREATE INDEX "IX_UserInfo_SystemId" ON "UserInfo" ("SystemId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240522151731_InitialCreate', '6.0.30');

COMMIT;

