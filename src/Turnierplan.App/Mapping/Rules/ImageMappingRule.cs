using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.Image;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ImageMappingRule : MappingRuleBase<Image, ImageDto>
{
    private readonly IImageStorage _imageStorage;

    public ImageMappingRule(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    protected override ImageDto Map(IMapper mapper, MappingContext context, Image source)
    {
        return new ImageDto
        {
            Id = source.PublicId,
            RbacScopeId = source.GetScopeId(),
            CreatedAt = source.CreatedAt,
            Type = source.Type,
            Name = source.Name,
            Url = _imageStorage.GetFullImageUrl(source),
            FileSize = source.FileSize,
            Width = source.Width,
            Height = source.Height
        };
    }
}
