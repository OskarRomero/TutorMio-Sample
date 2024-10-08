USE [TutorMioDB]
GO
/****** Object:  StoredProcedure [dbo].[Course_Select_Detailed]    Script Date: 8/18/2024 5:48:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Course_Select_Detailed]
as
/*TESTCODE

EXECUTE [dbo].[Course_Select_Detailed]

*/
BEGIN
		select  Id
			   ,Name
			   ,ImgUrl
			   ,Description
			   ,CourseClasses = 
				   (SELECT	 Id 
							,c.Id as CourseId
							,Title
							,FORMAT(Hours, 'N1') AS Hours
							,Description
							,ImgUrl
							,Features =
								(select FeatureOne
										,FeatureTwo
										,FeatureThree
										,FeatureFour
										,FeatureFive
								from dbo.Features as f
								WHERE f.ClassId = classes.Id
								FOR JSON AUTO
								)
					FROM dbo.Classes as classes
					INNER JOIN dbo.CourseClasses as cc on classes.Id = cc.ClassId 
					WHERE  cc.CourseId =c.Id 
					FOR JSON AUTO
				   )
				,DateModified
				,DateCreated
		from dbo.Courses as c
END
GO
