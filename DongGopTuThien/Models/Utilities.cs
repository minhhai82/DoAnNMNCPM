using System.Reflection;

namespace DongGopTuThien.Models
{
    public enum TrangThai
    {
        ChuaXacThuc = 0,
        XacThucDienThoai = 1,
        XacThucGiayPhep = 2,

    }

    public enum TrangThaiChienDich
    {
        Draft = 0,
        Active = 1,
        Cancelled = 2,
        Completed = 3
    }

    public enum TrangThaiDongGop
    {
        ChoDuyet = 0,
        DaDuyet = 1
    }

    public enum Loai
    {
        Admin = 0,
        NguoiDongGop = 1,
        NguoiXinTaiTro = 2,
        ToChucTuThien = 3
    }

    public class Utilities
    {
        /// <summary>
        /// Converts an entity framework object to a DTO.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDto">The type of the DTO.</typeparam>
        /// <param name="entity">The entity instance to be converted.</param>
        /// <returns>A DTO instance with mapped values.</returns>
        public static TDto ConvertToDto<TEntity, TDto>(TEntity entity)
            where TEntity : class
            where TDto : class, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            TDto dto = new TDto();

            // Get properties of the entity and DTO types
            PropertyInfo[] entityProperties = typeof(TEntity).GetProperties();
            PropertyInfo[] dtoProperties = typeof(TDto).GetProperties();

            foreach (var dtoProperty in dtoProperties)
            {
                // Find a matching entity property by name and type
                var matchingEntityProperty = entityProperties
                    .FirstOrDefault(p => p.Name == dtoProperty.Name &&
                                         p.PropertyType == dtoProperty.PropertyType);

                if (matchingEntityProperty != null)
                {
                    // Map the value from the entity to the DTO
                    object value = matchingEntityProperty.GetValue(entity);
                    dtoProperty.SetValue(dto, value);
                }
            }

            return dto;
        }

        /// <summary>
        /// Converts a list of entity objects to a list of DTOs.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TDto">The type of the DTO.</typeparam>
        /// <param name="entities">The list of entities to be converted.</param>
        /// <returns>A list of DTO instances with mapped values.</returns>
        public static List<TDto> ConvertToDtoList<TEntity, TDto>(IEnumerable<TEntity> entities)
            where TEntity : class
            where TDto : class, new()
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return entities.Select(entity => ConvertToDto<TEntity, TDto>(entity)).ToList();
        }
    }
}
