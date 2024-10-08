USE [TutorMioDB]
GO
/****** Object:  StoredProcedure [dbo].[Classes_Insert]    Script Date: 8/18/2024 5:48:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Classes_Insert]
		@Title nvarchar(50) 
		,@Hours float 
		,@Description nvarchar(500) 
		,@ImgUrl nvarchar(500) 
		,@Id int OUTPUT
as
/* TEST CODE
DECLARE @Id int = 0;
DECLARE @Title nvarchar(50) = 'Filtering data and conditional formatting'
		,@Hours float = 3.0
		,@Description nvarchar(500) = 'Building on the concepts covered in Excel overview, we continue developing a personal budget using formulas and conditional formatting. The budget evolves into a budget calendar'
		,@ImgUrl nvarchar(500) = 'https://images.pexels.com/photos/265087/pexels-photo-265087.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2'

EXECUTE [dbo].[Classes_Insert]
            @Title
           ,@Hours
           ,@Description
           ,@ImgUrl
		   ,@Id OUTPUT

	SELECT * FROM dbo.Classes
	WHERE Id = @Id
*/
BEGIN
	
INSERT INTO [dbo].[Classes]
           ([Title]
           ,[Hours]
           ,[Description]
           ,[ImgUrl])
     VALUES
           (@Title
           ,@Hours
           ,@Description
           ,@ImgUrl)

	SET @Id = SCOPE_IDENTITY()
END
GO
