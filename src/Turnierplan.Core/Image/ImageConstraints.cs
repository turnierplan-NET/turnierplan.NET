namespace Turnierplan.Core.Image;

public sealed class ImageConstraints
{
    private static readonly ImageConstraints __squareLargeLogoConstraints = new(true);
    private static readonly ImageConstraints __sponsorBannerConstraints = new(false, 3, 5);

    private readonly bool _mustBeSquare;
    private readonly float? _minimumAspectRatio;
    private readonly float? _maximumAspectRatio;

    private ImageConstraints(bool mustBeSquare, float? minimumAspectRatio = null, float? maximumAspectRatio = null)
    {
        if (mustBeSquare)
        {
            if (minimumAspectRatio is not null || maximumAspectRatio is not null)
            {
                throw new ArgumentException("Aspect ratio cannot be set if mustBeSquare is true.");
            }

            _mustBeSquare = true;
        }
        else
        {
            _mustBeSquare = false;
            _minimumAspectRatio = minimumAspectRatio ?? throw new ArgumentNullException(nameof(minimumAspectRatio));
            _maximumAspectRatio = maximumAspectRatio ?? throw new ArgumentNullException(nameof(maximumAspectRatio));
        }
    }

    public bool IsSizeValid(ushort width, ushort height)
    {
        if (width == 0 || height == 0)
        {
            return false;
        }

        if (_mustBeSquare)
        {
            return width == height;
        }

        var aspectRatio = (float)width / height;

        return aspectRatio >= _minimumAspectRatio && aspectRatio <= _maximumAspectRatio;
    }

    public override string ToString()
    {
        return _mustBeSquare
            ? "Must be square"
            : $"Must have aspect ratio between {_minimumAspectRatio:F2} and {_maximumAspectRatio:F2}";
    }

    public static ImageConstraints GetImageConstraints(ImageType imageType)
    {
        return imageType switch
        {
            ImageType.SquareLargeLogo => __squareLargeLogoConstraints,
            ImageType.SponsorBanner => __sponsorBannerConstraints,
            _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, "Invalid image type specified.")
        };
    }
}
