using Microsoft.AspNetCore.SignalR;

namespace EPano2.Hubs
{
    /// <summary>
    /// Global SignalR Hub - Tüm uygulama güncellemeleri için kullanılır
    /// Admin panelinde yapılan değişiklikler bu hub üzerinden tüm bağlı istemcilere bildirilir
    /// </summary>
    public class AppUpdateHub : Hub
    {
        // Hub metodları gerekirse buraya eklenebilir
        // Şu an için sadece server-to-client mesajlaşma kullanılıyor
    }
}

