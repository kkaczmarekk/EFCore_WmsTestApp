CREATE OR ALTER FUNCTION dbo.udf_CategoriesOwnedString (
	@CatId INT,
	@Separator nvarchar(5))
RETURNS NVARCHAR(4000)
AS BEGIN 
DECLARE @OwnedString AS NVARCHAR(4000)

;WITH category_hierarchy AS (
	SELECT c.[Id]
		,c.[ParentCategoryId]
		,0 AS [hierarchy_level]
	FROM [dbo].[Categories] c
	WHERE c.[Id] = @CatId
		AND c.[IsDeleted] = 0
	UNION ALL

	SELECT child.[Id]
		,child.[ParentCategoryId]
		,[hierarchy_level] + 1
	FROM category_hierarchy parent
	INNER JOIN [dbo].[Categories] child
		ON child.[ParentCategoryId] = parent.[Id]
	WHERE child.[IsDeleted] = 0)
SELECT @OwnedString = COALESCE(@OwnedString + @Separator, '') + CAST([Id] as nvarchar(15))
FROM category_hierarchy
WHERE [hierarchy_level] <> 0
ORDER BY [hierarchy_level] DESC

RETURN @OwnedString
END
GO