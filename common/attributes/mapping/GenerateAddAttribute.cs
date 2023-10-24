using System;
using Mapster;

//Thanks to Mapster for this from Sample.Codegen.
namespace CAS.COMMON.attributes.mapping
{
    public sealed class GenerateAddDto : AdaptFromAttribute
    {
        public GenerateAddDto() : base("Add[name]Dto")
        {
            Initialize();
        }

        public GenerateAddDto(Type type) : base(type)
        {
            Initialize();
        }

        private void Initialize()
        {
            IgnoreAttributes = new[]
            {
                typeof(ExcludeFromAddAndUpdateDtoAttribute),
                typeof(ExcludeFromAddDtoAttribute)
            };
            MapType = MapType.MapToTarget;
            ShallowCopyForSameType = true;
        }
    }
}
