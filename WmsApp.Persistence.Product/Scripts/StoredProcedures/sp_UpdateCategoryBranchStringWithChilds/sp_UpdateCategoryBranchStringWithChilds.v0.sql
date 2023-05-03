CREATE OR ALTER PROCEDURE dbo.sp_UpdateCategoryBranchStringWithChilds (
	@CatId INT,
	@Separator nvarchar(5))
AS BEGIN 
;WITH catWithChilds AS (
	SELECT c.Id
		,c.ParentCategoryId
	FROM Categories c
	WHERE c.[Id] = @CatId

	UNION ALL

	SELECT child.Id
		,child.ParentCategoryId
	FROM catWithChilds parent
	INNER JOIN Categories child
		ON child.ParentCategoryId = parent.[Id])
UPDATE [dbo].[Categories]
SET CategoryBranchString = [dbo].[udf_CategoryBranchString]([Id], @Separator)
WHERE [Id] IN (SELECT Id
	FROM catWithChilds)

END
GO