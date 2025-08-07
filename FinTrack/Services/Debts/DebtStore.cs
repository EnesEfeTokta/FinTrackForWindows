using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Core;
using FinTrackForWindows.Dtos.DebtDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;

namespace FinTrackForWindows.Services.Debts
{
    public partial class DebtStore : ObservableObject, IDebtStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<DebtStore> _logger;
        private readonly int _currentUserId;

        [ObservableProperty]
        private bool _isLoading;

        private readonly ObservableCollection<DebtModel> _pendingOffers;
        public ReadOnlyObservableCollection<DebtModel> PendingOffers { get; }

        private readonly ObservableCollection<DebtModel> _myDebtsList;
        public ReadOnlyObservableCollection<DebtModel> MyDebtsList { get; }

        private readonly ObservableCollection<DebtModel> _activeDebts;
        public ReadOnlyObservableCollection<DebtModel> ActiveDebts { get; }

        public event Action? DebtsChanged;

        public DebtStore(IApiService apiService, ILogger<DebtStore> logger)
        {
            _apiService = apiService;
            _logger = logger;

            _activeDebts = new ObservableCollection<DebtModel>();
            ActiveDebts = new ReadOnlyObservableCollection<DebtModel>(_activeDebts);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(SessionManager.CurrentToken);
            _currentUserId = Convert.ToInt32(jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

            _pendingOffers = new ObservableCollection<DebtModel>();
            PendingOffers = new ReadOnlyObservableCollection<DebtModel>(_pendingOffers);

            _myDebtsList = new ObservableCollection<DebtModel>();
            MyDebtsList = new ReadOnlyObservableCollection<DebtModel>(_myDebtsList);
        }

        public async Task LoadDebtsAsync()
        {
            IsLoading = true;
            _logger.LogInformation("Loading debt data from API...");
            try
            {
                var debtDtos = await _apiService.GetAsync<List<DebtDto>>("Debt");
                if (debtDtos == null) return;

                _pendingOffers.Clear();
                _myDebtsList.Clear();
                _activeDebts.Clear();

                foreach (var dto in debtDtos)
                {
                    var debtModel = new DebtModel
                    {
                        Id = dto.Id,
                        LenderId = dto.LenderId,
                        BorrowerId = dto.BorrowerId,
                        CurrentUserId = _currentUserId,
                        LenderName = dto.LenderName,
                        BorrowerName = dto.BorrowerName,
                        BorrowerImageUrl = dto.BorrowerProfilePicture ?? "/Assets/Images/Icons/user-red.png",
                        LenderImageUrl = dto.LenderProfilePicture ?? "/Assets/Images/Icons/user-green.png",
                        BorrowerEmail = dto.BorrowerEmail,
                        LenderEmail = dto.LenderEmail,
                        Description = dto.Description,
                        PaymentDate = dto.PaidAtUtc,
                        OperatorApprovalDate = dto.OperatorApprovalAtUtc,
                        Amount = dto.Amount,
                        Currency = dto.Currency,
                        DueDate = dto.DueDateUtc.ToLocalTime(),
                        Status = dto.Status,
                        VideoMetadataId = dto.VideoMetadataId
                    };

                    if (dto.Status == DebtStatusType.PendingBorrowerAcceptance && dto.BorrowerId == _currentUserId)
                    {
                        _pendingOffers.Add(debtModel);
                    }
                    else
                    {
                        _myDebtsList.Add(debtModel);
                    }

                    if (dto.Status == DebtStatusType.Active)
                    {
                        _activeDebts.Add(debtModel);
                    }
                }
                DebtsChanged?.Invoke();
                _logger.LogInformation("Successfully loaded and processed {Count} debts.", debtDtos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load debt data.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SendOfferAsync(string borrowerEmail, decimal amount, string currency, DateTime dueDate, string description)
        {
            IsLoading = true;
            try
            {
                var createDto = new CreateDebtOfferRequestDto
                {
                    BorrowerEmail = borrowerEmail,
                    Amount = amount,
                    CurrencyCode = BaseCurrencyType.TRY,
                    DueDateUtc = dueDate.ToUniversalTime(),
                    Description = description
                };

                _logger.LogInformation("Sending new debt offer to API...");
                await _apiService.PostAsync<object>("Debt/create-debt-offer", createDto);
                await LoadDebtsAsync();
                _logger.LogInformation("Debt offer sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send debt offer.");
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RespondToOfferAsync(DebtModel debt, bool accepted)
        {
            IsLoading = true;
            try
            {
                _logger.LogInformation("Responding to offer for DebtId: {DebtId} with decision: {Decision}", debt.Id, accepted);
                var requestBody = new { Accepted = accepted };
                await _apiService.PostAsync<bool>($"Debt/respond-to-offer/{debt.Id}", requestBody);
                await LoadDebtsAsync();
                _logger.LogInformation("Successfully responded to offer for DebtId: {DebtId}", debt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to respond to debt offer for DebtId: {DebtId}", debt.Id);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UploadVideoAsync(DebtModel debt, string filePath)
        {
            IsLoading = true;
            try
            {
                _logger.LogInformation("Uploading video for DebtId: {DebtId}", debt.Id);
                await _apiService.UploadFileAsync($"Videos/user-upload-video?debtId={debt.Id}", filePath);
                await LoadDebtsAsync();
                _logger.LogInformation("Video uploaded successfully for DebtId: {DebtId}", debt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload video for DebtId: {DebtId}", debt.Id);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task MarkDebtAsDefaultedAsync(int debtId)
        {
            IsLoading = true;
            _logger.LogInformation("Marking debt as defaulted via API. DebtId: {DebtId}", debtId);
            try
            {
                await _apiService.PostAsync<object>($"Debt/mark-as-defaulted/{debtId}", new { });

                await LoadDebtsAsync();

                _logger.LogInformation("Successfully marked debt as defaulted via API. DebtId: {DebtId}", debtId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark debt as defaulted for DebtId: {DebtId}", debtId);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<(Stream? Stream, string? ContentType)> GetVideoStreamAsync(int videoId, string key)
        {
            IsLoading = true;
            _logger.LogInformation("Requesting video stream via API for VideoId: {VideoId}", videoId);
            try
            {
                string encodedKey = System.Net.WebUtility.UrlEncode(key);
                string endpoint = $"Videos/video-metadata-stream/{videoId}?key={encodedKey}";

                var (stream, contentType, _) = await _apiService.StreamFileAsync(endpoint);

                if (stream != null)
                {
                    _logger.LogInformation("Video stream received for VideoId: {VideoId}", videoId);
                    return (stream, contentType);
                }

                _logger.LogWarning("API returned a null stream for VideoId: {VideoId}", videoId);
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get video stream for VideoId: {VideoId}", videoId);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}