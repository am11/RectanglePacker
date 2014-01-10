using System.Collections.Generic;

namespace Mapper
{
    /// <summary>
    /// Defines an iterative mapper. That is, a mapper that generates a number of candidate sprites
    /// and picks the best one.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public interface IMapperReturningStats<S> : IMapper<S> where S : class, ISprite, new()
    {
        /// <summary>
        /// Version of IMapper.Mapping. See IMapper.
        /// </summary>
        /// <param name="images">Same as for IMapper.Mapping</param>
        /// <param name="mapperStats">
        /// The method will fill the properties of this statistics object.
        /// Set to null if you don't want statistics.
        /// </param>
        /// <returns>
        /// Same as for IMapper.Mapping
        /// </returns>
        S Mapping(IEnumerable<IImageInfo> images, IMapperStats mapperStats);
    }
}
