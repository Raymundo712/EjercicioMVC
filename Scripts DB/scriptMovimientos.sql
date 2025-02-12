USE [CajaVirtual]
GO
/****** Object:  Table [dbo].[Movimientos]    Script Date: 05/07/2023 04:58:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Movimientos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EstadoCuenta_Id] [int] NOT NULL,
	[FechaMovimiento] [datetime2](7) NOT NULL,
	[ConceptoCobro_Id] [int] NULL,
	[Abono] [money] NOT NULL,
	[EstadoCuentaId] [int] NULL,
 CONSTRAINT [PK_Movimientos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Movimientos]  WITH CHECK ADD  CONSTRAINT [FK_Movimientos_Conceptos_ConceptoCobro_Id] FOREIGN KEY([ConceptoCobro_Id])
REFERENCES [dbo].[Conceptos] ([Id])
GO
ALTER TABLE [dbo].[Movimientos] CHECK CONSTRAINT [FK_Movimientos_Conceptos_ConceptoCobro_Id]
GO
ALTER TABLE [dbo].[Movimientos]  WITH CHECK ADD  CONSTRAINT [FK_Movimientos_EstadosCuenta_EstadoCuenta_Id] FOREIGN KEY([EstadoCuenta_Id])
REFERENCES [dbo].[EstadosCuenta] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Movimientos] CHECK CONSTRAINT [FK_Movimientos_EstadosCuenta_EstadoCuenta_Id]
GO
ALTER TABLE [dbo].[Movimientos]  WITH CHECK ADD  CONSTRAINT [FK_Movimientos_EstadosCuenta_EstadoCuentaId] FOREIGN KEY([EstadoCuentaId])
REFERENCES [dbo].[EstadosCuenta] ([Id])
GO
ALTER TABLE [dbo].[Movimientos] CHECK CONSTRAINT [FK_Movimientos_EstadosCuenta_EstadoCuentaId]
GO
