CREATE OR ALTER FUNCTION dbo.udf_Category_GetCategoryBranchString (
	@CatId INT,
	@Separator nvarchar(5))
RETURNS NVARCHAR(4000)
AS BEGIN 
DECLARE @BranchString AS NVARCHAR(4000)

;WITH category_hierarchy AS (
	SELECT c.[Id]
		,c.[ParentCategoryId]
		,c.[Name]
		,0 AS [hierarchy_level]
	FROM [dbo].[Categories] c
	WHERE c.[Id] = @CatId
		AND c.[IsDeleted] = 0

	UNION ALL

	SELECT parent.[Id]
		,parent.[ParentCategoryId]
		,parent.[Name]
		,[hierarchy_level] + 1
	FROM [category_hierarchy] child
	INNER JOIN [dbo].[Categories] parent
		ON child.[ParentCategoryId] = parent.[Id]
	WHERE parent.[IsDeleted] = 0)
SELECT @BranchString = COALESCE(@BranchString + @Separator, '') + [Name]
FROM category_hierarchy
ORDER BY [hierarchy_level] DESC

RETURN @BranchString
END
GO