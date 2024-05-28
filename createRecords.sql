-- Insert data into SystemInfo table
INSERT INTO public."SystemInfo" ("SystemId", "SystemName")
VALUES
('a18e8a5e-7f0e-4a0b-899f-2b7fd633e0fa', 'System A'),
('b3b2c408-c5e7-4d45-9f8b-705003107cc6', 'System B');

-- Insert data into MethodInfo table
INSERT INTO public."MethodInfo" ("MethodId", "MethodName")
VALUES
('c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b', 'suggestion'),
('d2f4d5c2-3d8c-40b3-9467-fbf7056e2133', 'cleaner');

-- Insert data into Identification table
INSERT INTO public."Identification" ("IdentificationId", "ClientId", "EnvName", "SystemId")
VALUES
('e1b4a9c5-bb28-4c8c-9f4a-4f56d8a1f6f3', 'testclient', 'prod', 'a18e8a5e-7f0e-4a0b-899f-2b7fd633e0fa');

-- Insert data into Billing table
INSERT INTO public."Billing" ("BillingId", "RequestLimit", "RequestCount", "StartDate", "EndDate", "IdentificationId", "MethodId")
VALUES
('bbd9a9f7-8344-4e9c-8e50-d0d8421dbf22', 100, 0, '2024-01-01', '2024-12-31', 'e1b4a9c5-bb28-4c8c-9f4a-4f56d8a1f6f3', 'c6c72f43-ec44-4d77-9fb8-1bbf7b6c7a9b'),
('c2d3b19e-5c7a-4cb9-8bb7-61b4a3b5a7dd', 200, 0, '2024-01-01', '2024-12-31', 'e1b4a9c5-bb28-4c8c-9f4a-4f56d8a1f6f3', 'd2f4d5c2-3d8c-40b3-9467-fbf7056e2133');