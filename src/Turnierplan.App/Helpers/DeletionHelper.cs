using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Turnierplan.Core.Venue;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Helpers;

internal interface IDeletionHelper
{
    Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken);

    Task<bool> DeleteOrganizationAsync(Organization organization, CancellationToken cancellationToken);
}

internal sealed class DeletionHelper : IDeletionHelper
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<DeletionHelper> _logger;

    public DeletionHelper(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IImageRepository imageRepository,
        IImageStorage imageStorage,
        ILogger<DeletionHelper> logger)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _tournamentRepository = tournamentRepository;
        _venueRepository = venueRepository;
        _imageRepository = imageRepository;
        _imageStorage = imageStorage;
        _logger = logger;
    }

    public async Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken)
    {
        // TODO: Decide how to handle this (there is no longer a 1-n relation between user and organisation)

        // foreach (var organization in user.Organizations.ToList()) // ToList() to avoid invalid operation exception
        // {
        //     cancellationToken.ThrowIfCancellationRequested();
        //
        //     var result = await DeleteOrganizationAsync(organization, cancellationToken).ConfigureAwait(false);
        //
        //     if (!result)
        //     {
        //         return false;
        //     }
        // }

        cancellationToken.ThrowIfCancellationRequested();

        _userRepository.Remove(user);

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> DeleteOrganizationAsync(Organization organization, CancellationToken cancellationToken)
    {
        var hasNonDeletedImages = false;

        foreach (var image in organization.Images.ToList()) // ToList() to avoid invalid operation exception
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _imageStorage.DeleteImageAsync(image).ConfigureAwait(false);

            if (result)
            {
                try
                {
                    _imageRepository.Remove(image);

                    await _imageRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Image with id '{ImageId}' was successfully deleted from image storage but the deletion from the database failed.", image.Id);

                    return false;
                }
            }
            else
            {
                _logger.LogError("Failed to delete image with id '{ImageId}' from image storage while deleting organization with id '{OrganizationId}'.", image.Id, organization.Id);

                hasNonDeletedImages = true;
            }
        }

        if (hasNonDeletedImages)
        {
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var tournament in organization.Tournaments.ToList()) // ToList() to avoid invalid operation exception
        {
            _tournamentRepository.Remove(tournament);
        }

        await _tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var venue in organization.Venues.ToList()) // ToList() to avoid invalid operation exception
        {
            _venueRepository.Remove(venue);
        }

        await _venueRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        // Note that ApiKeys and Folders need not be deleted explicitly, because the corresponding
        // foreign keys in the database are configured with the 'Cascade' deletion behaviour.

        _organizationRepository.Remove(organization);

        await _organizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return true;
    }
}
