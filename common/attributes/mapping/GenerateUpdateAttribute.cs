using System;
using Mapster;

//Thanks to Mapster for this from Sample.Codegen.
namespace CAS.COMMON.attributes.mapping
{
    public sealed class GenerateUpdateDto : AdaptFromAttribute
    {
        public GenerateUpdateDto() : base("Update[name]Dto")
        {
            Initialize();
        }

        public GenerateUpdateDto(Type type) : base(type)
        {
            Initialize();
        }

        private void Initialize()
        {
            IgnoreAttributes = new[]
            {
                typeof(ExcludeFromAddAndUpdateDtoAttribute)
            };
            MapType = MapType.MapToTarget;
            ShallowCopyForSameType = true;
        }
    }
}
