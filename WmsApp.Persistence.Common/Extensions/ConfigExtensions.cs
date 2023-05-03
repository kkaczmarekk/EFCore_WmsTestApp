using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WmsApp.Persistence.Common.Extensions
{
    public static class ConfigExtensions
    {
        public static void ApplyConfigurationForCommonInterfaces(this ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                toDb => toDb.ToUniversalTime(),
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc).ToLocalTime());

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                toDb => toDb.HasValue ? toDb.Value.ToUniversalTime() : toDb,
                fromDb => fromDb.HasValue ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc) : fromDb);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var interfaces = entity.ClrType.GetInterfaces();

                if (interfaces.Contains(typeof(ICreateAudit)))
                {
                    entity.GetProperty(nameof(ICreateAudit.CreatedDateUtc))
                        .SetValueConverter(dateTimeConverter);

                    entity.GetProperty(nameof(ICreateAudit.CreatedByUserName))
                        .SetIsUnicode(false);
                }

                if (interfaces.Contains(typeof(ISoftDeletable)))
                {
                    entity.GetProperty(nameof(ISoftDeletable.DeletedDateUtc))
                        .SetValueConverter(nullableDateTimeConverter);

                    entity.GetProperty(nameof(ISoftDeletable.DeletedByUserName))
                        .SetIsUnicode(false);

                    entity.AddIndex(entity.GetProperty(nameof(ISoftDeletable.IsDeleted)));
                }

                if (interfaces.Contains(typeof(IUpdateAudit)))
                {
                    entity.GetProperty(nameof(IUpdateAudit.UpdatedDateUtc))
                        .SetValueConverter(nullableDateTimeConverter);

                    entity.GetProperty(nameof(IUpdateAudit.UpdatedByUserName))
                        .SetIsUnicode(false);
                }
            }
        }
    }
}
