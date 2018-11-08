namespace FaceOff
{
    public static class Keys
    {
#if DEBUG
        public const string EmotionApiKey = "ec3ad08e736b40fea242ccd53da6b745";
        public const string AndroidAppCenterKey = "efcdeee7-b02b-40fa-a6a7-e018290cff86;";
        public const string iOSAppCenterKey = "05e5b809-8f7c-436c-8821-e34c4ad891b3;";
#else
        public const string EmotionApiKey = "emotionkey";
        public const string AndroidAppCenterKey = "androidkey";
        public const string iOSAppCenterKey = "ioskey";
#endif

        public const string CosmodDatabase = "db";
        public const string CosmosCollection = "dataCollection";
        public const string Endpoint = "https://cosmosfunctionsdemo.documents.azure.com";
    }
}
