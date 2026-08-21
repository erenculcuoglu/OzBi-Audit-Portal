namespace OzBiPortalCRM.Models
{
    /// <summary>
    /// OzBI Portal CRM - Merkezi ERP Versiyon ve İsimlendirme Standartları.
    /// Tüm denetim motorları, şema analizleri, prompt doğrulama ve UI rozetleri
    /// bu merkezi versiyon sabitlerine tam bağlı çalışır.
    /// </summary>
    public static class ErpVersionConstants
    {
        // 🌟 Logo ERP Active Version (Şema ve Prompt: logo_assistant_schema_v8.0.json & logo_assistant_prompt_v8.0.md)
        public const string LogoVersionNumber = "v8.0";
        public const string LogoSystemTypeName = "Logo ERP (v8.0)";
        public const string LogoPromptSignature = "Logo ERP Ek Talimatı — v8.0";
        public const string LogoPromptVersionLabel = "Logo v8.0 Uyumlu (Pozitif Mimari)";

        // 🌟 Mikro ERP Active Version (Şema ve Prompt: mikro_assistant_schema_v27.1.json & mikro_assistant_prompt_v27.1)
        public const string MikroVersionNumber = "v27.1";
        public const string MikroSystemTypeName = "Mikro ERP (v27.1)";
        public const string MikroPromptSignature = "Mikro ERP Ek Talimatı — v27.1";
        public const string MikroPromptVersionLabel = "Mikro v27.1 Altın Prompt";

        // Genel Uyumluluk Metni
        public const string DefaultErpCompatibility = "Logo v8.0 & Mikro v27.1 Uyumlu";
    }
}
