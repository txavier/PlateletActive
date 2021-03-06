USE [master]
GO
/****** Object:  Database [CsvParser]    Script Date: 10/19/2015 3:09:50 AM ******/
CREATE DATABASE [CsvParser]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CsvParser', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\CsvParser.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CsvParser_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\CsvParser_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CsvParser] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CsvParser].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CsvParser] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CsvParser] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CsvParser] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CsvParser] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CsvParser] SET ARITHABORT OFF 
GO
ALTER DATABASE [CsvParser] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CsvParser] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CsvParser] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CsvParser] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CsvParser] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CsvParser] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CsvParser] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CsvParser] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CsvParser] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CsvParser] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CsvParser] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CsvParser] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CsvParser] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CsvParser] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CsvParser] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CsvParser] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CsvParser] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CsvParser] SET RECOVERY FULL 
GO
ALTER DATABASE [CsvParser] SET  MULTI_USER 
GO
ALTER DATABASE [CsvParser] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CsvParser] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CsvParser] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CsvParser] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [CsvParser] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'CsvParser', N'ON'
GO
USE [CsvParser]
GO
/****** Object:  User [luke]    Script Date: 10/19/2015 3:09:50 AM ******/
CREATE USER [luke] FOR LOGIN [luke] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [luke]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [luke]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 10/19/2015 3:09:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client]    Script Date: 10/19/2015 3:09:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[clientId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_client] PRIMARY KEY CLUSTERED 
(
	[clientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HplcData]    Script Date: 10/19/2015 3:09:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HplcData](
	[HplcDataId] [int] IDENTITY(1,1) NOT NULL,
	[clientId] [int] NULL,
	[BatchId] [int] NULL,
	[SampleAge] [varchar](max) NULL,
	[Timestamp] [datetime2](7) NULL,
	[SampleName] [varchar](max) NULL,
	[Dp4] [float] NULL,
	[Dp3] [float] NULL,
	[Dp2Maltose] [float] NULL,
	[Dp1Glucose] [float] NULL,
	[LacticAcid] [float] NULL,
	[Glycerol] [float] NULL,
	[AceticAcid] [float] NULL,
	[Ethanol] [float] NULL,
	[User] [nvarchar](max) NULL,
	[SampleLocation] [varchar](max) NULL,
 CONSTRAINT [PK_HplcData] PRIMARY KEY CLUSTERED 
(
	[HplcDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HplcData]  WITH CHECK ADD  CONSTRAINT [FK_HplcData_client] FOREIGN KEY([clientId])
REFERENCES [dbo].[Client] ([clientId])
GO
ALTER TABLE [dbo].[HplcData] CHECK CONSTRAINT [FK_HplcData_client]
GO
USE [master]
GO
ALTER DATABASE [CsvParser] SET  READ_WRITE 
GO
