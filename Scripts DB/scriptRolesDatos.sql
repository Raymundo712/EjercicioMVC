USE [CajaVirtual]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [Nombre], [UsuarioId]) VALUES (1, N'Cliente', NULL)
INSERT [dbo].[Roles] ([Id], [Nombre], [UsuarioId]) VALUES (2, N'Cajero', NULL)
INSERT [dbo].[Roles] ([Id], [Nombre], [UsuarioId]) VALUES (3, N'Administrador', NULL)
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
