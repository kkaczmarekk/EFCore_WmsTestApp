CREATE OR ALTER PROCEDURE dbo.sp_Category_UpdateAllCategoriesOwnedStrings (
	@Separator nvarchar(5))
AS BEGIN 
	;WITH category_hierarchy AS (
			SELECT c.[Id]
				, c.[Id] AS [IdForChilds]
			FROM [Categories] c
			WHERE c.[IsDeleted] = 0 

			UNION ALL

			SELECT parent.[Id]
				, child.[Id] As [IdForChilds]
			FROM category_hierarchy parent
				INNER JOIN [Categories] child
					ON child.[ParentCategoryId] = parent.[IdForChilds]
			WHERE child.[IsDeleted] = 0),
		category_strings AS (SELECT c.[Id]
				, [OwnedString]
			FROM [Categories] c
			LEFT JOIN (SELECT [Id]
						,STRING_AGG(CAST([IdForChilds] AS NVARCHAR(4000)), @Separator) AS [OwnedString]
					FROM category_hierarchy
					WHERE [Id] <> [IdForChilds]
					GROUP BY [Id]) As catWithString
				ON c.[Id] = catWithString.[Id])
	UPDATE c
	SET [CategoriesOwnedString] = cs.[OwnedString]
	FROM [Categories] c
	LEFT JOIN category_strings cs
		ON c.[Id] = cs.[Id]
	WHERE ISNULL(c.[CategoriesOwnedString], '') <> ISNULL(cs.[OwnedString], '')
END
GO