USE [LOLATEST]
GO
INSERT [dbo].[Languages] ([ID], [Name], [InitialsLanguage], [IdLanguage], [DotNetCulture]) VALUES (N'dac66418-9148-49d8-8d45-08609051ddbf', N'French', N'fr', 3, N'fr-FR')
GO
INSERT [dbo].[Languages] ([ID], [Name], [InitialsLanguage], [IdLanguage], [DotNetCulture]) VALUES (N'd8619ee5-abe0-4c90-a5db-1e46d3df9ef3', N'German', N'de', 5, N'de-DE')
GO
INSERT [dbo].[Languages] ([ID], [Name], [InitialsLanguage], [IdLanguage], [DotNetCulture]) VALUES (N'92440952-f542-431a-82e8-3284c02471b2', N'Italian', N'it', 2, N'it-IT')
GO
INSERT [dbo].[Languages] ([ID], [Name], [InitialsLanguage], [IdLanguage], [DotNetCulture]) VALUES (N'f6d59604-aec0-49c6-bd08-43a39fd7d7ce', N'Spanish', N'es', 4, N'es-ES')
GO
INSERT [dbo].[Languages] ([ID], [Name], [InitialsLanguage], [IdLanguage], [DotNetCulture]) VALUES (N'3d7906c6-db11-4410-987c-7770549ccb63', N'English', N'en', 1, N'en-GB')
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'2716582b-9689-495a-b669-0bc67279fabd', N'romina', N'+cLRod5w12iFprTBOM/78Q==', N'Romina', N'Giardi', N'f6d59604-aec0-49c6-bd08-43a39fd7d7ce', N'rgiardi@a.com', NULL, 1, NULL, NULL, N'2716582b-9689-495a-b669-0bc67279fabd', CAST(N'2019-08-07T09:23:24.807' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2019-08-07T09:23:24.807' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'7d1b1b7c-b86d-44a2-a3ff-168127a5d579', N'Administrator', N'Y2orBaPXAyi4NrsysQhr8tQhw1M2qZsrm1xxRqqYeOs=', N'Admin', N'istrator', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'ec101b5b-b043-48aa-ba65-1af3a924c27c', N'userDE', N'p5Cy+RxrgiwOYYBaqFTMOg==', N'user', N'DE', N'd8619ee5-abe0-4c90-a5db-1e46d3df9ef3', N'rgiardifom@gmail.com', NULL, 1, NULL, NULL, N'ec101b5b-b043-48aa-ba65-1af3a924c27c', CAST(N'2020-05-04T10:52:00.597' AS DateTime), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'0f428ffc-47ed-42dd-8dba-257460f765ee', N'tewst', N'8g4Kzt5QdNj9Obc2psp5qg==', N'test', N'test', N'd8619ee5-abe0-4c90-a5db-1e46d3df9ef3', NULL, NULL, 1, NULL, NULL, N'0f428ffc-47ed-42dd-8dba-257460f765ee', CAST(N'2020-04-22T08:53:05.053' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2020-04-22T08:53:05.053' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'32fd3016-e4de-4331-b044-2cd25a032d55', N'usr00043', N'rominaFom', N'usr00043', N'usr00043', N'3d7906c6-db11-4410-987c-7770549ccb63', N'rgiardifom@gmail.com', NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'c77f88c7-1130-40b9-970e-3909aad4c0df', N'usr00102', N'DHs7+agIneBB+b/+1elKlQ==', N'usr00102', N'usr00102', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'7adf67c3-a2f8-498c-8b12-3ce1e123ef44', N'userFR', N'p5Cy+RxrgiwOYYBaqFTMOg==', N'user', N'FR', N'dac66418-9148-49d8-8d45-08609051ddbf', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'1b9cb7b6-702e-4fd1-949d-479767006326', N'mrossi', N'u/+9Y2zYgtjcN2EWnO/Reg==', N'Mario', N'Rossi', N'92440952-f542-431a-82e8-3284c02471b2', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'9bd41e60-461b-4117-9bc3-47d84f8f2ab6', N'newUsrIT', N'+cLRod5w12j3l2iRfT1DOA==', N'IT', N'User', N'92440952-f542-431a-82e8-3284c02471b2', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'982d2e9b-b77b-4477-a12b-609cd1d50339', N'WorkshopManagerTest', N'8g4Kzt5QdNhO5jqBx/9Ryg==', N'Capo', N'Officina Mod', N'dac66418-9148-49d8-8d45-08609051ddbf', N's@c.com', NULL, 1, NULL, NULL, N'982d2e9b-b77b-4477-a12b-609cd1d50339', CAST(N'2020-04-22T08:48:50.627' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2020-04-22T08:48:50.627' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'd478eb36-03de-4b10-add1-6c3a9c882086', N'usr00008', N'DHs7+agIneBB+b/+1elKlQ==', N'usr00008', N'usr00008', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'd1646002-b53d-4b2c-9310-775355cf0901', N'user2020', N'u/+9Y2zYgtjcN2EWnO/Reg==', N'new test', N'user', N'dac66418-9148-49d8-8d45-08609051ddbf', NULL, NULL, 1, NULL, NULL, N'd1646002-b53d-4b2c-9310-775355cf0901', CAST(N'2020-05-04T10:33:06.037' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2020-05-04T10:33:06.037' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'd1da25b6-4644-47f8-aed2-782555f72202', N'Pippolo', N'vaWd7Twnd1D2DEURSFc3/FrEXpoIpo+Y', N'Ciccio', N'Pasticcio', N'92440952-f542-431a-82e8-3284c02471b2', N'ciccio@pasticcio.it', NULL, 1, NULL, NULL, N'd1da25b6-4644-47f8-aed2-782555f72202', CAST(N'2019-08-06T17:07:37.017' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2019-08-06T17:07:37.017' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'fadabae8-bd54-452b-ac2b-9bf633928353', N'usr00007', N'DHs7+agIneBB+b/+1elKlQ==', N'usr00007', N'usr00007', N'3d7906c6-db11-4410-987c-7770549ccb63', N'rgiardi@fomindustrie.com', NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'6b15910a-f63c-4555-8964-9f51e3057b0e', N'Assistance', N'FomMonitoring', N'Assis', N'tance', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'e47c7be8-7492-4a75-8bee-af2ae8d7d56b', N'mbelletti', N'ohtCn8u+rT7GgWypfSCz1w==', N'Matteo', N'Belletti', N'92440952-f542-431a-82e8-3284c02471b2', N'mbelletti@fomsoftware.com', NULL, 1, NULL, NULL, N'e47c7be8-7492-4a75-8bee-af2ae8d7d56b', CAST(N'2019-08-02T08:54:40.977' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2019-08-02T08:54:40.977' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'2a564d2a-ffa5-4dc9-a77c-af2d63ec3d0e', N'lola', N'u/+9Y2zYgtjvFfRWGFgeFg==', N'Mark', N'Kofpler', N'3d7906c6-db11-4410-987c-7770549ccb63', N'prova@mail.com', NULL, 1, NULL, NULL, N'2a564d2a-ffa5-4dc9-a77c-af2d63ec3d0e', CAST(N'2020-05-04T10:21:50.097' AS DateTime), NULL, NULL, NULL, NULL, CAST(N'2020-05-04T10:21:50.097' AS DateTime))
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'621051e4-59be-4db8-847e-b4e306500721', N'userEN', N'p5Cy+RxrgiwOYYBaqFTMOg==', N'user', N'EN', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'758b1941-1229-433e-a5dd-bff3b27c2fb3', N'userIT', N'p5Cy+RxrgiwOYYBaqFTMOg==', N'user', N'IT', N'92440952-f542-431a-82e8-3284c02471b2', NULL, NULL, 1, NULL, NULL, N'758b1941-1229-433e-a5dd-bff3b27c2fb3', CAST(N'2020-06-30T11:11:39.480' AS DateTime), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'56924e54-b9b3-4d95-9567-dace90215e1c', N'test', N'u/+9Y2zYgtjcN2EWnO/Reg==', N'ciao', N'ciao', N'dac66418-9148-49d8-8d45-08609051ddbf', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'41f6373c-4bbd-4d21-bc81-e097f6088cb3', N'userES', N'p5Cy+RxrgiwOYYBaqFTMOg==', N'user', N'ES', N'f6d59604-aec0-49c6-bd08-43a39fd7d7ce', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'1aea6451-7934-4874-89c8-f787cf3f6aa1', N'UserApi', N'Y2orBaPXAyi4NrsysQhr8tQhw1M2qZsrm1xxRqqYeOs=', N'User', N'Api', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([ID], [Username], [Password], [FirstName], [LastName], [LanguageID], [Email], [DefaultHomePage], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [Domain], [LastDateUpdatePassword]) VALUES (N'e3f96061-8e80-42a8-b55e-fdb7fd7a59b9', N'NewUsrEN', N'u/+9Y2zYgtjcN2EWnO/Reg==', N'R', N'G', N'3d7906c6-db11-4410-987c-7770549ccb63', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'517a230d-5104-4711-bdc0-0a587cb0f3ea', 3, N'Assistance', N'Assistance', 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'e64e99fc-12ba-41ed-9fc5-59af8d5e8723', 4, N'Customer', N'Customer', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'c95302b3-1bb9-4563-8459-7c977c0469b0', 5, N'UserApi', N'UserApi', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'6be861f2-fc68-4a1e-964f-914474d17484', 1, N'Operator', N'Operator', 1, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', 2, N'HeadWorkshop', N'HeadWorkshop', 1, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL)
GO
INSERT [dbo].[Roles] ([ID], [IdRole], [Name], [Description], [Enabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate], [Status], [HomePage]) VALUES (N'e2f098ba-012f-43af-abd5-fb003ed3cea4', 0, N'Administrator', N'Administrator', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'ed1b7c0f-9b09-44dc-9f4a-041c5878c2a3', N'e64e99fc-12ba-41ed-9fc5-59af8d5e8723', N'fadabae8-bd54-452b-ac2b-9bf633928353')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'71a9b721-64c7-4d5a-93ec-0ebbf1e3af08', N'6be861f2-fc68-4a1e-964f-914474d17484', N'9bd41e60-461b-4117-9bc3-47d84f8f2ab6')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'50c5fb02-4187-4dba-8ef8-0fa4719ddab8', N'e2f098ba-012f-43af-abd5-fb003ed3cea4', N'7d1b1b7c-b86d-44a2-a3ff-168127a5d579')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'b3cfe95e-dab9-4f49-8be6-17d4243524c9', N'c95302b3-1bb9-4563-8459-7c977c0469b0', N'1aea6451-7934-4874-89c8-f787cf3f6aa1')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'4e03e3a3-8862-444a-9d68-26cf2e4115d5', N'6be861f2-fc68-4a1e-964f-914474d17484', N'7adf67c3-a2f8-498c-8b12-3ce1e123ef44')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'475f2bf7-24fa-4d1d-a9cb-318b34cb4fbb', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'd1646002-b53d-4b2c-9310-775355cf0901')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'a0064720-22ed-4386-835c-32eebd96a2a6', N'e64e99fc-12ba-41ed-9fc5-59af8d5e8723', N'd478eb36-03de-4b10-add1-6c3a9c882086')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'9c34aaf2-0ad1-4b4e-966f-5119ae2b9a86', N'6be861f2-fc68-4a1e-964f-914474d17484', N'd1da25b6-4644-47f8-aed2-782555f72202')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'cc873659-f980-4afe-b3c2-5888f7f7c789', N'6be861f2-fc68-4a1e-964f-914474d17484', N'0f428ffc-47ed-42dd-8dba-257460f765ee')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'3a5cd337-4ec6-4fac-8060-5b6199b3a154', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'e47c7be8-7492-4a75-8bee-af2ae8d7d56b')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'b4ee9a5e-8f0f-4421-9d5f-72608c00d806', N'6be861f2-fc68-4a1e-964f-914474d17484', N'e3f96061-8e80-42a8-b55e-fdb7fd7a59b9')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'8a889080-185e-4762-a142-7bc7da872e99', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'2a564d2a-ffa5-4dc9-a77c-af2d63ec3d0e')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'5db43f88-aed7-442f-b36f-8245e64f9fb0', N'6be861f2-fc68-4a1e-964f-914474d17484', N'ec101b5b-b043-48aa-ba65-1af3a924c27c')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'b2b21f31-4d0f-451f-b674-8840b71e7251', N'e64e99fc-12ba-41ed-9fc5-59af8d5e8723', N'32fd3016-e4de-4331-b044-2cd25a032d55')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'bbb9cd54-d0b4-4bc0-8d4c-8c2a3843f030', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'758b1941-1229-433e-a5dd-bff3b27c2fb3')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'3291c77d-a406-4924-a5f4-923da3dffa97', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'1b9cb7b6-702e-4fd1-949d-479767006326')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'f8c9a0f1-4ab6-4716-a214-a36d17075c8d', N'6be861f2-fc68-4a1e-964f-914474d17484', N'41f6373c-4bbd-4d21-bc81-e097f6088cb3')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'ccf033c5-4c0a-4e14-aa80-b0bf85a2d1a1', N'6be861f2-fc68-4a1e-964f-914474d17484', N'56924e54-b9b3-4d95-9567-dace90215e1c')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'a6670490-76fb-4beb-90ca-bb7d4ce5d9e4', N'5e9e8fc8-0609-4b4d-975e-bef4bb2a404c', N'2716582b-9689-495a-b669-0bc67279fabd')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'd3ae06d7-ce4f-4558-8c57-bf1ea8bbd87c', N'6be861f2-fc68-4a1e-964f-914474d17484', N'982d2e9b-b77b-4477-a12b-609cd1d50339')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'0cdea908-24d5-480c-8307-c60809187fdc', N'517a230d-5104-4711-bdc0-0a587cb0f3ea', N'6b15910a-f63c-4555-8964-9f51e3057b0e')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'1aada8ad-de65-41a9-ad33-ca8dac0122f5', N'e64e99fc-12ba-41ed-9fc5-59af8d5e8723', N'c77f88c7-1130-40b9-970e-3909aad4c0df')
GO
INSERT [dbo].[Roles_Users] ([ID], [RoleID], [UserID]) VALUES (N'789c7e25-3e38-4303-8bde-fafb0752ebee', N'6be861f2-fc68-4a1e-964f-914474d17484', N'621051e4-59be-4db8-847e-b4e306500721')
GO
