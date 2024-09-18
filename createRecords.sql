-- Insert data into MethodInfo table
INSERT INTO public."MethodInfo" ("MethodId", "MethodName", "MethodPath", "ApiBaseUri", "ApiPrefix", "ApiTimeout")
VALUES
('c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b', 'proxy', 'proxy', 'http://ptoxy-service', 'test/proxy', 15);

-- Insert data into SubMethodInfo table
INSERT INTO public."SubMethodInfo" ("SubMethodId", "SubMethodName", "SubMethodPath", "MethodId")
VALUES
('d2f4d5c2-3d8c-40b3-9467-fbf7056e2133', 'proxy', 'proxy', 'c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b');

-- Insert data into MethodHeader table
INSERT INTO public."MethodHeaders" ("MethodHeaderId", "HeaderName", "HeaderValue", "Description", "MethodId")
VALUES
('e4c3d5b1-2c3a-4b7d-8c8a-6b6a8c8a4a7a', 'Content-Type', 'application/json', 'Content-Type', 'c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b');

-- Insert data into SystemInfo table
INSERT INTO public."SystemInfo" ("SystemId", "SystemName")
VALUES
('a18e8a5e-7f0e-4a0b-899f-2b7fd633e0fa', 'System A');

-- Insert data into Identification table
INSERT INTO public."Identification" ("IdentificationId", "ClientId", "EnvName", "SystemId")
VALUES
('e1b4a9c5-bb28-4c8c-9f4a-4f56d8a1f6f3', 'testclient', 'prod', 'a18e8a5e-7f0e-4a0b-899f-2b7fd633e0fa');

-- Insert data into Billing config table
INSERT INTO public."BillingConfig"
("BillingConfigId", "PeriodInDays", "RequestLimitPerPeriod", "StartDate", "EndDate", "IdentificationId", "MethodId")
VALUES
('bbd9a9f7-8344-4e9c-8e50-d0d8421dbf22', 1, 25, '2024-01-01', '2024-12-31', 'e1b4a9c5-bb28-4c8c-9f4a-4f56d8a1f6f3', 'c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b');

-- Insert data into NotificationInfo table
INSERT INTO public."NotificationInfo"
("NotificationId", "NotificationLimitPercentage", "Message", "BillingConfigId", "RecipientList", "Subject")
VALUES
('c8f1b6e1-2a4a-4b1d-9e8c-7b5e6e1c4a7b', 50, 'Test Message', 'bbd9a9f7-8344-4e9c-8e50-d0d8421dbf22', 'test1@max.test;test2@max.test', 'Test Subject');
