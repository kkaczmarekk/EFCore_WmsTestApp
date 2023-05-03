CREATE OR ALTER PROCEDURE dbo.sp_UpdateCategoriesOwnedStringWithParents (
	@CatId INT,
	@Separator nvarchar(5))
AS BEGIN 

;WITH catWithParents AS (
	SELECT c.Id
		,c.ParentCategoryId
	FROM Categories c
	WHERE c.[Id] = @CatId

	UNION ALL

	SELECT parent.Id
		,parent.ParentCategoryId
	FROM catWithParents child
	INNER JOIN Categories parent
		ON child.ParentCategoryId = parent.[Id])
UPDATE [dbo].[Categories]
SET CategoriesOwnedString = [dbo].[udf_CategoriesOwnedString]([Id], ', ')
WHERE [Id] IN (SELECT Id
	FROM catWithParents)

END
GO