USE [master]
GO
/****** Object:  Database [TAO]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE DATABASE [TAO]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TAO', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQL\DATA\TAO.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TAO_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQL\DATA\TAO_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TAO] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TAO].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TAO] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TAO] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TAO] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TAO] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TAO] SET ARITHABORT OFF 
GO
ALTER DATABASE [TAO] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TAO] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TAO] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TAO] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TAO] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TAO] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TAO] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TAO] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TAO] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TAO] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TAO] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TAO] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TAO] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TAO] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TAO] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TAO] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [TAO] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TAO] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TAO] SET  MULTI_USER 
GO
ALTER DATABASE [TAO] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TAO] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TAO] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TAO] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TAO] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TAO] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [TAO] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TAO] SET QUERY_STORE = ON
GO
ALTER DATABASE [TAO] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TAO]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentCompetencyEvaluations]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentCompetencyEvaluations](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentResultId] [uniqueidentifier] NOT NULL,
	[CompetencyName] [nvarchar](200) NOT NULL,
	[Priority] [nvarchar](50) NOT NULL,
	[Score] [tinyint] NOT NULL,
	[MinimumPassPercentage] [tinyint] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentCompetencyEvaluations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentQuestionEvaluations]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentQuestionEvaluations](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentQuestionId] [uniqueidentifier] NOT NULL,
	[Score] [tinyint] NOT NULL,
	[Confidence] [tinyint] NOT NULL,
	[Strengths] [nvarchar](max) NOT NULL,
	[Gaps] [nvarchar](max) NOT NULL,
	[Evidence] [nvarchar](max) NOT NULL,
	[Competencies] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentQuestionEvaluations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentQuestions]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentQuestions](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentSessionRoundId] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[PrimaryQuestion] [nvarchar](4000) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Conversation] [nvarchar](max) NULL,
	[CandidateCode] [nvarchar](max) NULL,
	[StartedOn] [datetime2](7) NULL,
	[CompletedOn] [datetime2](7) NULL,
	[Competencies] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentQuestions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentResults]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentResults](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentSessionId] [uniqueidentifier] NOT NULL,
	[OverallScore] [tinyint] NOT NULL,
	[OverallConfidence] [tinyint] NOT NULL,
	[Recommendation] [tinyint] NOT NULL,
	[ExecutiveSummary] [nvarchar](max) NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
	[ReviewedByUserId] [uniqueidentifier] NULL,
	[ReviewedOn] [datetime2](7) NULL,
	[RecruiterDecision] [nvarchar](100) NULL,
	[RecruiterComments] [nvarchar](max) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_AssessmentResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentRoundEvaluations]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentRoundEvaluations](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentSessionRoundId] [uniqueidentifier] NOT NULL,
	[Score] [tinyint] NOT NULL,
	[Confidence] [tinyint] NOT NULL,
	[Strengths] [nvarchar](max) NOT NULL,
	[Gaps] [nvarchar](max) NOT NULL,
	[Evidence] [nvarchar](max) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentRoundEvaluations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentRounds]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentRounds](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentStrategyId] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Difficulty] [tinyint] NOT NULL,
	[DurationInMinutes] [int] NOT NULL,
	[TargetQuestionCount] [int] NOT NULL,
	[Competencies] [nvarchar](max) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentRounds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentSessionRounds]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentSessionRounds](
	[Id] [uniqueidentifier] NOT NULL,
	[AssessmentSessionId] [uniqueidentifier] NOT NULL,
	[AssessmentRoundId] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Difficulty] [tinyint] NOT NULL,
	[DurationInMinutes] [int] NOT NULL,
	[TargetQuestionCount] [int] NOT NULL,
	[Competencies] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[StartedOn] [datetime2](7) NULL,
	[ExpiresOn] [datetime2](7) NULL,
	[CompletedOn] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentSessionRounds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentSessions]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentSessions](
	[Id] [uniqueidentifier] NOT NULL,
	[CandidateApplicationId] [uniqueidentifier] NOT NULL,
	[AssessmentStrategyId] [uniqueidentifier] NOT NULL,
	[StrategySnapshot] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CurrentSessionRoundId] [uniqueidentifier] NULL,
	[CurrentQuestionId] [uniqueidentifier] NULL,
	[ConsentAcceptedOn] [datetime2](7) NULL,
	[ConsentVersion] [int] NOT NULL,
	[StartedOn] [datetime2](7) NULL,
	[CompletedOn] [datetime2](7) NULL,
	[AssessmentExpiresOn] [datetime2](7) NOT NULL,
	[LastActivityOn] [datetime2](7) NOT NULL,
	[HasUsedInterruptionWindow] [bit] NOT NULL,
	[IsInterrupted] [bit] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_AssessmentSessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentStrategies]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentStrategies](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[AssessmentName] [nvarchar](200) NOT NULL,
	[GeneratedContent] [nvarchar](max) NOT NULL,
	[StructuredData] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
	[ApprovedByUserId] [uniqueidentifier] NULL,
	[ApprovedOn] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[Prompt] [nvarchar](max) NOT NULL,
	[RawResponse] [nvarchar](max) NOT NULL,
	[ProviderName] [nvarchar](100) NOT NULL,
	[ModelName] [nvarchar](100) NOT NULL,
	[PromptVersion] [int] NOT NULL,
 CONSTRAINT [PK_AssessmentStrategies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Campaigns]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaigns](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ReferenceNumber] [nvarchar](50) NOT NULL,
	[RecruiterId] [uniqueidentifier] NOT NULL,
	[HiringManagerId] [uniqueidentifier] NOT NULL,
	[NumberOfOpenings] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_Campaigns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CandidateApplications]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidateApplications](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[CandidateName] [nvarchar](200) NOT NULL,
	[Email] [nvarchar](320) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[LinkedInUrl] [nvarchar](500) NULL,
	[CurrentCompany] [nvarchar](200) NULL,
	[CurrentLocation] [nvarchar](200) NULL,
	[OverallMatchPercentage] [tinyint] NOT NULL,
	[IsRecommended] [bit] NOT NULL,
	[ResumeUploadedOn] [datetime2](7) NOT NULL,
	[LastScreenedOn] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_CandidateApplications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailDeliveries]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailDeliveries](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[CandidateApplicationId] [uniqueidentifier] NOT NULL,
	[RecipientEmail] [nvarchar](320) NOT NULL,
	[Subject] [nvarchar](500) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[SentOn] [datetime2](7) NULL,
	[FailedOn] [datetime2](7) NULL,
	[FailureReason] [nvarchar](2000) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_EmailDeliveries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HiringStrategies]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiringStrategies](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[GeneratedContent] [nvarchar](max) NOT NULL,
	[StructuredData] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[ApprovedByUserId] [uniqueidentifier] NULL,
	[ApprovedOn] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[Prompt] [nvarchar](max) NOT NULL,
	[RawResponse] [nvarchar](max) NOT NULL,
	[ProviderName] [nvarchar](100) NOT NULL,
	[ModelName] [nvarchar](100) NOT NULL,
	[PromptVersion] [int] NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_HiringStrategies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobProfiles]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobProfiles](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[OriginalJobDescription] [nvarchar](max) NOT NULL,
	[Prompt] [nvarchar](max) NOT NULL,
	[RawResponse] [nvarchar](max) NOT NULL,
	[ProviderName] [nvarchar](100) NOT NULL,
	[ModelName] [nvarchar](100) NOT NULL,
	[PromptVersion] [int] NOT NULL,
	[GeneratedContent] [nvarchar](max) NOT NULL,
	[StructuredProfile] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
	[ApprovedByUserId] [uniqueidentifier] NULL,
	[ApprovedOn] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_JobProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organizations](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeImportFailures]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumeImportFailures](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[ResumeImportId] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FailureReason] [nvarchar](500) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_ResumeImportFailures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeImports]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumeImports](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[CampaignId] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[TotalFiles] [int] NOT NULL,
	[SuccessfulFiles] [int] NOT NULL,
	[FailedFiles] [int] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_ResumeImports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeProfiles]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumeProfiles](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[StructuredContent] [nvarchar](max) NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_ResumeProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resumes]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resumes](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[ContentType] [nvarchar](100) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[FileContent] [varbinary](max) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_Resumes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeScreenings]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumeScreenings](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[StructuredContent] [nvarchar](max) NOT NULL,
	[GeneratedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_ResumeScreenings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 9/10/2026 8:01:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Role] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_AssessmentCompetencyEvaluations_Result_Competency]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentCompetencyEvaluations_Result_Competency] ON [dbo].[AssessmentCompetencyEvaluations]
(
	[AssessmentResultId] ASC,
	[CompetencyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_AssessmentQuestionEvaluations_QuestionId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentQuestionEvaluations_QuestionId] ON [dbo].[AssessmentQuestionEvaluations]
(
	[AssessmentQuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_AssessmentQuestions_SessionRound_Order]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentQuestions_SessionRound_Order] ON [dbo].[AssessmentQuestions]
(
	[AssessmentSessionRoundId] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentResults_ReviewedByUserId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentResults_ReviewedByUserId] ON [dbo].[AssessmentResults]
(
	[ReviewedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_AssessmentResults_AssessmentSessionId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentResults_AssessmentSessionId] ON [dbo].[AssessmentResults]
(
	[AssessmentSessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_AssessmentRoundEvaluations_SessionRoundId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentRoundEvaluations_SessionRoundId] ON [dbo].[AssessmentRoundEvaluations]
(
	[AssessmentSessionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentRounds_AssessmentStrategyId_Order]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AssessmentRounds_AssessmentStrategyId_Order] ON [dbo].[AssessmentRounds]
(
	[AssessmentStrategyId] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentSessionRounds_AssessmentRoundId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentSessionRounds_AssessmentRoundId] ON [dbo].[AssessmentSessionRounds]
(
	[AssessmentRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_AssessmentSessionRounds_Session_Order]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_AssessmentSessionRounds_Session_Order] ON [dbo].[AssessmentSessionRounds]
(
	[AssessmentSessionId] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentSessions_AssessmentStrategyId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentSessions_AssessmentStrategyId] ON [dbo].[AssessmentSessions]
(
	[AssessmentStrategyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentSessions_CandidateApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentSessions_CandidateApplicationId] ON [dbo].[AssessmentSessions]
(
	[CandidateApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentSessions_Status_Expires]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentSessions_Status_Expires] ON [dbo].[AssessmentSessions]
(
	[Status] ASC,
	[AssessmentExpiresOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentStrategies_ApprovedByUserId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentStrategies_ApprovedByUserId] ON [dbo].[AssessmentStrategies]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentStrategies_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AssessmentStrategies_CampaignId] ON [dbo].[AssessmentStrategies]
(
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssessmentStrategies_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_AssessmentStrategies_OrganizationId] ON [dbo].[AssessmentStrategies]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Campaigns_HiringManagerId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Campaigns_HiringManagerId] ON [dbo].[Campaigns]
(
	[HiringManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Campaigns_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Campaigns_OrganizationId] ON [dbo].[Campaigns]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Campaigns_OrganizationId_ReferenceNumber]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Campaigns_OrganizationId_ReferenceNumber] ON [dbo].[Campaigns]
(
	[OrganizationId] ASC,
	[ReferenceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Campaigns_OrganizationId_Status]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Campaigns_OrganizationId_Status] ON [dbo].[Campaigns]
(
	[OrganizationId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Campaigns_RecruiterId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Campaigns_RecruiterId] ON [dbo].[Campaigns]
(
	[RecruiterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Campaigns_Status]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Campaigns_Status] ON [dbo].[Campaigns]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CandidateApplications_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_CampaignId] ON [dbo].[CandidateApplications]
(
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CandidateApplications_Email]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_Email] ON [dbo].[CandidateApplications]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CandidateApplications_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_OrganizationId] ON [dbo].[CandidateApplications]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CandidateApplications_OrganizationId_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_OrganizationId_CampaignId] ON [dbo].[CandidateApplications]
(
	[OrganizationId] ASC,
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CandidateApplications_OrganizationId_CampaignId_IsRecommended]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_OrganizationId_CampaignId_IsRecommended] ON [dbo].[CandidateApplications]
(
	[OrganizationId] ASC,
	[CampaignId] ASC,
	[IsRecommended] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CandidateApplications_OrganizationId_CampaignId_OverallMatchPercentage]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_CandidateApplications_OrganizationId_CampaignId_OverallMatchPercentage] ON [dbo].[CandidateApplications]
(
	[OrganizationId] ASC,
	[CampaignId] ASC,
	[OverallMatchPercentage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmailDeliveries_CampaignId_CandidateApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmailDeliveries_CampaignId_CandidateApplicationId] ON [dbo].[EmailDeliveries]
(
	[CampaignId] ASC,
	[CandidateApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmailDeliveries_RecipientEmail]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_EmailDeliveries_RecipientEmail] ON [dbo].[EmailDeliveries]
(
	[RecipientEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HiringStrategies_ApprovedByUserId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_HiringStrategies_ApprovedByUserId] ON [dbo].[HiringStrategies]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HiringStrategies_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_HiringStrategies_CampaignId] ON [dbo].[HiringStrategies]
(
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HiringStrategies_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_HiringStrategies_OrganizationId] ON [dbo].[HiringStrategies]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobProfiles_ApprovedByUserId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_JobProfiles_ApprovedByUserId] ON [dbo].[JobProfiles]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobProfiles_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_JobProfiles_CampaignId] ON [dbo].[JobProfiles]
(
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobProfiles_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_JobProfiles_OrganizationId] ON [dbo].[JobProfiles]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Organizations_Code]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Organizations_Code] ON [dbo].[Organizations]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Organizations_Name]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Organizations_Name] ON [dbo].[Organizations]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Organizations_Status]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Organizations_Status] ON [dbo].[Organizations]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeImportFailures_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeImportFailures_OrganizationId] ON [dbo].[ResumeImportFailures]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeImportFailures_ResumeImportId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeImportFailures_ResumeImportId] ON [dbo].[ResumeImportFailures]
(
	[ResumeImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeImports_CampaignId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeImports_CampaignId] ON [dbo].[ResumeImports]
(
	[CampaignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeImports_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeImports_OrganizationId] ON [dbo].[ResumeImports]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeProfiles_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeProfiles_ApplicationId] ON [dbo].[ResumeProfiles]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeProfiles_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeProfiles_OrganizationId] ON [dbo].[ResumeProfiles]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeProfiles_OrganizationId_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeProfiles_OrganizationId_ApplicationId] ON [dbo].[ResumeProfiles]
(
	[OrganizationId] ASC,
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Resumes_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Resumes_ApplicationId] ON [dbo].[Resumes]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Resumes_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Resumes_OrganizationId] ON [dbo].[Resumes]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Resumes_OrganizationId_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Resumes_OrganizationId_ApplicationId] ON [dbo].[Resumes]
(
	[OrganizationId] ASC,
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeScreenings_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeScreenings_ApplicationId] ON [dbo].[ResumeScreenings]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeScreenings_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeScreenings_OrganizationId] ON [dbo].[ResumeScreenings]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ResumeScreenings_OrganizationId_ApplicationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_ResumeScreenings_OrganizationId_ApplicationId] ON [dbo].[ResumeScreenings]
(
	[OrganizationId] ASC,
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_OrganizationId]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_Users_OrganizationId] ON [dbo].[Users]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_OrganizationId_Email]    Script Date: 9/10/2026 8:01:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_OrganizationId_Email] ON [dbo].[Users]
(
	[OrganizationId] ASC,
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssessmentQuestions] ADD  CONSTRAINT [DF_AssessmentQuestions_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AssessmentRoundEvaluations] ADD  CONSTRAINT [DF_AssessmentRoundEvaluations_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AssessmentSessionRounds] ADD  CONSTRAINT [DF_AssessmentSessionRounds_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AssessmentSessions] ADD  CONSTRAINT [DF_AssessmentSessions_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AssessmentCompetencyEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentCompetencyEvaluations_AssessmentResults] FOREIGN KEY([AssessmentResultId])
REFERENCES [dbo].[AssessmentResults] ([Id])
GO
ALTER TABLE [dbo].[AssessmentCompetencyEvaluations] CHECK CONSTRAINT [FK_AssessmentCompetencyEvaluations_AssessmentResults]
GO
ALTER TABLE [dbo].[AssessmentQuestionEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestionEvaluations_AssessmentQuestions] FOREIGN KEY([AssessmentQuestionId])
REFERENCES [dbo].[AssessmentQuestions] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestionEvaluations] CHECK CONSTRAINT [FK_AssessmentQuestionEvaluations_AssessmentQuestions]
GO
ALTER TABLE [dbo].[AssessmentQuestions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestions_AssessmentSessionRounds] FOREIGN KEY([AssessmentSessionRoundId])
REFERENCES [dbo].[AssessmentSessionRounds] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestions] CHECK CONSTRAINT [FK_AssessmentQuestions_AssessmentSessionRounds]
GO
ALTER TABLE [dbo].[AssessmentResults]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResults_AssessmentSessions] FOREIGN KEY([AssessmentSessionId])
REFERENCES [dbo].[AssessmentSessions] ([Id])
GO
ALTER TABLE [dbo].[AssessmentResults] CHECK CONSTRAINT [FK_AssessmentResults_AssessmentSessions]
GO
ALTER TABLE [dbo].[AssessmentResults]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResults_ReviewedByUser] FOREIGN KEY([ReviewedByUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[AssessmentResults] CHECK CONSTRAINT [FK_AssessmentResults_ReviewedByUser]
GO
ALTER TABLE [dbo].[AssessmentRoundEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentRoundEvaluations_AssessmentSessionRounds] FOREIGN KEY([AssessmentSessionRoundId])
REFERENCES [dbo].[AssessmentSessionRounds] ([Id])
GO
ALTER TABLE [dbo].[AssessmentRoundEvaluations] CHECK CONSTRAINT [FK_AssessmentRoundEvaluations_AssessmentSessionRounds]
GO
ALTER TABLE [dbo].[AssessmentRounds]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentRounds_AssessmentStrategies_AssessmentStrategyId] FOREIGN KEY([AssessmentStrategyId])
REFERENCES [dbo].[AssessmentStrategies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AssessmentRounds] CHECK CONSTRAINT [FK_AssessmentRounds_AssessmentStrategies_AssessmentStrategyId]
GO
ALTER TABLE [dbo].[AssessmentSessionRounds]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessionRounds_AssessmentRounds] FOREIGN KEY([AssessmentRoundId])
REFERENCES [dbo].[AssessmentRounds] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessionRounds] CHECK CONSTRAINT [FK_AssessmentSessionRounds_AssessmentRounds]
GO
ALTER TABLE [dbo].[AssessmentSessionRounds]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessionRounds_AssessmentSessions] FOREIGN KEY([AssessmentSessionId])
REFERENCES [dbo].[AssessmentSessions] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessionRounds] CHECK CONSTRAINT [FK_AssessmentSessionRounds_AssessmentSessions]
GO
ALTER TABLE [dbo].[AssessmentSessions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessions_AssessmentStrategies] FOREIGN KEY([AssessmentStrategyId])
REFERENCES [dbo].[AssessmentStrategies] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessions] CHECK CONSTRAINT [FK_AssessmentSessions_AssessmentStrategies]
GO
ALTER TABLE [dbo].[AssessmentSessions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessions_CandidateApplications] FOREIGN KEY([CandidateApplicationId])
REFERENCES [dbo].[CandidateApplications] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessions] CHECK CONSTRAINT [FK_AssessmentSessions_CandidateApplications]
GO
ALTER TABLE [dbo].[AssessmentSessions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessions_CurrentQuestion] FOREIGN KEY([CurrentQuestionId])
REFERENCES [dbo].[AssessmentQuestions] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessions] CHECK CONSTRAINT [FK_AssessmentSessions_CurrentQuestion]
GO
ALTER TABLE [dbo].[AssessmentSessions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSessions_CurrentSessionRound] FOREIGN KEY([CurrentSessionRoundId])
REFERENCES [dbo].[AssessmentSessionRounds] ([Id])
GO
ALTER TABLE [dbo].[AssessmentSessions] CHECK CONSTRAINT [FK_AssessmentSessions_CurrentSessionRound]
GO
ALTER TABLE [dbo].[AssessmentStrategies]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentStrategies_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[AssessmentStrategies] CHECK CONSTRAINT [FK_AssessmentStrategies_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[AssessmentStrategies]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentStrategies_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[AssessmentStrategies] CHECK CONSTRAINT [FK_AssessmentStrategies_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[AssessmentStrategies]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentStrategies_Users_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[AssessmentStrategies] CHECK CONSTRAINT [FK_AssessmentStrategies_Users_ApprovedByUserId]
GO
ALTER TABLE [dbo].[Campaigns]  WITH CHECK ADD  CONSTRAINT [FK_Campaigns_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Campaigns] CHECK CONSTRAINT [FK_Campaigns_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[Campaigns]  WITH CHECK ADD  CONSTRAINT [FK_Campaigns_Users_HiringManagerId] FOREIGN KEY([HiringManagerId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Campaigns] CHECK CONSTRAINT [FK_Campaigns_Users_HiringManagerId]
GO
ALTER TABLE [dbo].[Campaigns]  WITH CHECK ADD  CONSTRAINT [FK_Campaigns_Users_RecruiterId] FOREIGN KEY([RecruiterId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Campaigns] CHECK CONSTRAINT [FK_Campaigns_Users_RecruiterId]
GO
ALTER TABLE [dbo].[CandidateApplications]  WITH CHECK ADD  CONSTRAINT [FK_CandidateApplications_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[CandidateApplications] CHECK CONSTRAINT [FK_CandidateApplications_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[CandidateApplications]  WITH CHECK ADD  CONSTRAINT [FK_CandidateApplications_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[CandidateApplications] CHECK CONSTRAINT [FK_CandidateApplications_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[EmailDeliveries]  WITH CHECK ADD  CONSTRAINT [FK_EmailDeliveries_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[EmailDeliveries] CHECK CONSTRAINT [FK_EmailDeliveries_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[EmailDeliveries]  WITH CHECK ADD  CONSTRAINT [FK_EmailDeliveries_CandidateApplications_CandidateApplicationId] FOREIGN KEY([CandidateApplicationId])
REFERENCES [dbo].[CandidateApplications] ([Id])
GO
ALTER TABLE [dbo].[EmailDeliveries] CHECK CONSTRAINT [FK_EmailDeliveries_CandidateApplications_CandidateApplicationId]
GO
ALTER TABLE [dbo].[EmailDeliveries]  WITH CHECK ADD  CONSTRAINT [FK_EmailDeliveries_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[EmailDeliveries] CHECK CONSTRAINT [FK_EmailDeliveries_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[HiringStrategies]  WITH CHECK ADD  CONSTRAINT [FK_HiringStrategies_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[HiringStrategies] CHECK CONSTRAINT [FK_HiringStrategies_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[HiringStrategies]  WITH CHECK ADD  CONSTRAINT [FK_HiringStrategies_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[HiringStrategies] CHECK CONSTRAINT [FK_HiringStrategies_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[HiringStrategies]  WITH CHECK ADD  CONSTRAINT [FK_HiringStrategies_Users_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[HiringStrategies] CHECK CONSTRAINT [FK_HiringStrategies_Users_ApprovedByUserId]
GO
ALTER TABLE [dbo].[JobProfiles]  WITH CHECK ADD  CONSTRAINT [FK_JobProfiles_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[JobProfiles] CHECK CONSTRAINT [FK_JobProfiles_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[JobProfiles]  WITH CHECK ADD  CONSTRAINT [FK_JobProfiles_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[JobProfiles] CHECK CONSTRAINT [FK_JobProfiles_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[JobProfiles]  WITH CHECK ADD  CONSTRAINT [FK_JobProfiles_Users_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[JobProfiles] CHECK CONSTRAINT [FK_JobProfiles_Users_ApprovedByUserId]
GO
ALTER TABLE [dbo].[ResumeImportFailures]  WITH CHECK ADD  CONSTRAINT [FK_ResumeImportFailures_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[ResumeImportFailures] CHECK CONSTRAINT [FK_ResumeImportFailures_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[ResumeImportFailures]  WITH CHECK ADD  CONSTRAINT [FK_ResumeImportFailures_ResumeImports_ResumeImportId] FOREIGN KEY([ResumeImportId])
REFERENCES [dbo].[ResumeImports] ([Id])
GO
ALTER TABLE [dbo].[ResumeImportFailures] CHECK CONSTRAINT [FK_ResumeImportFailures_ResumeImports_ResumeImportId]
GO
ALTER TABLE [dbo].[ResumeImports]  WITH CHECK ADD  CONSTRAINT [FK_ResumeImports_Campaigns_CampaignId] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[ResumeImports] CHECK CONSTRAINT [FK_ResumeImports_Campaigns_CampaignId]
GO
ALTER TABLE [dbo].[ResumeImports]  WITH CHECK ADD  CONSTRAINT [FK_ResumeImports_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[ResumeImports] CHECK CONSTRAINT [FK_ResumeImports_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[ResumeProfiles]  WITH CHECK ADD  CONSTRAINT [FK_ResumeProfiles_CandidateApplications_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[CandidateApplications] ([Id])
GO
ALTER TABLE [dbo].[ResumeProfiles] CHECK CONSTRAINT [FK_ResumeProfiles_CandidateApplications_ApplicationId]
GO
ALTER TABLE [dbo].[ResumeProfiles]  WITH CHECK ADD  CONSTRAINT [FK_ResumeProfiles_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[ResumeProfiles] CHECK CONSTRAINT [FK_ResumeProfiles_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[Resumes]  WITH CHECK ADD  CONSTRAINT [FK_Resumes_CandidateApplications_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[CandidateApplications] ([Id])
GO
ALTER TABLE [dbo].[Resumes] CHECK CONSTRAINT [FK_Resumes_CandidateApplications_ApplicationId]
GO
ALTER TABLE [dbo].[Resumes]  WITH CHECK ADD  CONSTRAINT [FK_Resumes_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Resumes] CHECK CONSTRAINT [FK_Resumes_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[ResumeScreenings]  WITH CHECK ADD  CONSTRAINT [FK_ResumeScreenings_CandidateApplications_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[CandidateApplications] ([Id])
GO
ALTER TABLE [dbo].[ResumeScreenings] CHECK CONSTRAINT [FK_ResumeScreenings_CandidateApplications_ApplicationId]
GO
ALTER TABLE [dbo].[ResumeScreenings]  WITH CHECK ADD  CONSTRAINT [FK_ResumeScreenings_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[ResumeScreenings] CHECK CONSTRAINT [FK_ResumeScreenings_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Organizations_OrganizationId]
GO
USE [master]
GO
ALTER DATABASE [TAO] SET  READ_WRITE 
GO
