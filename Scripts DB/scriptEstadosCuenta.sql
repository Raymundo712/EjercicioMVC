USE [CajaVirtual]
GO
/****** Object:  Table [dbo].[EstadosCuenta]    Script Date: 05/07/2023 04:57:25 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadosCuenta](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Saldo] [money] NOT NULL,
	[Cliente_Id] [int] NOT NULL,
 CONSTRAINT [PK_EstadosCuenta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EstadosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_EstadosCuenta_Clientes_Cliente_Id] FOREIGN KEY([Cliente_Id])
REFERENCES [dbo].[Clientes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EstadosCuenta] CHECK CONSTRAINT [FK_EstadosCuenta_Clientes_Cliente_Id]
GO
