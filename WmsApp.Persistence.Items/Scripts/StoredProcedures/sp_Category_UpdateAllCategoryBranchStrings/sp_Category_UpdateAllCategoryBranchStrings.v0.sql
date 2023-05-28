CREATE OR ALTER PROCEDURE dbo.sp_Category_UpdateAllCategoryBranchStrings (
	@Separator nvarchar(5))
AS BEGIN 

	;WITH category_hierarchy AS (
		SELECT c.[Id]
			,c.[ParentCategoryId]
			,CAST(c.[Name] AS NVARCHAR(4000)) AS [BranchString]
		FROM [Categories] c
		WHERE c.[IsDeleted] = 0
			AND c.[ParentCategoryId] IS NULL

		UNION ALL

		SELECT child.[Id]
			,child.[ParentCategoryId]
			,COALESCE(parent.[BranchString] + @Separator, '') + child.[Name] AS [BranchString]
		FROM category_hierarchy parent
		INNER JOIN [dbo].[Categories] child
			ON child.[ParentCategoryId] = parent.[Id]
		WHERE child.[IsDeleted] = 0)
	UPDATE c
	SET [CategoryBranchString] = ch.[BranchString]
	FROM [Categories] c
	LEFT JOIN category_hierarchy ch
		ON c.[Id] = ch.[Id]
	WHERE ISNULL(c.[CategoryBranchString], '') <> ISNULL(ch.[BranchString], '')
END
GO