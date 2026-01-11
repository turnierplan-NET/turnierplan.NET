using Turnierplan.Core.Organization;
using Turnierplan.Dal.Repositories;
using Turnierplan.ImageStorage;

namespace Turnierplan.App.Helpers;

internal interface IDeletionHelper
{
    Task<bool> DeleteOrganizationAsync(Organization organization, CancellationToken cancellationToken);
}

internal sealed class DeletionHelper : IDeletionHelper
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IPlanningRealmRepository _planningRealmRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IImageStorage _imageStorage;
    private readonly DeletionHelperLogger _logger;

    public DeletionHelper(
        IOrganizationRepository organizationRepository,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IPlanningRealmRepository planningRealmRepository,
        IImageRepository imageRepository,
        IImageStorage imageStorage,
        ILogger<DeletionHelper> logger)
    {
        _organizationRepository = organizationRepository;
        _tournamentRepository = tournamentRepository;
        _venueRepository = venueRepository;
        _planningRealmRepository = planningRealmRepository;
        _imageRepository = imageRepository;
        _imageStorage = imageStorage;
        _logger = new DeletionHelperLogger(logger);
    }

    public async Task<bool> DeleteOrganizationAsync(Organization organization, CancellationToken cancellationToken)
    {
        var hasNonDeletedImages = false;

        foreach (var image in organization.Images.ToList()) // ToList() to avoid invalid operation exception
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _imageStorage.DeleteImageAsync(image);

            if (result)
            {
                try
                {
                    _imageRepository.Remove(image);

                    await _imageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.ImageDeletedButDatabaseRemovalFailed(ex, image.Id);

                    return false;
                }
            }
            else
            {
                _logger.ImageDeletionFromStorageFailed(image.Id, organization.Id);

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

        await _tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var venue in organization.Venues.ToList()) // ToList() to avoid invalid operation exception
        {
            _venueRepository.Remove(venue);
        }

        await _venueRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var planningRealm in organization.PlanningRealms.ToList()) // ToList() to avoid invalid operation exception
        {
            _planningRealmRepository.Remove(planningRealm);
        }

        await _planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Note that ApiKeys and Folders need not be deleted explicitly, because the corresponding
        // foreign keys in the database are configured with the 'Cascade' deletion behaviour.

        _organizationRepository.Remove(organization);

        await _organizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
