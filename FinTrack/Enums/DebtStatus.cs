namespace FinTrack.Enums
{
    public enum DebtStatus
    {
        PendingProposal,        // Teklif gönderildi, borçlunun onayı bekleniyor
        AwaitingVideoUpload,    // Teklif kabul edildi, video yüklenmesi bekleniyor
        AwaitingOperatorApproval,// Video yüklendi, operatör onayı bekleniyor
        Active,                 // Operatör onayladı, borç aktif
        RejectedByBorrower,     // Borçlu teklifi reddetti
        RejectedByOperator,     // Operatör videoyu reddetti
        Completed               // Borç ödendi
    }
}
