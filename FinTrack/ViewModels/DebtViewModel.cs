using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Dtos.DebtDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class DebtViewModel : ObservableObject
    {
        private readonly ILogger<DebtViewModel> _logger;
        private readonly IApiService _apiService;

        private readonly int _currentUserId;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? newProposalBorrowerEmail;

        [ObservableProperty]
        private decimal newProposalAmount;

        [ObservableProperty]
        private string newProposalDescription;

        [ObservableProperty]
        private DateTime newProposalDueDate = DateTime.Now.AddMonths(1);

        [ObservableProperty]
        private ObservableCollection<DebtModel> pendingOffers;

        [ObservableProperty]
        private ObservableCollection<DebtModel> myDebtsList;

        public DebtViewModel(ILogger<DebtViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(SessionManager.CurrentToken);
            _currentUserId = Convert.ToInt16(jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

            pendingOffers = new ObservableCollection<DebtModel>();
            myDebtsList = new ObservableCollection<DebtModel>();

            _ = LoadDebtsAsync();
        }

        [RelayCommand]
        private async Task LoadDebtsAsync()
        {
            IsLoading = true;
            _logger.LogInformation("Loading debt data from API...");
            try
            {
                var debtDtos = await _apiService.GetAsync<List<DebtDto>>("Debt");
                if (debtDtos == null) return;

                PendingOffers.Clear();
                MyDebtsList.Clear();

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
                        borrowerImageUrl = dto.BorrowerProfilePicture ?? "/Assets/Images/Icons/user-red.png",
                        lenderImageUrl = dto.LenderProfilePicture ?? "/Assets/Images/Icons/user-green.png",
                        Amount = dto.Amount,
                        DueDate = dto.DueDateUtc.ToLocalTime(),
                        Status = dto.Status
                    };

                    // Bana gelen ve henüz karar vermediğim borç teklifleri
                    if (dto.Status == DebtStatusType.PendingBorrowerAcceptance && dto.BorrowerId == _currentUserId)
                    {
                        PendingOffers.Add(debtModel);
                    }
                    else // Diğer tüm borçlarım (aktif, ödenmiş, reddedilmiş vb.)
                    {
                        MyDebtsList.Add(debtModel);
                    }
                }
                _logger.LogInformation("Successfully loaded and processed {Count} debts.", debtDtos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load debt data.");
                MessageBox.Show("Borç verileri yüklenirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SendOfferAsync()
        {
            IsLoading = true;
            try
            {
                if (string.IsNullOrWhiteSpace(NewProposalBorrowerEmail) || NewProposalAmount <= 0)
                {
                    _logger.LogWarning("New proposal validation failed: Borrower email or amount is invalid.");
                    return;
                }

                var createDto = new CreateDebtOfferRequestDto
                {
                    BorrowerEmail = NewProposalBorrowerEmail,
                    Amount = NewProposalAmount,
                    CurrencyCode = BaseCurrencyType.TRY,
                    DueDateUtc = NewProposalDueDate,
                    Description = NewProposalDescription
                };

                _logger.LogInformation("Sending new debt offer to API...");
                var result = await _apiService.PostAsync<object>("Debt/create-debt-offer", createDto);

                if (result != null)
                {
                    NewProposalBorrowerEmail = string.Empty;
                    NewProposalAmount = 0;
                    await LoadDebtsAsync();

                    _logger.LogInformation("Debt offer sent successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send debt offer.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RespondToOfferAsync(object parameter)
        {
            if (parameter is not object[] values || values.Length != 2 || values[0] is not DebtModel debt || values[1] is not bool decision) return;

            IsLoading = true;
            try
            {
                _logger.LogInformation("Attempting to {Action} offer for DebtId: {DebtId}", decision, debt.Id);

                var requestBody = new { Accepted = decision };
                bool result = await _apiService.PostAsync<bool>($"Debt/respond-to-offer/{debt.Id}", requestBody);

                if (result)
                {
                    _logger.LogInformation("Successfully responded to offer for DebtId: {DebtId}", debt.Id);
                    await LoadDebtsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to respond to debt offer for DebtId: {DebtId}", debt.Id);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task UploadVideoAsync(DebtModel debt)
        {
            if (debt == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files (*.mp4;*.mov;*.wmv)|*.mp4;*.mov;*.wmv|All files (*.*)|*.*",
                Title = "Select an Approval Video"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                try
                {
                    _logger.LogInformation("Uploading video for DebtId: {DebtId}", debt.Id);
                    var success = await _apiService.UploadFileAsync($"Videos/user-upload-video?debtId={debt.Id}", openFileDialog.FileName);

                    if (success)
                    {
                        _logger.LogInformation("Video uploaded successfully for DebtId: {DebtId}", debt.Id);
                        await LoadDebtsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload video for DebtId: {DebtId}", debt.Id);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}